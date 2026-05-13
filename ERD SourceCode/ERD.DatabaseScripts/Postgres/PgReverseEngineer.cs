// PSEUDOCODE / PLAN:
// 1. Accept that the surrounding class remains unchanged; only the SQL Server-specific type parsing must be adapted to Postgres types.
// 2. Implement a robust ParseSqlDbType method that:
//    - Normalizes the incoming type string (lowercase, trimmed).
//    - Handles common PostgreSQL type names and their synonyms (e.g., "character varying", "varchar", "text").
//    - Maps Postgres types to the closest System.Data.SqlDbType values used by the rest of the application.
//    - Uses prefix checks (StartsWith) for types that can include modifiers like precision/length.
//    - Falls back to SqlDbType.VarChar for unknown/empty types.
// 3. Replace the existing ParseSqlDbType implementation with the Postgres-aware one.
// 4. Keep all other class behavior intact to avoid affecting external callers.

using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DatabaseScripts.Engineering;
using ERD.DatabaseScripts.MsSql;
using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Principal;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Automation.Peers;
using System.Windows.Threading;
using System.Xml.Linq;
using ViSo.Dialogs.ModelViewer;
using WPF.Tools.Functions;


namespace ERD.DatabaseScripts.Postgres
{
    internal class PgReverseEngineer : IReverseEngineer
    {
        private Dispatcher dispatcher;

        internal PgReverseEngineer(Dispatcher senderDispatcher)
        {
            this.dispatcher = senderDispatcher;
        }

        public string GetTablePrimaryKeyCluster(string tableName)
        {
            DataAccess dataAccess = new DataAccess(Connections.Instance.DatabaseModel);

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

            DataAccess dataAccess = null;

            dispatcher.Invoke(() =>
            {
            RECONNECT:

                dataAccess = new DataAccess(Connections.Instance.DatabaseModel);

                if (!dataAccess.TestConnection())
                {
                    if (ModelView.ShowDialog("Connection Failure", Connections.Instance.DatabaseModel).IsFalse())
                    {
                        throw new Exception("Connection failure. User opt out.");
                    }

                    goto RECONNECT;
                }
            });

            XDocument tablesXml = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseTablesQuery(Connections.Instance.DatabaseModel.DatabaseName));

            List<dynamic> clusterList = dataAccess.ExecuteQueryDynamic(SQLQueries.DatabaseQueries.DatabasePrimaryClusterName());

            foreach (XElement rowItem in tablesXml.Root.Elements())
            {
                string tableName = rowItem.Element("TABLE_NAME").Value;
                string tableSchema = rowItem.Element("TABLE_SCHEMA").Value;

                EventParser.ParseMessage(this, dispatcher, "Reading", tableName);

                if (tableName == "sysdiagrams")
                {
                    continue;
                }

                dynamic selectedCluster = clusterList.FirstOrDefault(d => ((IDictionary<string, object>)d).Values.ToArray()[1].ToString() == tableName);

                TableModel model = new TableModel { TableName = tableName, SchemaName = tableSchema };

                model.PrimaryKeyClusterConstraintName = selectedCluster != null ? ((IDictionary<string, object>)clusterList[0]).Values.FirstOrDefault().ToString() : tableName;

                result.Add(model);
            }

            return result;
        }

        public List<ColumnObjectModel> GetTableColumns(string schema, string tableName)
        {
            EventParser.ParseMessage(this, this.dispatcher, "Reading Table ", tableName);

            List<ColumnObjectModel> result = new List<ColumnObjectModel>();

            DataAccess dataAccess = new DataAccess(Connections.Instance.DatabaseModel);

            XDocument columnsXml = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseTableColumnsQuery(schema, tableName));

            Dictionary<string, List<XElement>> columnGroups = columnsXml
                .Elements()
                .Elements()
                .GroupBy(x => (string)x.Element("COLUMNNAME") ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (KeyValuePair<string, List<XElement>> columnGroup in columnGroups)
            {
                if (result.Any(col => col.ColumnName == columnGroup.Key))
                {
                    continue;
                }

                ColumnObjectModel column = this.CreateColumn(tableName, columnGroup, ref dataAccess);

                result.Add(column);
            }

            return result;
        }

