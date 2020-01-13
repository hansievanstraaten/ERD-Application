using ERD.Base;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WPF.Tools.ToolModels;

namespace ERD.Common
{
  public static class Integrity
  {
    private static List<string> tableMasterList = new List<string>();

    private static List<string> foreignKeyConstraintNames = new List<string>();

    private static Dictionary<string, int> globalColumnsCount = new Dictionary<string, int>();

    private static Dictionary<string, SqlDbType> globalColumnsDataType = new Dictionary<string, SqlDbType>();

    /// <summary>
    /// Key = ownerTable to lower
    /// </summary>
    private static Dictionary<string, List<string>> tableColumns = new Dictionary<string, List<string>>();

    /// <summary>
    /// Key = ColumnName to lower. This is used to get the columns that are primary keys only and used for building relations
    /// Value = the Master table oqning the primary key
    /// </summary>
    private static Dictionary<string, string> primaryColumns = new Dictionary<string, string>();

    private static Dictionary<string, ColumnObjectModel> columnObjectModels = new Dictionary<string, ColumnObjectModel>();

    private static object globalColumnsCountLock = new object();

    public static List<string> DropRelations = new List<string>();

    public static bool KeepColumnsUnique {get; set;}

    public static bool AllowDatabaseRelations {get; set;}

    public static bool AllowVertualRelations {get; set;}
    
    public static bool HasGlobalColumn(string columnName)
    {
      lock (globalColumnsCountLock)
      {
        string columnKey = columnName.ToLower();

        return Integrity.globalColumnsCount.Keys.Any(k => k.ToLower() == columnKey);
      }
    }

    public static bool IsColumnInThisTable(string tableName, string columnName)
    {
      string tableNameKey = tableName.ToLower();

      if (!Integrity.tableColumns.ContainsKey(tableNameKey))
      {
        return false;
      }

      string columnKey = columnName.ToLower();

      return Integrity.tableColumns[tableNameKey].Any(k => k.ToLower() == columnKey);
    }
    
    public static bool IsPrimaryKeyColumn(string columnName)
    {
      string columnKey = columnName.ToLower();

      return Integrity.primaryColumns.ContainsKey(columnKey);
    }

    public static int GetGlobalPrecision(string columnName)
    {
      string columnKey = $"||{columnName.ToLower()}";

      if (!Integrity.columnObjectModels.Any(k => k.Key.Contains(columnKey)))
      {
        return 0;
      }

      return Integrity.columnObjectModels.First(ck => ck.Key.Contains(columnKey)).Value.Precision;
    }

    public static int GetGlobalScale(string columnName)
    {
      string columnKey = $"||{columnName.ToLower()}";

      if (!Integrity.columnObjectModels.Any(k => k.Key.ToLower().Contains(columnKey)))
      {
        return 0;
      }

      return Integrity.columnObjectModels.First(ck => ck.Key.Contains(columnKey)).Value.Scale;
    }

    public static int GetGlobalMaxLength(string columnName)
    {
      string columnKey = $"||{columnName.ToLower()}";

      if (!Integrity.columnObjectModels.Any(k => k.Key.Contains(columnKey)))
      {
        return 0;
      }

      return Integrity.columnObjectModels.First(ck => ck.Key.Contains(columnKey)).Value.MaxLength;
    }

    public static SqlDbType? GetGlobalColumnDataType(string columnName)
    {
      string columnKey = columnName.ToLower();

      if (!Integrity.globalColumnsDataType.Any(k => k.Key.ToLower() == columnKey))
      {
        return null;
      }

      return Integrity.globalColumnsDataType.First(ck => ck.Key.ToLower() == columnKey).Value;
    }

    public static string BuildForeighKeyName(string parentTable, string childTable)
    {
      string result = $"FK_{parentTable}_{childTable}";

      if (!Integrity.foreignKeyConstraintNames.Contains(result))
      {
        return result;
      }

      int itemCount = 0;

      while (Integrity.foreignKeyConstraintNames.Contains($"{result}_{itemCount}"))
      {
        itemCount++;
      }

      return $"{result}_{itemCount}";
    }

    public static string GetPrimaryKeyTable(string columnName)
    {
      string columnKey = columnName.ToLower();

      if (!Integrity.primaryColumns.ContainsKey(columnKey))
      {
        return string.Empty;
      }

      return Integrity.primaryColumns[columnKey];
    }

    public static string GetFriendlyName(string columnName)
    {
      string columnNameKey = columnName.ToLower();

      string objectModelKey = $"||{columnNameKey}";

      List<ColumnObjectModel> machList = Integrity.columnObjectModels.Where(tk => tk.Key.EndsWith(objectModelKey)).Select(v => v.Value).ToList();

      if (machList.Count == 0)
      {
        return string.Empty;
      }

      // First try the option where the column is a primary key
      ColumnObjectModel keyColumn = machList.FirstOrDefault(pk => pk.InPrimaryKey && !pk.IsForeignkey);

      if (keyColumn != null)
      {
        return keyColumn.FriendlyName;
      }

      return machList[0].FriendlyName;
    }

