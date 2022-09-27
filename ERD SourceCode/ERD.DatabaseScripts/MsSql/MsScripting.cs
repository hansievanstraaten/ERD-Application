using ERD.Base;
using ERD.Common;
using ERD.DatabaseScripts;
using ERD.Models;
using ERD.Models.ModelExstentions;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ERD.Viewer.Database.MsSql
{
    internal class MsScripting : IScripting
    {
        public DatabaseTypeEnum DatabaseScriptingType
        {
            get
            {
                return DatabaseTypeEnum.SQL;
            }
        }

        public string ScriptMerge(TableModel tableModel,
            string[] matchOnColumns, 
            string csvFile, 
            char delimiter, 
            bool mergeIdentityValues, 
            bool embedDataInSQL,
            bool mergeInser,
            bool mergeUpdate,
            bool mergeDelete)
        {
            if (!matchOnColumns.HasElements())
            {
                throw new ArgumentException("Match on Columns require atleast one column.");
            }

            int tableColumnsCount = tableModel.Columns.Length;

            StringBuilder result = new StringBuilder();

            result.AppendLine("DROP TABLE IF EXISTS #csvData");

            result.AppendLine();
            result.AppendLine("BEGIN TRANSACTION");

            result.AppendLine();
            result.AppendLine("BEGIN TRY");

            #region CREATE TEMP TABLE AND INSERT DATA

            result.AppendLine("     CREATE TABLE #csvData");
            result.AppendLine("	    (");

            for(int x = 0; x < tableColumnsCount; ++x)
            {
                ColumnObjectModel column = tableModel.Columns[x];

                if (x == (tableColumnsCount - 1))
                {
                    result.AppendLine($"           [{column.ColumnName}] [{this.ColumnDataType(column)}] {this.FieldLength(column)}"); // {this.NullString(column)}");
                }
                else
                {
                    result.AppendLine($"           [{column.ColumnName}] [{this.ColumnDataType(column)}] {this.FieldLength(column)},"); // {this.NullString(column)},");
                }
            }
            
            result.AppendLine("	    )");

            result.AppendLine();

            if (embedDataInSQL)
            {
                result.AppendLine("     INSERT INTO #csvData ");

                result.Append("     (");

                for (int x = 0; x < tableColumnsCount; ++x)
                {
                    ColumnObjectModel column = tableModel.Columns[x];

                    if (x == (tableColumnsCount - 1))
                    {
                        result.Append($"[{column.ColumnName}]");
                    }
                    else
                    {
                        result.Append($"[{column.ColumnName}], ");
                    }
                }

                result.AppendLine(")");

                result.AppendLine("     VALUES ");

                string[] dataLines = File.ReadAllLines(csvFile);

                for (int l = 1; l < dataLines.Length; ++l)
                {
                    string line = dataLines[l];

                    string[] lineItems = line.Split(delimiter, StringSplitOptions.None);

                    result.Append("     (");

                    for (int x = 0; x < tableColumnsCount; ++x)
                    {
                        string lineItem = lineItems[x].Replace("'", "''");

                        if (x == (tableColumnsCount - 1))
                        {
                            result.Append($"'{lineItem}'");
                        }
                        else
                        {
                            result.Append($"'{lineItem}', ");
                        }
                    }

                    if (l == (dataLines.Length - 1))
                    {
                        result.AppendLine(")");
                    }
                    else
                    {
                        result.AppendLine("),");
                    }
                }
            }
            else
            {
                result.AppendLine("     BULK INSERT #csvData");
                result.AppendLine($"     FROM '{csvFile}'");
                result.AppendLine("     WITH");
                result.AppendLine("     (");
                result.AppendLine("         FIRSTROW = 2,");
                result.AppendLine($"        FIELDTERMINATOR = '{delimiter}',");
                result.AppendLine("         ROWTERMINATOR = '\n'");
                result.AppendLine("     );");
            }
            
            result.AppendLine();

            #endregion

            #region MERGE AND USING

            result.AppendLine($"     MERGE [{tableModel.TableName}]  AS[TARGET]");
            result.AppendLine("     USING #csvData	AS [SOURCE]");
            result.AppendLine($"	    ON [TARGET].[{matchOnColumns[0]}] = [SOURCE].[{matchOnColumns[0]}]");

            for (int x = 1; x < matchOnColumns.Length; ++x)
            {
                result.AppendLine($"        AND[TARGET].[{matchOnColumns[x]}] = [SOURCE].[{matchOnColumns[x]}]");
            }

            #endregion

            #region WHEN MATCHED

            if (mergeUpdate)
            {
                result.AppendLine("     WHEN MATCHED");
                result.AppendLine("        THEN UPDATE SET");

                for (int x = 0; x < tableColumnsCount; ++x)
                {
                    ColumnObjectModel column = tableModel.Columns[x];

                    if (column.SqlDataType == SqlDbType.Timestamp ||
                        (!mergeIdentityValues && column.IsIdentity))
                    {
                        continue;
                    }

                    if (x == (tableColumnsCount - 1))
                    {
                        result.Append($"            [TARGET].[{column.ColumnName}] = [SOURCE].[{column.ColumnName}]");
                    }
                    else
                    {
                        result.AppendLine($"            [TARGET].[{column.ColumnName}] = [SOURCE].[{column.ColumnName}],");
                    }
                }
            }

            #endregion

            if (!mergeInser && !mergeDelete)
            {
                result.AppendLine(";");
            }
            else
            {
                result.AppendLine();
            }

            #region WHEN NOT MATCHED BY TARGET

            if (mergeInser)
            {
                result.AppendLine("    WHEN NOT MATCHED BY TARGET");
                result.Append("       THEN INSERT(");

                for (int x = 0; x < tableColumnsCount; ++x)
                {
                    ColumnObjectModel column = tableModel.Columns[x];

                    if (column.SqlDataType == SqlDbType.Timestamp ||
                        (!mergeIdentityValues && column.IsIdentity))
                    {
                        continue;
                    }

                    if (x == (tableColumnsCount - 1))
                    {
                        result.AppendLine($"[{column.ColumnName}])");
                    }
                    else
                    {
                        result.Append($"[{column.ColumnName}],");
                    }
                }

                result.Append("            VALUES(");

                for (int x = 0; x < tableColumnsCount; ++x)
                {
                    ColumnObjectModel column = tableModel.Columns[x];

                    if (column.SqlDataType == SqlDbType.Timestamp ||
                        (!mergeIdentityValues && column.IsIdentity))
                    {
                        continue;
                    }

                    if (x == (tableColumnsCount - 1))
                    {
                        result.Append($"[SOURCE].[{column.ColumnName}])");
                    }
                    else
                    {
                        result.Append($"[SOURCE].[{column.ColumnName}],");
                    }
                }
            }

            #endregion

            if (!mergeDelete)
            {
                result.AppendLine(";");
            }
            else
            {
                result.AppendLine();
            }

            #region WHEN NOT EXIST IN SOURCE

            if (mergeDelete)
            {
                result.AppendLine("     WHEN NOT MATCHED BY SOURCE");
                result.AppendLine("        THEN DELETE;");
            }

            #endregion

            result.AppendLine();

            result.AppendLine("     DROP TABLE IF EXISTS #csvData;");

            result.AppendLine("     COMMIT TRANSACTION;");
            result.AppendLine("END TRY");
            result.AppendLine("BEGIN CATCH");
            result.AppendLine("    ROLLBACK TRANSACTION;");
            result.AppendLine("    DROP TABLE IF EXISTS #csvData;");

            result.AppendLine("    THROW;");
            result.AppendLine("END CATCH");


            string fileName = Path.GetFileNameWithoutExtension(csvFile);

            string filePath = Path.GetDirectoryName(csvFile);

            string sqlFileName = Path.Combine(filePath, $"{fileName}.sql");

            File.WriteAllText(sqlFileName, result.ToString());
            
            return sqlFileName;
        }

        public string ScriptTableCreate(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("SET ANSI_NULLS ON");

            result.AppendLine();
            result.AppendLine("SET QUOTED_IDENTIFIER ON");

            # region DELETE OPTIONS

            if (table.IsDeleted)
            {
                result.Append(this.DropTable(table));

                return result.ToString();
            }

            foreach (ColumnObjectModel deletedColumn in table.DeltedColumns)
            {
                result.Append(this.DropColumn(table.SchemaName, table.TableName, deletedColumn.ColumnName));
            }

            table.DeltedColumns = new ColumnObjectModel[] { };

            #endregion

            result.AppendLine();
            result.AppendLine(this.CreateSchema(table.SchemaName));

            result.AppendLine();
            result.AppendLine("IF NOT EXISTS (SELECT 1");
            result.AppendLine("                 FROM INFORMATION_SCHEMA.TABLES");
            result.AppendLine($"                WHERE TABLE_NAME = '{table.TableName}')");

            result.AppendLine("BEGIN");

            result.AppendLine($"    CREATE TABLE {table.FullNameBraced()}");
            result.AppendLine("         ( ");

            foreach (ColumnObjectModel column in table.Columns.OrderByDescending(p => p.IsIdentity).ThenByDescending(k => k.InPrimaryKey))
            {
                if (column.IsIdentity)
                {
                    result.AppendLine($"           [{column.ColumnName}] [{column.SqlDataType}]  IDENTITY(1,1) NOT NULL,");
                }
                else
                {
                    result.AppendLine($"           [{column.ColumnName}] [{this.ColumnDataType(column)}] {this.FieldLength(column)} {this.NullString(column)},");
                }
            }

            int primaryKeyCount = table.Columns.Any(pk => pk.InPrimaryKey) ? table.Columns.Where(pkc => pkc.InPrimaryKey).Count() : 0;

            bool havePrimaryKeyChangs = false;

            if (primaryKeyCount > 0)
            {
                result.Append(this.BuildPrimaryKeys(table, primaryKeyCount, out havePrimaryKeyChangs));
            }
            else
            {
                result.Remove(result.Length - 1, 1);
                result.AppendLine("         ) ");
            }

            result.AppendLine("END");

            if (havePrimaryKeyChangs)
            {
                result.Append(this.BuildDropAndCreatePrimaryKeys(table, primaryKeyCount));
            }

            result.AppendLine();

            result.Append(this.BuildColumnExists(table));

            string[] primaryKeyColumns = table.Columns.Any(pk => pk.InPrimaryKey) ?
                table.Columns.Where(pkc => pkc.InPrimaryKey).Select(col => col.ColumnName).ToArray() :
                new string[] { };

            result.Append(this.BuildePrimaryKeyClusterCheck(table.PrimaryKeyClusterConstraintName, table.FullNameBraced(), primaryKeyColumns));

            return result.ToString();
        }
        
        public string CreateSchema(string schemaName)
		{
            if (schemaName.IsNullEmptyOrWhiteSpace() || schemaName.ToLower() == "dbo")
			{
                return string.Empty;
			}

            StringBuilder result = new StringBuilder();

            result.AppendLine("IF NOT EXISTS (SELECT  1");
            result.AppendLine("                 FROM INFORMATION_SCHEMA.SCHEMATA");
            result.AppendLine($"                 WHERE   SCHEMA_NAME = '{schemaName}')");
            result.AppendLine("BEGIN");
            result.AppendLine($"    EXECUTE('CREATE SCHEMA [{schemaName}] AUTHORIZATION [dbo]');");
            result.AppendLine("END");

            return result.ToString();
        }

        public string DropTable(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine();
            result.AppendLine("IF EXISTS (SELECT 1");
            result.AppendLine("                 FROM INFORMATION_SCHEMA.TABLES");
            result.AppendLine($"                WHERE TABLE_NAME = '{table.TableName}')");

            result.AppendLine("BEGIN");
            result.AppendLine($"    DROP TABLE {table.FullNameBraced()}");
            result.AppendLine("END");
            result.AppendLine();

            return result.ToString();
        }

        public string DropColumn(string schema, string tableName, string columnName)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("IF EXISTS (SELECT 1 ");
            result.AppendLine("                 FROM INFORMATION_SCHEMA.COLUMNS");
            result.AppendLine($"                WHERE TABLE_NAME  = '{tableName}' ");
            result.AppendLine($"                  AND COLUMN_NAME = '{columnName}')");

            result.AppendLine("BEGIN");
            result.AppendLine($"    ALTER TABLE [{schema}].[{tableName}]");
            result.AppendLine($"    DROP COLUMN [{columnName}];");
            result.AppendLine("END");
            result.AppendLine();

            return result.ToString();
        }

        public string DropForeignKey(string schema, string tableName, string constaintName)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("IF EXISTS(SELECT 1");
            result.AppendLine("          FROM SYS.FOREIGN_KEYS");
            result.AppendLine($"         WHERE OBJECT_ID = OBJECT_ID(N'{schema}{constaintName}')");
            result.AppendLine($"         AND PARENT_OBJECT_ID = OBJECT_ID(N'{schema}{tableName}'))");
            result.AppendLine("BEGIN");
            result.AppendLine($" ALTER TABLE[{tableName}] DROP CONSTRAINT[{constaintName}]");
            result.AppendLine("END");

            return result.ToString();
        }

        public string BuildForeignKey(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            DataAccess dataAccess = new DataAccess(Connections.Instance.DatabaseModel);

            Dictionary<string, ColumnObjectModel[]> constraintDic = table.Columns
                .Where(fk => fk.IsForeignkey && !fk.IsVertualRelation)
                .GroupBy(fkn => fkn.ForeignConstraintName)
                .ToDictionary(kd => kd.Key, kd => kd.ToArray());

            if (Connections.Instance.IsDefaultConnection)
            {
                foreach (KeyValuePair<string, ColumnObjectModel[]> foreignKey in constraintDic)
                {
                    string fkName = foreignKey.Key;

                    Dictionary<int, string> columnPosistions = new Dictionary<int, string>();

                    foreach (ColumnObjectModel column in foreignKey.Value)
                    {
                        string columnQuery = SQLQueries.DatabaseQueries.DatabaseColumnKeysQuery(column.ForeignKeyTable, column.ForeignKeyColumn);

                        XDocument columnResult = dataAccess.ExecuteQuery(columnQuery);

                        XElement rowItem = columnResult.Root.Elements().FirstOrDefault(r => r.Element("CONSTRAINT_TYPE").Value == "PRIMARY KEY");

                        if (rowItem == null)
                        {
                            continue;
                        }

                        column.OriginalPosition = rowItem.Element("ORDINAL_POSITION").Value.ToInt32();

                        columnPosistions.Add(column.OriginalPosition, column.ColumnName);
                    }

                    StringBuilder childColumns = new StringBuilder();

                    StringBuilder parentColumns = new StringBuilder();

                    if (columnPosistions.Count <= 0)
                    {
                        return string.Empty;
                    }

                    foreach (KeyValuePair<int, string> columnItem in columnPosistions.OrderBy(s => s.Key))
                    {
                        ColumnObjectModel column = foreignKey.Value.FirstOrDefault(c => c.ColumnName == columnItem.Value);

                        childColumns.Append($"[{column.ColumnName}], ");

                        parentColumns.Append($"[{column.ForeignKeyColumn}], ");
                    }

                    childColumns.Remove((childColumns.Length - 2), 2);

                    parentColumns.Remove((parentColumns.Length - 2), 2);

                    result.AppendLine();
                    result.AppendLine($"IF (OBJECT_ID('{table.SchemaNameBraced(true)}{foreignKey.Key}', 'F') IS NULL)");
                    result.AppendLine("BEGIN");
                    result.AppendLine($"    ALTER TABLE {table.FullNameBraced()}  WITH CHECK ADD  CONSTRAINT [{fkName}] FOREIGN KEY({childColumns.ToString()})");
                    result.AppendLine($"    REFERENCES {table.SchemaNameBraced(true)}[{foreignKey.Value[0].ForeignKeyTable}] ({parentColumns.ToString()})");
                    result.AppendLine();
                    result.AppendLine($"ALTER TABLE {table.FullNameBraced()} CHECK CONSTRAINT [{fkName}]");
                    result.AppendLine("END");
                }
            }

            return result.ToString();
        }

        public string BuildeColumnCreate(string schemaName, string tableName, ColumnObjectModel column)
        {
            StringBuilder result = new StringBuilder();

            Tuple<object, object> originalName = column["ColumnName"];

            if (originalName != null)
            {
                result.AppendLine("IF EXISTS (SELECT 1");
                result.AppendLine("             FROM INFORMATION_SCHEMA.COLUMNS");
                result.AppendLine($"            WHERE TABLE_NAME  = '{tableName}' ");
                result.AppendLine($"              AND COLUMN_NAME = '{originalName.Item1}')");
                result.AppendLine("BEGIN");
                result.AppendLine($"    EXEC sp_rename '[{schemaName}].[{tableName}].[{originalName.Item1}]', '{originalName.Item2}', 'COLUMN';");
                result.AppendLine("END");

                result.AppendLine();
            }

            if (column.SqlDataType == SqlDbType.Timestamp)
            {
                return result.ToString();
            }

            result.AppendLine("IF NOT EXISTS (SELECT 1 ");
            result.AppendLine("                 FROM INFORMATION_SCHEMA.COLUMNS");
            result.AppendLine($"                WHERE TABLE_NAME  = '{tableName}' ");
            result.AppendLine($"                  AND COLUMN_NAME = '{column.ColumnName}')");

            result.AppendLine("BEGIN");

            result.AppendLine($"    ALTER TABLE [{schemaName}].[{tableName}]");
            if (column.IsIdentity)
            {
                result.AppendLine($"    ADD [{column.ColumnName}] [{column.SqlDataType}]  IDENTITY(1,1) NOT NULL");
            }
            else
            {
                result.AppendLine($"    ADD [{column.ColumnName}] [{this.ColumnDataType(column)}] {this.FieldLength(column)} {this.NullString(column)}");
            }

            result.AppendLine("END");
            result.AppendLine();

            return result.ToString();
        }

        public string BuildColumnAlter(string tableName, ColumnObjectModel column)
        {
            StringBuilder result = new StringBuilder();

            if (column.SqlDataType == SqlDbType.Timestamp)
            {
                return string.Empty;
            }

            result.AppendLine("IF EXISTS (SELECT 1");
            result.AppendLine("             FROM INFORMATION_SCHEMA.COLUMNS");
            result.AppendLine($"            WHERE TABLE_NAME  = '{tableName}' ");
            result.AppendLine($"              AND COLUMN_NAME = '{column.ColumnName}')");
            result.AppendLine("BEGIN");
            result.AppendLine($"    ALTER TABLE {tableName}");
            result.AppendLine($"    ALTER COLUMN [{column.ColumnName}] [{this.ColumnDataType(column)}] {this.FieldLength(column)} {this.NullString(column)}");
            result.AppendLine("END");

            result.AppendLine();

            return result.ToString();
        }

        public string DatabaseDataType(ColumnObjectModel column)
        {
            if (column == null)
            {
                return string.Empty;
            }

            switch (column.SqlDataType)
            {
                case SqlDbType.VarChar:
                    if (column.MaxLength == 8016)
                    {
                        return "sql_variant";
                    }

                    goto default;

                case SqlDbType.Variant:
                    return "numeric";

                default:
                    return $"{column.SqlDataType}";
            }
        }

        public string DatafieldLength(ColumnObjectModel column)
        {
            if (column == null)
            {
                return string.Empty;
            }

            switch (column.SqlDataType)
            {
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Time:
                    return $"({column.MaxLength})";

                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                    return $"({column.Scale})";

                case SqlDbType.VarChar:

                    if (column.MaxLength == 8016)
                    {
                        return string.Empty;
                    }

                    return $"({(column.MaxLength <= 0 ? "MAX" : column.MaxLength.ToString())})";

                case SqlDbType.VarBinary:
                case SqlDbType.NVarChar:
                    return $"({(column.MaxLength <= 0 ? "MAX" : column.MaxLength.ToString())})";


                case SqlDbType.Decimal:
                case SqlDbType.Variant:
                    return $"({column.Precision}, {column.Scale})";

                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.DateTime:
                case SqlDbType.Float:
                case SqlDbType.Image:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.NText:
                case SqlDbType.Real:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.SmallDateTime:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Text:
                case SqlDbType.Timestamp:
                case SqlDbType.TinyInt:
                case SqlDbType.Xml:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.Date:
                default:
                    return string.Empty;
            }
        }

        private string BuildColumnExists(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            foreach (ColumnObjectModel column in table.Columns)
            {
                result.AppendLine(this.BuildeColumnCreate(table.SchemaName, table.TableName, column));
            }

            result.AppendLine(this.BuildColumnChanges(table));

            return result.ToString();
        }

        private string BuildColumnChanges(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            foreach (ColumnObjectModel column in table.Columns)
            {
                if (!column.HasModelChanged)
                {
                    continue;
                }

                Tuple<object, object> originalDataType = column["SqlDataType"];

                Tuple<object, object> originalNullable = column["AllowNulls"];

                Tuple<object, object> originalMaxLen = column["MaxLength"];

                Tuple<object, object> originalPrecision = column["Precision"];

                Tuple<object, object> originalScale = column["Scale"];

                if (originalDataType != null || originalNullable != null || originalMaxLen != null || originalPrecision != null || originalScale != null)
                {
                    result.Append(this.BuildColumnAlter(table.FullNameBraced(), column));
                }
            }

            return result.ToString();
        }

        private string BuildPrimaryKeys(TableModel table, int keyCount, out bool haveChanges)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"    CONSTRAINT [{table.PrimaryKeyClusterConstraintName}] PRIMARY KEY CLUSTERED");

            if (keyCount == 1)
            {
                ColumnObjectModel column = table.Columns.First(pk => pk.InPrimaryKey);

                result.AppendLine("    (");
                result.AppendLine($"        [{column.ColumnName}] ASC");
                result.AppendLine("    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
                result.AppendLine("    ) ON [PRIMARY]");

                haveChanges = column.HasModelChanged;
            }
            else
            {
                haveChanges = false;

                result.AppendLine("    (");

                foreach (ColumnObjectModel column in table.Columns.Where(pkc => pkc.InPrimaryKey))
                {
                    result.AppendLine($"        [{column.ColumnName}] ASC,");

                    if (!haveChanges && column.HasModelChanged)
                    {
                        haveChanges = true;
                    }
                }

                result.Remove(result.Length - 3, 3); // Remove three to cater  for the line feed
                result.AppendLine();

                result.AppendLine("    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
                result.AppendLine("    ) ON [PRIMARY]");
            }

            return result.ToString();
        }

        private string BuildDropAndCreatePrimaryKeys(TableModel table, int keyCount)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("ELSE");
            result.AppendLine("BEGIN");

            List<string> scriptedForeignKeys = new List<string>();

            foreach (ColumnObjectModel pkColumn in table.Columns.Where(pk => pk.InPrimaryKey))
            {
                foreach (ColumnRelationMapModel columnRelation in Integrity.GetConstraintNames(table.TableName, pkColumn.ColumnName))
                {
                    if (scriptedForeignKeys.Contains(columnRelation.ForeignConstraintName))
                    {
                        continue;
                    }

                    result.AppendLine($"    IF (OBJECT_ID('{table.SchemaNameBraced(true)}[{columnRelation.ForeignConstraintName}]', 'F') IS NOT NULL)");
                    result.AppendLine("    BEGIN");
                    result.AppendLine($"        ALTER TABLE {table.SchemaNameBraced(true)}[{columnRelation.ChildTable}]  drop [{columnRelation.ForeignConstraintName}]");
                    result.AppendLine("    END");
                    result.AppendLine();

                    scriptedForeignKeys.Add(columnRelation.ForeignConstraintName);
                }
            }

            foreach (ColumnObjectModel fkColumn in table.Columns.Where(fk => fk.IsForeignkey))
            {
                if (scriptedForeignKeys.Contains(fkColumn.ForeignConstraintName))
                {
                    continue;
                }

                result.AppendLine($"    IF (OBJECT_ID('{table.SchemaNameBraced(true)}[{fkColumn.ForeignConstraintName}]', 'F') IS NOT NULL)");
                result.AppendLine("    BEGIN");
                result.AppendLine($"        ALTER TABLE {table.FullNameBraced()}  drop [{fkColumn.ForeignConstraintName}]");
                result.AppendLine("    END");
                result.AppendLine();

                scriptedForeignKeys.Add(fkColumn.ForeignConstraintName);
            }

            result.AppendLine("    IF EXISTS (SELECT 1 ");
            result.AppendLine("        FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS");
            result.AppendLine($"           WHERE CONSTRAINT_NAME = '{table.PrimaryKeyClusterConstraintName}')");
            result.AppendLine("            BEGIN");
            result.AppendLine($"               ALTER TABLE {table.FullNameBraced()} DROP CONSTRAINT {table.PrimaryKeyClusterConstraintName}");
            result.AppendLine();
            result.AppendLine($"               ALTER TABLE {table.FullNameBraced()}");

            if (keyCount == 1)
            {
                ColumnObjectModel column = table.Columns.First(pkc => pkc.InPrimaryKey);

                result.AppendLine($"               ADD CONSTRAINT {table.PrimaryKeyClusterConstraintName} PRIMARY KEY CLUSTERED ([{column.ColumnName}])");
            }
            else
            {
                result.AppendLine($"               ADD CONSTRAINT [{table.PrimaryKeyClusterConstraintName}] PRIMARY KEY CLUSTERED");
                result.AppendLine("               (");

                foreach (ColumnObjectModel column in table.Columns.Where(pkc => pkc.InPrimaryKey))
                {
                    result.AppendLine($"                   [{column.ColumnName}] ASC,");
                }

                result.Remove(result.Length - 3, 3); // Remove three to cater  for the line feed
                result.AppendLine();

                result.AppendLine("               )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            }

            result.AppendLine("            END");

            result.Append("END");

            return result.ToString();
        }

        private string BuildePrimaryKeyClusterCheck(string constarintName, string tableName, string[] primaryKeyColumns)
        {
            if (!primaryKeyColumns.HasElements())
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            StringBuilder keyColumns = new StringBuilder();

            foreach (string column in primaryKeyColumns)
            {
                keyColumns.Append($"[{column}],");
            }

            keyColumns.Remove(keyColumns.Length - 1, 1);

            result.AppendLine("IF NOT EXISTS(SELECT 1");
            result.AppendLine("                FROM SYS.INDEXES");
            result.AppendLine("               WHERE IS_PRIMARY_KEY = 1");
            result.AppendLine($"               AND[NAME] = '{constarintName}')");
            result.AppendLine("BEGIN");
            result.AppendLine($"    ALTER TABLE{tableName}");
            result.AppendLine($"    ADD CONSTRAINT {constarintName} PRIMARY KEY CLUSTERED({keyColumns.ToString()});");
            result.AppendLine("END");


            return result.ToString();
        }

        private string ColumnDataType(ColumnObjectModel column)
        {
            switch (column.SqlDataType)
            {
                case SqlDbType.VarChar:
                    if (column.MaxLength == 8016)
                    {
                        return "sql_variant";
                    }

                    return column.SqlDataType.ToString();

                case SqlDbType.Variant:
                    return "numeric";

                default: return column.SqlDataType.ToString();
            }
        }

        private string FieldLength(ColumnObjectModel column)
        {
            switch (column.SqlDataType)
            {
                case SqlDbType.Binary:
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.Time:
                    return $"({column.MaxLength})";

                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                    return $"({column.Scale})";

                case SqlDbType.VarChar:

                    if (column.MaxLength == 8016)
                    {
                        return string.Empty;
                    }

                    return $"({(column.MaxLength <= 0 ? "MAX" : column.MaxLength.ToString())})";

                case SqlDbType.VarBinary:
                case SqlDbType.NVarChar:
                    return $"({(column.MaxLength <= 0 ? "MAX" : column.MaxLength.ToString())})";


                case SqlDbType.Decimal:
                case SqlDbType.Variant:
                    return $"({column.Precision}, {column.Scale})";

                case SqlDbType.BigInt:
                case SqlDbType.Bit:
                case SqlDbType.DateTime:
                case SqlDbType.Float:
                case SqlDbType.Image:
                case SqlDbType.Int:
                case SqlDbType.Money:
                case SqlDbType.NText:
                case SqlDbType.Real:
                case SqlDbType.UniqueIdentifier:
                case SqlDbType.SmallDateTime:
                case SqlDbType.SmallInt:
                case SqlDbType.SmallMoney:
                case SqlDbType.Text:
                case SqlDbType.Timestamp:
                case SqlDbType.TinyInt:
                case SqlDbType.Xml:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.Date:
                default:
                    return string.Empty;
            }
        }

        private string NullString(ColumnObjectModel column)
        {
            if (column.AllowNulls)
            {
                return "NULL";
            }

            return "NOT NULL";
        }

    }
}
