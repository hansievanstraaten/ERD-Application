﻿using ERD.Common;
using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Threading;
using System.Xml.Linq;
using WPF.Tools.Functions;

namespace ERD.DatabaseScripts
{
  internal class MsReverseEngineer : IReverseEngineer
  {
    private Dispatcher dispatcher;

    internal MsReverseEngineer(Dispatcher senderDispatcher)
    {
      this.dispatcher = senderDispatcher;
    }

    public string GetTablePrimaryKeyCluster(string tableName)
    {
      DataAccess dataAccess = new DataAccess(Connections.DatabaseModel);

      List<dynamic> clusterList = dataAccess.ExecuteQueryDynamic(SQLQueries.DatabaseQueries.DatabasePrimaryClusterName(tableName));

      return clusterList.Count > 0 ? ((IDictionary<string, object>)clusterList[0]).Values.FirstOrDefault().ToString() : $"PK_{tableName}";
    }
    
    /// <summary>
    /// Loads the tables from the database
    /// </summary>
    /// <param name="databaseModel">the DatabaseModel object to exeute the query against</param>
    /// <returns>returns a List<TableModel> (Columns not loaded)</returns>
    public List<TableModel> GetTables(Dispatcher dispatcher)
    {
      List<TableModel> result = new List<TableModel>();

      DataAccess dataAccess = new DataAccess(Connections.DatabaseModel);

      XDocument tablesXml = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseTablesQuery(Connections.DatabaseModel.DatabaseName));

      foreach (XElement rowItem in tablesXml.Root.Elements())
      {
        string tableName = rowItem.Element("TABLE_NAME").Value;

        EventParser.ParseMessage(this, dispatcher, "Reading", tableName);

        if (tableName.ToLower() == "sysdiagrams")
        {
          continue;
        }

        List<dynamic> clusterList = dataAccess.ExecuteQueryDynamic(SQLQueries.DatabaseQueries.DatabasePrimaryClusterName(tableName));

        TableModel model = new TableModel {TableName = tableName};
        
        model.PrimaryKeyClusterConstraintName = clusterList.Count > 0 ? ((IDictionary<string, object>)clusterList[0]).Values.FirstOrDefault().ToString() : tableName;

        result.Add(model);
      }

      return result;
    }

    public List<ColumnObjectModel> GetTableColumns(string tableName)
    {
      EventParser.ParseMessage(this, this.dispatcher, "Reading Table ", tableName);

      List<ColumnObjectModel> result = new List<ColumnObjectModel>();

      DataAccess dataAccess = new DataAccess(Connections.DatabaseModel);

      XDocument columnsXml = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseTableColumnsQuery(tableName));

      foreach (XElement item in columnsXml.Root.Elements())
      {
        string columnName = item.Element("COLUMNNAME").Value;

        XDocument primaryKey = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseColumnKeysQuery(tableName, columnName));

        ColumnObjectModel column = new ColumnObjectModel
        {
          ColumnName = columnName,
          IsIdentity = item.Element("IS_IDENTITY").Value.ToBool(),
          AllowNulls = item.Element("IS_NULLABLE").Value.ToBool(),
          MaxLength = item.Element("MAX_LENGTH").Value.ToInt32(),
          Precision = item.Element("PRECISION").Value.ToInt32(),
          Scale = item.Element("SCALE").Value.ToInt32(),
          IsForeignkey = !item.Element("PRIMARY_TABLE").Value.IsNullEmptyOrWhiteSpace(),
          ForeignKeyTable = item.Element("PRIMARY_TABLE").Value,
          ForeignKeyColumn = item.Element("PRIMARY_COLUMNNAME").Value,
          ForeignConstraintName = item.Element("FK_CONSTRAINT_NAME").Value,
          SqlDataType = this.ParseSqlDbType(item.Element("DATA_TYPE").Value),
          InPrimaryKey = primaryKey.Descendants().Any(d => d.Value == "PRIMARY KEY"),
          Column_Id = item.Element("COLUMN_ID").Value.ToInt32()
        };

        column.HasModelChanged = false;

        result.Add(column);
      }

      return result;
    }

    private SqlDbType ParseSqlDbType(string value)
    {
      switch (value.ToLower())
      {
        case "bigint": return SqlDbType.BigInt;
        case "binary": return SqlDbType.Binary;
        case "bit": return SqlDbType.Bit;
        case "char": return SqlDbType.Char;
        case "datetime": return SqlDbType.DateTime;
        case "decimal": return SqlDbType.Decimal;
        case "float": return SqlDbType.Float;
        case "image": return SqlDbType.Image;
        case "int": return SqlDbType.Int;
        case "money": return SqlDbType.Money;
        case "nchar": return SqlDbType.NChar;
        case "ntext": return SqlDbType.NText;
        case "nvarchar": return SqlDbType.NVarChar;
        case "real": return SqlDbType.Real;
        case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
        case "smalldatetime": return SqlDbType.SmallDateTime;
        case "smallint": return SqlDbType.SmallInt;
        case "smallmoney": return SqlDbType.SmallMoney;
        case "text": return SqlDbType.Text;
        case "timestamp": return SqlDbType.Timestamp;
        case "tinyint": return SqlDbType.TinyInt;
        case "varbinary": return SqlDbType.VarBinary;
        case "variant":
        case "numeric":
          return SqlDbType.Variant;
        case "xml": return SqlDbType.Xml;
        case "udt": return SqlDbType.Udt;
        case "structured": return SqlDbType.Structured;
        case "date": return SqlDbType.Date;
        case "time": return SqlDbType.Time;
        case "datetime2": return SqlDbType.DateTime2;
        case "datetimeoffset": return SqlDbType.DateTimeOffset;
        case "varchar":
        default:
          return SqlDbType.VarChar;
      }
    }
  }
}
