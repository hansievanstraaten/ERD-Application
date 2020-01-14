using ERD.DatabaseScripts.Engineering;
using ERD.Models;
using GeneralExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using WPF.Tools.Functions;

namespace ERD.DatabaseScripts.Compare
{
  public class DatabaseCompare
  {
    private Dispatcher dispatcher;

    private ReverseEngineer reverse;

    private List<TableModel> canvasTables;

    private List<TableModel> databaseTables;

    private List<CompareResultModel> result = new List<CompareResultModel>();

    public DatabaseCompare(Dispatcher applicationDispatcher)
    {
      this.dispatcher = applicationDispatcher;
    }

    public List<CompareResultModel> RunComparison(List<TableModel> tablesList)
    {
      this.canvasTables = tablesList;

      this.reverse = new ReverseEngineer(this.dispatcher);

      this.databaseTables = reverse.GetTables();
      
      this.CheckTables();

      this.CheckColumns();
      
      this.CheckRelations();

      return this.result;
    }
    
    private void CheckTables()
    {
      foreach (TableModel fromDatabase in this.databaseTables)
      {
        EventParser.ParseMessage(this, dispatcher, fromDatabase.TableName, $"Compare Table {fromDatabase.TableName} from Database to ERD");

        if (!this.canvasTables.Any(ct => ct.TableName.ToUpper() == fromDatabase.TableName.ToUpper()))
        {
          this.result.Add(new CompareResultModel
          {
            TableObject = fromDatabase,
            ObjectName = fromDatabase.TableName,
            Message = "Table exist in Database but not on ERD Model.",
            ObjectType = ObjectTypeEnum.Table,
            ObjectAction = ObjectActionEnum.DropFromDatabase
          });
        }
      }

      foreach (TableModel fromCanvas in this.canvasTables)
      {
        EventParser.ParseMessage(this, dispatcher, fromCanvas.TableName, $"Compare Table {fromCanvas.TableName} from ERD to Database");

        if (!this.databaseTables.Any(ct => ct.TableName.ToUpper() == fromCanvas.TableName.ToUpper()))
        {
          this.result.Add(new CompareResultModel
          {
            TableObject = fromCanvas,
            ObjectName = fromCanvas.TableName,
            Message = "Table exists on ERD Model but not in Database.",
            ObjectType = ObjectTypeEnum.Table,
            ObjectAction = ObjectActionEnum.CreateInDatabase
          });
        }
      }
    }

    private void CheckColumns()
    {
      string[] databaseTableNames = this.databaseTables.Select(tn => tn.TableName).ToArray();

      string[] erdTableNames = this.canvasTables.Select(tn => tn.TableName).ToArray();

      Dictionary<string, List<ColumnObjectModel>> fromDatabaseList = this.reverse.GetInTableColumns(databaseTableNames);

      Dictionary<string, List<ColumnObjectModel>> fromErdList = this.reverse.GetInTableColumns(erdTableNames);

      foreach (TableModel fromDatabase in this.databaseTables)
      {
        EventParser.ParseMessage(this, dispatcher, "Getting Columns", fromDatabase.TableName);

        fromDatabase.Columns = fromDatabaseList[fromDatabase.TableName].ToArray();
          //this.reverse.GetTableColumns(fromDatabase.TableName).ToArray();

        TableModel fromCanvas = this.canvasTables.FirstOrDefault(tn => tn.TableName.ToUpper() == fromDatabase.TableName.ToUpper());

        if (fromCanvas == null ||
            fromCanvas.ErdSegmentModelName.IsNullEmptyOrWhiteSpace())
        {
          // This was catched in this.CheckTables() 
          continue;
        }

        if (fromCanvas.ErdSegmentModelName.IsNullEmptyOrWhiteSpace())
        {
          // The table is not on A Canvas, but was read on startup
          // We need the columns though
          fromCanvas.Columns = fromErdList[fromDatabase.TableName].ToArray();
          //this.reverse.GetTableColumns(fromDatabase.TableName).ToArray();
        }

        #region COMPARE DATABASE COLUMNS TO MODEL

        foreach (ColumnObjectModel databaseColumn in fromDatabase.Columns)
        {
          ColumnObjectModel canvasColummn = fromCanvas.Columns.FirstOrDefault(dc => dc.ColumnName.ToUpper() == databaseColumn.ColumnName.ToUpper());

          if (canvasColummn == null)
          {
            this.result.Add(new CompareResultModel
            {
              TableObject = fromDatabase,
              ObjectName = databaseColumn.ColumnName,
              Message = "Column Exist in Database but not on ERD Model.",
              ObjectType = ObjectTypeEnum.Column,
              ObjectAction = ObjectActionEnum.DropFromDatabase
            });
          }
          else if (databaseColumn.DataType != canvasColummn.DataType)
          {
            this.result.Add(new CompareResultModel
            {
              TableObject = fromCanvas,
              ObjectName = canvasColummn.ColumnName,
              Message = $"Data Type differs; Database Data Type {databaseColumn.DataType}; ERD Model Data Type{canvasColummn.DataType}.",
              ObjectType = ObjectTypeEnum.Column,
              ObjectAction = ObjectActionEnum.AlterDatabase
            });
          }
        }

        #endregion

        #region COMPARE DATABASE COLUMNS TO MODEL

        foreach (ColumnObjectModel canvasColumn in fromCanvas.Columns)
        {
          ColumnObjectModel databaseColummn = fromDatabase.Columns.FirstOrDefault(dc => dc.ColumnName.ToUpper() == canvasColumn.ColumnName.ToUpper());

          if (databaseColummn == null)
          {
            this.result.Add(new CompareResultModel
            {
              TableObject = fromCanvas,
              ObjectName = canvasColumn.ColumnName,
              Message = "Column Exist on ERD Model Database but not in Database.",
              ObjectType = ObjectTypeEnum.Column,
              ObjectAction = ObjectActionEnum.CreateInDatabase
            });
          }
        }

        #endregion
      }
    }