    public static string GetDescription(string columnName)
    {
      string columnNameKey = columnName.ToLower();

      string objectModelKey = $"||{columnNameKey}";

      List<ColumnObjectModel> machList = Integrity.columnObjectModels.Where(tk => tk.Key.EndsWith(objectModelKey)).Select(v => v.Value).ToList();

      if (machList.Count == 0)
      {
        return string.Empty;
      }

      // First try the option where the column is a primary key
      ColumnObjectModel keyColumn = machList.FirstOrDefault(pk => pk.InPrimaryKey && !pk.IsForeignkey);

      if (keyColumn != null)
      {
        return keyColumn.Description;
      }

      return machList[0].Description;
    }

    public static ColumnObjectModel GetObjectModel(string ownerTable, string columnName)
    {
      string ownerTableKey = ownerTable.ToLower();

      string columnNameKey = columnName.ToLower();

      string objectModelKey = $"{ownerTableKey}||{columnNameKey}";

      if (!Integrity.columnObjectModels.ContainsKey(objectModelKey))
      {
        return null;
      }

      return Integrity.columnObjectModels[objectModelKey];
    }

    public static List<ColumnRelationMapModel> GetConstraintNames(string ownerTable, string columnName)
    {
      string ownerTableKey = $"{ownerTable.ToLower()}||";

      string columnNameKey = columnName.ToLower();

      string searchKey = $"||{columnNameKey}";

      List<ColumnRelationMapModel> result = new List<ColumnRelationMapModel>();

      foreach (KeyValuePair<string, ColumnObjectModel> column in Integrity.columnObjectModels
        .Where(tm => tm.Value.ForeignKeyTable == ownerTable && tm.Value.ForeignKeyColumn == columnName))
      {
        if (column.Key.StartsWith(ownerTableKey))
        {
          continue;
        }

        string[] keySplit = column.Key.Split(new string[] {"||"}, StringSplitOptions.RemoveEmptyEntries);

        result.Add(new ColumnRelationMapModel
        {
          ForeignConstraintName = column.Value.ForeignConstraintName,
          ParentTable = ownerTable,
          ParentColumn = columnName,
          ChildTable = keySplit[0],
          ChildColumn =  column.Value.ColumnName
        });

      }

      return result;
    }

    public static List<ColumnObjectModel> GetObjectModel(string ownerTable)
    {
      string ownerTableKey = $"{ownerTable.ToLower()}||";
      
      if (!Integrity.columnObjectModels.Any(k => k.Key.StartsWith(ownerTableKey)))
      {
        return null;
      }

      return Integrity.columnObjectModels.Where(tk => tk.Key.StartsWith(ownerTableKey)).Select(v => v.Value).ToList();
    }

    public static List<DataItemModel> GetSystemTables()
    {
      return Integrity.tableMasterList.Select(tm => new DataItemModel {DisplayValue = tm, ItemKey = tm}).OrderBy(s => s.DisplayValue).ToList();
    }

    public static List<DataItemModel> GetSystemColumns()
    {
      lock (globalColumnsCountLock)
      {
        return Integrity.globalColumnsCount.Select(co => co)
          .Select(ss => new DataItemModel {DisplayValue = ss.Key, ItemKey = ss.Key})
          .OrderBy(o => o.ItemKey.ParseToString())
          .ToList();
      }
    }

    public static List<DataItemModel> GetColumnsForTable(string tableName)
    {
      tableName = tableName.ToLower();

      if (!Integrity.tableColumns.ContainsKey(tableName))
      {
        return new List<DataItemModel>();
      }

      return Integrity.tableColumns[tableName].Select(di =>  new DataItemModel { DisplayValue = di, ItemKey = di }).ToList();
    }    
  
    public static void MapTable(TableModel table)
    {
      string tableNameKey = table.TableName.ToLower();

      if (!Integrity.tableMasterList.Any(t => t.ToLower() == tableNameKey))
      { // NOTE: We check case but dont use it
        Integrity.tableMasterList.Add(table.TableName);
      }

      if (!Integrity.tableColumns.ContainsKey(tableNameKey))
      {
        Integrity.tableColumns.Add(tableNameKey, new List<string>());
      }

      if (!table.Columns.HasElements())
      { // Nothing to do futher
        return;
      }

      foreach (ColumnObjectModel column in table.Columns)
      {
        Integrity.MapColumn(column, table.TableName);
      }
    }

