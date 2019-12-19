using ERD.DatabaseScripts.Engineering;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

      this.CheckTables();

      this.CheckColumns();

      return result;
    }

    private void CheckTables()
    {
      this.databaseTables = reverse.GetTables();

      foreach (TableModel fromDatabase in this.databaseTables)
      {
        EventParser.ParseMessage(this, dispatcher, fromDatabase.TableName, $"Compare Table {fromDatabase.TableName} from Database to ERD");

        if (!this.canvasTables.Any(ct => ct.TableName.ToUpper() == fromDatabase.TableName.ToUpper()))
        {
          result.Add(new CompareResultModel
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
          result.Add(new CompareResultModel
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
      foreach (TableModel fromDatabase in this.databaseTables)
      {
        EventParser.ParseMessage(this, dispatcher, "Getting Columns", fromDatabase.TableName);

        fromDatabase.Columns = this.reverse.GetTableColumns(fromDatabase.TableName).ToArray();

        TableModel fromCanvas = this.canvasTables.FirstOrDefault(tn => tn.TableName.ToUpper() == fromDatabase.TableName.ToUpper());

        if (fromCanvas == null ||
            fromCanvas.ErdSegmentModelName.IsNullEmptyOrWhiteSpace())
        {
          // This was catched in this.CheckTables() 
          // OR
          // The table is not on A Canvas
          continue;
        }

        #region COMPARE DATABASE COLUMNS TO MODEL

        foreach (ColumnObjectModel databaseColumn in fromDatabase.Columns)
        {
          ColumnObjectModel canvasColummn = fromCanvas.Columns.FirstOrDefault(dc => dc.ColumnName.ToUpper() == databaseColumn.ColumnName.ToUpper());

          if (canvasColummn == null)
          {
            result.Add(new CompareResultModel
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
            result.Add(new CompareResultModel
            {
              TableObject = fromDatabase,
              ObjectName = databaseColumn.ColumnName,
              Message = $"Data Type differs; Database Data Type {databaseColumn.DataType}; ERD Model Data Type{canvasColummn.DataType}.",
              ObjectType = ObjectTypeEnum.Column,
              ObjectAction = ObjectActionEnum.CorrectInDatabase
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
            result.Add(new CompareResultModel
            {
              TableObject = fromCanvas,
              ObjectName = canvasColumn.ColumnName,
              Message = "Column Exist on ERD Model Database but not in Database.",
              ObjectType = ObjectTypeEnum.Column,
              ObjectAction = ObjectActionEnum.DropFromDatabase
            });
          }
        }

        #endregion
      }
    }
  }
}