        public Dictionary<string, List<ColumnObjectModel>> GetInTableColumns(string[] tableNamesArray)
        {
            if (!tableNamesArray.HasElements())
            {
                return new Dictionary<string, List<ColumnObjectModel>>();
            }

            EventParser.ParseMessage(this, this.dispatcher, "Reading Tables", "");

            Dictionary<string, List<ColumnObjectModel>> result = new Dictionary<string, List<ColumnObjectModel>>();

            DataAccess dataAccess = new DataAccess(Connections.Instance.DatabaseModel);

            XDocument columnsXml = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseInTableColumnsQuery(tableNamesArray));

            Dictionary<string, List<XElement>> groupedTables = new Dictionary<string, List<XElement>>();

            foreach (XElement column in columnsXml.Root.Elements())
            {
                string tableName = column.Element("TABLENAME").Value;

                if (!groupedTables.ContainsKey(tableName))
                {
                    groupedTables.Add(tableName, new List<XElement>());
                }

                groupedTables[tableName].Add(column);
            }

            foreach (KeyValuePair<string, List<XElement>> tableColumns in groupedTables)
            {
                result.Add(tableColumns.Key, new List<ColumnObjectModel>());

                EventParser.ParseMessage(this, this.dispatcher, "Reading Columns for ", tableColumns.Key);

                string[] columnNamesArray = tableColumns.Value.Select(c => c.Element("COLUMNNAME").Value).ToArray();

                XDocument primaryKeys = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseInColumnKeysQuery(tableColumns.Key, columnNamesArray));

                Dictionary<string, List<XElement>> columnGroups = tableColumns.Value
                .GroupBy(x => (string)x.Element("COLUMNNAME") ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.ToList());