    public static void MapColumn(ColumnObjectModel column, string ownerTable)
    {
      string ownerTableKey = ownerTable.ToLower();

      string columnNameKey = column.ColumnName.ToLower();

      string objectModelKey = $"{ownerTableKey}||{columnNameKey}";

      if (!Integrity.columnObjectModels.ContainsKey(objectModelKey))
      {
        Integrity.columnObjectModels.Add(objectModelKey, column);
      }

      if (!Integrity.tableColumns[ownerTableKey].Any(cn => cn.ToLower() == columnNameKey))
      {
        Integrity.tableColumns[ownerTableKey].Add(column.ColumnName);
      }

      if (column.InPrimaryKey && !column.IsForeignkey && !Integrity.primaryColumns.ContainsKey(columnNameKey))
      {
        Integrity.primaryColumns.Add(columnNameKey, ownerTable);
      }

      if (column.IsForeignkey && !Integrity.foreignKeyConstraintNames.Any(fk => fk.ToLower() == column.ForeignConstraintName.ToLower()))
      {
        Integrity.foreignKeyConstraintNames.Add(column.ForeignConstraintName); 
      }

      lock (globalColumnsCountLock)
      {
        if (!Integrity.globalColumnsCount.Any(gl => gl.Key.ToLower() == columnNameKey))
        {
          Integrity.globalColumnsCount.Add(columnNameKey, 1);

          if (column.SqlDataType.HasValue)
          {
            Integrity.globalColumnsDataType.Add(column.ColumnName, column.SqlDataType.Value);
          }
        }
        else
        {
          int previousCount = Integrity.globalColumnsCount[columnNameKey];

          previousCount++;

          Integrity.globalColumnsCount.Remove(columnNameKey);

          Integrity.globalColumnsCount.Add(columnNameKey, previousCount);
        }
      }
    }

    public static void RemoveTableMapping(TableModel table)
    {
      string tableNameKey = table.TableName.ToLower();

      if (table.Columns.HasElements())
      {
        foreach (ColumnObjectModel column in table.Columns)
        {
          Integrity.RemoveColumnMapping(column, table.TableName);
        }
      }

      if (!Integrity.tableColumns.ContainsKey(tableNameKey))
      {
        Integrity.tableColumns.Remove(tableNameKey);
      }

      if (!Integrity.tableMasterList.Any(t => t.ToLower() == tableNameKey))
      { // NOTE: We check case but dont use it
        Integrity.tableMasterList.Remove(table.TableName);
      }
    }

    public static void RemoveColumnMapping(ColumnObjectModel column, string ownerTable)
    {
      ownerTable = ownerTable.ToLower();

      string columnNameKey = column.ColumnName.ToLower();

      lock (globalColumnsCountLock)
      {
        int previousCount = Integrity.globalColumnsCount.ContainsKey(column.ColumnName) ?
          Integrity.globalColumnsCount[column.ColumnName] :
          0;

        previousCount--;

        Integrity.globalColumnsCount.Remove(column.ColumnName);

        if (previousCount > 0)
        {
          Integrity.globalColumnsCount.Add(columnNameKey, previousCount);
        }
      }

      if (column.IsForeignkey && !Integrity.foreignKeyConstraintNames.Any(fk => fk.ToLower() == column.ForeignConstraintName.ToLower()))
      {
        Integrity.foreignKeyConstraintNames.Remove(column.ForeignConstraintName); 
      }

      if (column.InPrimaryKey && !column.IsForeignkey && !Integrity.primaryColumns.ContainsKey(columnNameKey))
      {
        Integrity.primaryColumns.Remove(columnNameKey);
      }
      
      if (!Integrity.tableColumns[ownerTable].Any(cn => cn.ToLower() == columnNameKey))
      {
        Integrity.tableColumns[ownerTable].Remove(column.ColumnName);
      }
    }

    public static void RemoveForeighKey(DatabaseRelation relation)
    {
      foreach (ColumnRelationMapModel mappedColumn in relation.Columns)
      {
        string columnKey = $"{mappedColumn.ChildTable.ToLower()}||{mappedColumn.ChildColumn.ToLower()}";

        ColumnObjectModel column = Integrity.columnObjectModels[columnKey];

        string constraintKey = $"{mappedColumn.ChildTable}||{column.ForeignConstraintName}";

        if (relation.RelationType == RelationTypesEnum.DatabaseRelation && !Integrity.DropRelations.Contains(constraintKey))
        {
          Integrity.DropRelations.Add(constraintKey);
        }

        column.ForeignConstraintName = string.Empty;

        column.ForeignKeyColumn = string.Empty;

        column.ForeignKeyTable = string.Empty;

        column.IsForeignkey = false;
      }
    }
  }
}