    private void CheckRelations()
    {
      foreach (TableModel fromDatabase in this.databaseTables.Where(fk => fk.Columns.Any(col => col.IsForeignkey)))
      {
        EventParser.ParseMessage(this, dispatcher, fromDatabase.TableName, $"Compare Table {fromDatabase.TableName} Foreign Constraints");

        Dictionary<string, List<ColumnObjectModel>> databaseForeignkeyStructure = fromDatabase.Columns
          .Where(dk => dk.IsForeignkey
                      && !dk.IsVertualRelation)
          .GroupBy(dg => dg.ForeignKeyTable.ToUpper())
          .ToDictionary(dd => dd.Key, dd => dd.ToList());

        foreach(KeyValuePair<string, List<ColumnObjectModel>> keyPair in databaseForeignkeyStructure)
        {
          TableModel canvasKeyTable = this.canvasTables
            .FirstOrDefault(ck => ck.TableName.ToUpper() == fromDatabase.TableName.ToUpper());

          if (canvasKeyTable == null)
          { // This was handled in the Table Comparison
            continue;
          }

          foreach(ColumnObjectModel databaseColumn in keyPair.Value)
          {
            ColumnObjectModel canvasColumn = canvasKeyTable.Columns.FirstOrDefault(cc => cc.ColumnName.ToUpper() == databaseColumn.ColumnName.ToUpper());

            if (canvasColumn == null)
            { // This was handled in the Column Comparison
              continue;
            }
            
            if (databaseColumn.ForeignKeyColumn.ToUpper() != canvasColumn.ForeignKeyColumn.ToUpper())
            {
              this.result.Add(new CompareResultModel
              {
                TableObject = fromDatabase,
                ObjectName = databaseColumn.ForeignConstraintName,
                Message = $"Foreign Key Constraint Exist in Database Model but not on ERD Model. ({databaseColumn.ColumnName})",
                ObjectType = ObjectTypeEnum.ForeignKeyConstraint,
                ObjectAction = ObjectActionEnum.DropFromDatabase
              });
            }
          }
        }
      }

      foreach (TableModel fromCanvas in this.canvasTables.Where(fk => fk.Columns.Any(col => col.IsForeignkey && !col.IsVertualRelation)))
      {
        EventParser.ParseMessage(this, dispatcher, fromCanvas.TableName, $"Compare Table {fromCanvas.TableName} Foreign Constraints");

        Dictionary<string, List<ColumnObjectModel>> canvasForeignkeyStructure = fromCanvas.Columns
          .Where(dk => dk.IsForeignkey
                      && !dk.IsVertualRelation)
          .GroupBy(dg => dg.ForeignKeyTable.ToUpper())
          .ToDictionary(dd => dd.Key, dd => dd.ToList());

        foreach (KeyValuePair<string, List<ColumnObjectModel>> keyPair in canvasForeignkeyStructure)
        {
          TableModel canvasKeyTable = this.databaseTables
            .FirstOrDefault(ck => ck.TableName.ToUpper() == fromCanvas.TableName.ToUpper());

          if (canvasKeyTable == null)
          { // This was handled in the Table Comparison
            continue;
          }

          foreach (ColumnObjectModel databaseColumn in keyPair.Value)
          {
            ColumnObjectModel canvasColumn = canvasKeyTable.Columns.FirstOrDefault(cc => cc.ColumnName.ToUpper() == databaseColumn.ColumnName.ToUpper());

            if (canvasColumn == null)
            { // This was handled in the Column Comparison
              continue;
            }

            if (databaseColumn.ForeignKeyColumn.ToUpper() != canvasColumn.ForeignKeyColumn.ToUpper())
            {
              this.result.Add(new CompareResultModel
              {
                TableObject = fromCanvas,
                ObjectName = databaseColumn.ForeignConstraintName,
                Message = $"Foreign Key Constraint Exist on ERD Model but not in Database. ({databaseColumn.ColumnName})",
                ObjectType = ObjectTypeEnum.ForeignKeyConstraint,
                ObjectAction = ObjectActionEnum.CreateInDatabase
              });
            }
          }
        }
      }
    }
  }
}