                foreach(KeyValuePair<string, List<XElement>> item in columnGroups)
                {
                    if (result[tableColumns.Key].Any(col => col.ColumnName == item.Key))
                    {
                        continue;
                    }

                    ColumnObjectModel column = this.CreateColumn(tableColumns.Key, item, ref dataAccess);

                    result[tableColumns.Key].Add(column);
                }
            }

            return result;
        }

        private ColumnObjectModel CreateColumn(string tableName, KeyValuePair<string, List<XElement>> columnGroupItem, ref DataAccess dataAccess)
        {
            XElement item = columnGroupItem.Value.FirstOrDefault(c =>
                    c.Element("TABLENAME").Value == tableName
                    && c.Element("COLUMNNAME").Value == columnGroupItem.Key);

            if (item == null)
            {
                throw new Exception("Someting very bad just happend, the primary column data not availabe");
            }

            EventParser.ParseMessage(this, this.dispatcher, "Reading Column ", columnGroupItem.Key);
                        
            string originalPosistion = Connections.Instance.IsDefaultConnection ? item.Element("ORDINAL_POSITION").Value : string.Empty;

            XDocument primaryKey = dataAccess.ExecuteQuery(SQLQueries.DatabaseQueries.DatabaseColumnKeysQuery(tableName, columnGroupItem.Key));

            ColumnObjectModel column = new ColumnObjectModel
            {
                ColumnName = columnGroupItem.Key,
                IsIdentity = item.Element("IS_IDENTITY").Value.ToBool(),
                AllowNulls = item.Element("IS_NULLABLE").Value.ToBool(),
                MaxLength = item.Element("MAX_LENGTH").Value.ToInt32(),
                Precision = item.Element("PRECISION").Value.ToInt32(),
                Scale = item.Element("SCALE").Value.ToInt32(),
                IsForeignkey = !item.Element("PRIMARY_TABLE").Value.IsNullEmptyOrWhiteSpace(),
                SqlDataType = this.ParseSqlDbType(item.Element("DATA_TYPE").Value),
                InPrimaryKey = primaryKey.Descendants().Any(d => d.Value == "PRIMARY KEY"),
                Column_Id = item.Element("COLUMN_ID").Value.ToInt32(),
                OriginalPosition = originalPosistion.IsNullEmptyOrWhiteSpace() ? 0 : originalPosistion.ToInt32()
            };

            List<XElement> constraints = columnGroupItem.Value
                .Where(fk => string.IsNullOrWhiteSpace(fk.Element("FK_CONSTRAINT_NAME").Value) == false)
                .ToList();

            foreach (XElement foreignKey in constraints)
            {
                ForeignKeyObjectModel fkModel = new ForeignKeyObjectModel
                {
                    LocalColumnName = columnGroupItem.Key,
                    OriginalPosition = originalPosistion.IsNullEmptyOrWhiteSpace() ? 0 : originalPosistion.ToInt32(),
                    ForeignKeyTable = foreignKey.Element("PRIMARY_TABLE").Value,
                    ForeignConstraintName = foreignKey.Element("FK_CONSTRAINT_NAME").Value,
                    ForeignKeyColumn = foreignKey.Element("PRIMARY_COLUMNNAME").Value
                };

                column.ForeignKeys.Add(fkModel);
            }

            column.HasModelChanged = false;

            return column;
        }

        private SqlDbType ParseSqlDbType(string value)
        {
            // Postgres-aware type mapping to System.Data.SqlDbType
            if (string.IsNullOrWhiteSpace(value))
            {
                return SqlDbType.VarChar;
            }

            string v = value.Trim().ToLowerInvariant();

            // Normalize common forms (e.g., "character varying", "timestamp with time zone")
            // Check for prefix forms first to handle size/precision suffixes like varchar(50), numeric(10,2)
            if (v.StartsWith("character varying") || v.StartsWith("varchar") || v.StartsWith("character") || v.StartsWith("char") || v == "text")
            {
                return SqlDbType.VarChar;
            }

            if (v == "boolean" || v == "bool")
            {
                return SqlDbType.Bit;
            }

            if (v.StartsWith("bytea"))
            {
                return SqlDbType.VarBinary;
            }

            if (v.StartsWith("bigint") || v.StartsWith("int8") || v.StartsWith("bigserial") || v.StartsWith("serial8"))
            {
                return SqlDbType.BigInt;
            }

            if (v.StartsWith("integer") || v == "int" || v.StartsWith("int4") || v.StartsWith("serial"))
            {
                return SqlDbType.Int;
            }

            if (v.StartsWith("smallint") || v.StartsWith("int2") || v.StartsWith("smallserial"))
            {
                return SqlDbType.SmallInt;
            }

            if (v.StartsWith("numeric") || v.StartsWith("decimal"))
            {
                return SqlDbType.Decimal;
            }

            if (v.StartsWith("real") || v.StartsWith("float4"))
            {
                return SqlDbType.Real;
            }

            if (v.StartsWith("double precision") || v.StartsWith("float8"))
            {
                // Map double precision to Float to represent 8-byte floating point
                return SqlDbType.Float;
            }

            if (v.StartsWith("money"))
            {
                return SqlDbType.Money;
            }

            if (v.StartsWith("date"))
            {
                return SqlDbType.Date;
            }

            // time without timezone
            if (v.StartsWith("time") && !v.Contains("zone") && !v.Contains("stamp"))
            {
                return SqlDbType.Time;
            }

            // timestamp without time zone
            if (v.StartsWith("timestamp") && !v.Contains("with time zone") && !v.Contains("timestamptz"))
            {
                return SqlDbType.DateTime;
            }

            // timestamp with time zone / timestamptz
            if (v.StartsWith("timestamptz") || v.Contains("with time zone") || v.StartsWith("timestamp with time zone"))
            {
                return SqlDbType.DateTimeOffset;
            }

            if (v.StartsWith("interval"))
            {
                return SqlDbType.VarChar;
            }

            if (v.StartsWith("uuid"))
            {
                return SqlDbType.UniqueIdentifier;
            }

            if (v.StartsWith("json") || v.StartsWith("jsonb"))
            {
                // JSON types map to text/varchar in the model
                return SqlDbType.VarChar;
            }

            if (v.StartsWith("xml"))
            {
                return SqlDbType.Xml;
            }

            if (v.StartsWith("bit"))
            {
                return SqlDbType.Bit;
            }

            if (v.StartsWith("varbit"))
            {
                return SqlDbType.VarBinary;
            }

            // Fallbacks for unknown or custom types
            switch (v)
            {
                case "inet":
                case "cidr":
                case "macaddr":
                    return SqlDbType.VarChar;
                default:
                    return SqlDbType.VarChar;
            }
        }
    }
}