using ERD.Base;
using ERD.Common;
using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ERD.Common.ModelExstentions;

namespace ERD.DatabaseScripts.Postgres
{
    internal class PgScripting : IScripting
    {
        public DatabaseTypeEnum DatabaseScriptingType
        {
            get
            {
                return DatabaseTypeEnum.POSTGRES;
            }
        }

        private static string QuoteIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return "\"\"";
            // Double any existing quotes
            string safe = identifier.Replace("\"", "\"\"");
            return $"\"{safe}\"";
        }

        private static string TableNameQualified(string schema, string table)
        {
            if (string.IsNullOrWhiteSpace(schema))
                return QuoteIdentifier(table);

            string defaultName = Integrity.SchemaValidation(DatabaseTypeEnum.POSTGRES, schema);

            return $"{QuoteIdentifier(defaultName)}.{QuoteIdentifier(table)}";
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

            string tempTable = "temp_csv_data"; // use a generic temp name

            result.AppendLine("-- Auto-generated PostgreSQL upsert script");
            result.AppendLine($"BEGIN;");
            result.AppendLine();

            #region CREATE TEMP TABLE AND INSERT DATA

            result.AppendLine($"CREATE TEMP TABLE {QuoteIdentifier(tempTable)} (");

            for (int x = 0; x < tableColumnsCount; ++x)
            {
                ColumnObjectModel column = tableModel.Columns[x];

                string colType = this.ColumnDataType(column);
                string length = this.FieldLength(column);
                string nullStr = column.AllowNulls ? "" : " NOT NULL";

                string colDef = $"    {QuoteIdentifier(column.ColumnName)} {colType}{length}{nullStr}";

                if (x < tableColumnsCount - 1)
                {
                    result.AppendLine(colDef + ",");
                }
                else
                {
                    result.AppendLine(colDef);
                }
            }

            result.AppendLine(") ON COMMIT DROP;"); // temp table removed at end of transaction
            result.AppendLine();

            if (embedDataInSQL)
            {
                result.AppendLine($"INSERT INTO {QuoteIdentifier(tempTable)} (");

                for (int x = 0; x < tableColumnsCount; ++x)
                {
                    ColumnObjectModel column = tableModel.Columns[x];
                    if (x == tableColumnsCount - 1)
                        result.AppendLine($"    {QuoteIdentifier(column.ColumnName)}");
                    else
                        result.AppendLine($"    {QuoteIdentifier(column.ColumnName)},");
                }

                result.AppendLine(")");
                result.AppendLine("VALUES");

                string[] dataLines = File.ReadAllLines(csvFile);

                for (int l = 1; l < dataLines.Length; ++l)
                {
                    string line = dataLines[l];
                    string[] lineItems = line.Split(delimiter);

                    result.Append("    (");
                    for (int x = 0; x < tableColumnsCount; ++x)
                    {
                        string lineItem = lineItems[x].Replace("'", "''").Trim();

                        ColumnObjectModel column = tableModel.Columns[x];

                        if (column.AllowNulls && string.Equals(lineItem, "null", StringComparison.OrdinalIgnoreCase))
                        {
                            result.Append("NULL");
                        }
                        else
                        {
                            // Numeric/null handling kept simple: quote everything except clearly numeric DB types
                            if (IsNumericType(column.SqlDataType))
                            {
                                result.Append(string.IsNullOrEmpty(lineItem) ? "NULL" : lineItem);
                            }
                            else if (IsBooleanType(column.SqlDataType))
                            {
                                bool convertedValue = string.IsNullOrEmpty(lineItem) ? false : lineItem == "1";

                                result.Append(convertedValue);
                            }
                            else
                            {
                                result.Append($"'{lineItem}'");
                            }
                        }

                        if (x < tableColumnsCount - 1)
                            result.Append(", ");
                    }
                    result.AppendLine(l == dataLines.Length - 1 ? ")" : "),");
                }

                result.AppendLine(";");
            }
            else
            {
                // Use COPY to load CSV into temp table. Note: path accessibility depends on server permissions.
                // Use STDIN alternative if copying from client. Here we generate server-side COPY command.
                string escapedPath = csvFile.Replace("'", "''");
                result.AppendLine($"COPY {QuoteIdentifier(tempTable)} ({string.Join(", ", tableModel.Columns.Select(c => QuoteIdentifier(c.ColumnName)))})");
                result.AppendLine($"FROM '{escapedPath}' WITH (FORMAT csv, DELIMITER '{delimiter}', HEADER true);");
            }

            result.AppendLine();

            #endregion

            #region UPSERT (INSERT ... ON CONFLICT DO UPDATE)

            // Build column lists for insert/select and update
            var nonTimestampColumns = tableModel.Columns
                .Where(c => c.SqlDataType != SqlDbType.Timestamp && (mergeIdentityValues || !c.IsIdentity))
                .ToArray();

            string insertCols = string.Join(", ", nonTimestampColumns.Select(c => QuoteIdentifier(c.ColumnName)));
            string selectCols = string.Join(", ", nonTimestampColumns.Select(c => $"{QuoteIdentifier(c.ColumnName)}"));

            // Build conflict target
            string conflictCols = string.Join(", ", matchOnColumns.Select(c => QuoteIdentifier(c)));

            result.AppendLine($"-- UPSERT into target table");
            result.AppendLine($"INSERT INTO {tableModel.FullNamePostgreFormat()} ({insertCols})");
            result.AppendLine($"SELECT {selectCols} FROM {QuoteIdentifier(tempTable)}");
            result.AppendLine($"ON CONFLICT ({conflictCols})");

            if (mergeUpdate)
            {
                result.AppendLine("DO UPDATE SET");
                var updates = nonTimestampColumns
                    .Select(c => $"{QuoteIdentifier(c.ColumnName)} = EXCLUDED.{QuoteIdentifier(c.ColumnName)}")
                    .ToArray();
                result.AppendLine("    " + string.Join(",\n    ", updates));
                result.AppendLine(";");
            }
            else
            {
                result.AppendLine("DO NOTHING;");
            }

            result.AppendLine();

            #endregion

            #region DELETE MISSING

            if (mergeDelete)
            {
                // Delete rows present in target but not in source
                // Build join condition
                StringBuilder joinCondition = new StringBuilder();
                for (int i = 0; i < matchOnColumns.Length; ++i)
                {
                    string col = matchOnColumns[i];
                    if (i > 0) joinCondition.Append(" AND ");
                    joinCondition.Append($"target.{QuoteIdentifier(col)} = src.{QuoteIdentifier(col)}");
                }

                result.AppendLine($"-- DELETE rows not present in CSV source");
                result.AppendLine($"DELETE FROM {tableModel.FullNamePostgreFormat()} AS target");
                result.AppendLine($"USING {QuoteIdentifier(tempTable)} AS src");
                result.AppendLine($"WHERE NOT EXISTS (SELECT 1 FROM {QuoteIdentifier(tempTable)} WHERE {string.Join(" AND ", matchOnColumns.Select(c => $"src.{QuoteIdentifier(c)} = target.{QuoteIdentifier(c)}"))});");
                result.AppendLine();
            }

            #endregion

            result.AppendLine("COMMIT;");
            result.AppendLine();

            // Write SQL file
            string fileName = Path.GetFileNameWithoutExtension(csvFile);
            string filePath = Path.GetDirectoryName(csvFile);
            string sqlFileName = Path.Combine(filePath, $"{fileName}.sql");
            File.WriteAllText(sqlFileName, result.ToString());

            return sqlFileName;
        }

        private static bool IsNumericType(SqlDbType? t)
        {
            if (!t.HasValue) return false;
            switch (t.Value)
            {
                case SqlDbType.BigInt:
                case SqlDbType.Int:
                case SqlDbType.SmallInt:
                case SqlDbType.TinyInt:
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                case SqlDbType.Float:
                case SqlDbType.Real:
                    return true;
                case SqlDbType.Bit:
                default:
                    return false;
            }
        }

        private static bool IsBooleanType(SqlDbType? t)
        {
            if (!t.HasValue) return false;
            switch (t.Value)
            {
                case SqlDbType.Bit:
                    return true;
                default:
                    return false;
            }
        }

        public string ScriptTableCreate(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("-- PostgreSQL create table script");
            result.AppendLine();

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

            // Create table if not exists
            result.AppendLine($"CREATE TABLE IF NOT EXISTS {table.FullNamePostgreFormat()} (");

            // Columns - put identity columns first
            foreach (ColumnObjectModel column in table.Columns.OrderByDescending(p => p.IsIdentity).ThenByDescending(k => k.InPrimaryKey))
            {
                string colType = ColumnDataType(column);
                string length = FieldLength(column);
                string nullStr = column.AllowNulls ? "" : " NOT NULL";

                if (column.IsIdentity && (column.SqlDataType == SqlDbType.Int || column.SqlDataType == SqlDbType.BigInt || column.SqlDataType == SqlDbType.SmallInt))
                {
                    // Use identity
                    result.AppendLine($"    {QuoteIdentifier(column.ColumnName)} {colType} GENERATED ALWAYS AS IDENTITY{nullStr},");
                }
                else
                {
                    result.AppendLine($"    {QuoteIdentifier(column.ColumnName)} {colType}{length}{nullStr},");
                }
            }

            // Primary key
            int primaryKeyCount = table.Columns.Any(pk => pk.InPrimaryKey) ? table.Columns.Where(pkc => pkc.InPrimaryKey).Count() : 0;

            if (primaryKeyCount > 0)
            {
                var pkCols = table.Columns.Where(pkc => pkc.InPrimaryKey).Select(c => QuoteIdentifier(c.ColumnName));
                string pkName = string.IsNullOrEmpty(table.PrimaryKeyClusterConstraintName) ? $"{table.TableName}_pkey" : table.PrimaryKeyClusterConstraintName;
                result.AppendLine($"    CONSTRAINT {QuoteIdentifier(pkName)} PRIMARY KEY ({string.Join(", ", pkCols)})");
            }
            else
            {
                // remove trailing comma from last column
                result.Remove(result.Length - 3, 1);
            }

            result.AppendLine(");");
            result.AppendLine();

            // Add columns if not exists and other changes
            result.AppendLine(this.BuildColumnExists(table));

            // Foreign keys creation
            //result.AppendLine(this.BuildForeignKey(table));

            return result.ToString(); //.Replace("\"dbo\"", "\"public\"");
        }

        public string CreateSchema(string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName) || Integrity.DefaultSchemaNames.Contains(schemaName.ToLower()))
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            result.AppendLine($"CREATE SCHEMA IF NOT EXISTS {QuoteIdentifier(schemaName)};");
            
            return result.ToString();
        }

        public string DropTable(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine();
            result.AppendLine($"DROP TABLE IF EXISTS {table.FullNamePostgreFormat()} CASCADE;");
            result.AppendLine();

            return result.ToString();
        }

        public string DropColumn(string schema, string tableName, string columnName)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"ALTER TABLE {TableNameQualified(schema, tableName)} DROP COLUMN IF EXISTS {QuoteIdentifier(columnName)};");            
            result.AppendLine();

            return result.ToString();
        }

        public string DropForeignKey(string schema, string tableName, string constaintName)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine($"ALTER TABLE {TableNameQualified(schema, tableName)} DROP CONSTRAINT IF EXISTS {QuoteIdentifier(constaintName)};");
            result.AppendLine();

            return result.ToString();
        }

        public string BuildForeignKey(TableModel table)
        {
            StringBuilder result = new StringBuilder();

            Dictionary<string, ForeignKeyObjectModel[]> constraintGroups = table.Columns
                .Where(fk => fk.IsForeignkey)
                .SelectMany(fm => fm.ForeignKeys)
                .GroupBy(fkn => fkn.ForeignConstraintName)
                .ToDictionary(kd => kd.Key, kd => kd.ToArray());

            foreach (KeyValuePair<string, ForeignKeyObjectModel[]> kvp in constraintGroups)
            {
                string fkName = kvp.Key;
                ForeignKeyObjectModel[] cols = kvp.Value.Where(vr => vr.IsVertualRelation == false).ToArray();
                string[] distinctColumns = cols.Select(c => c.ForeignKeyColumn).ToArray();


                string childCols = string.Join(", ", distinctColumns.Select(c => QuoteIdentifier(c)));
                string parentTable = cols[0].ForeignKeyTable;
                string parentCols = string.Join(", ", distinctColumns.Select(c => QuoteIdentifier(c)));

                result.AppendLine("DO $$");
                result.AppendLine("BEGIN");
                result.AppendLine("    BEGIN");

                result.AppendLine("        IF NOT EXISTS (");
                result.AppendLine("            SELECT 1 FROM pg_constraint");
                result.AppendLine($"            WHERE conname = '{fkName}'");
                result.AppendLine("        ) THEN");

                result.AppendLine($"            ALTER TABLE {table.FullNamePostgreFormat()}");
                result.AppendLine($"            ADD CONSTRAINT {QuoteIdentifier(fkName)}");
                result.AppendLine($"            FOREIGN KEY ({childCols})");
                result.AppendLine($"            REFERENCES {TableNameQualified(table.SchemaName, parentTable)} ({parentCols});");

                result.AppendLine("        END IF;");

                result.AppendLine("    EXCEPTION");
                result.AppendLine("        WHEN others THEN");
                result.AppendLine($"            RAISE NOTICE 'Constraint {fkName} failed: %', SQLERRM;");
                result.AppendLine("    END;");
                result.AppendLine("END $$;");
                result.AppendLine();
            }

            return result.ToString();
        }

        //public string BuildForeignKey(TableModel table)
        //{
        //    StringBuilder result = new StringBuilder();

        //    // Attempt to create foreign key constraints for columns flagged as foreign keys
        //    // Use safe existence check via pg_constraint
        //    var constraintGroups = table.Columns
        //        .Where(fk => fk.IsForeignkey && !fk.IsVertualRelation)
        //        .GroupBy(fk => fk.ForeignConstraintName)
        //        .ToDictionary(g => g.Key, g => g.ToArray());

        //    foreach (var kvp in constraintGroups)
        //    {
        //        string fkName = kvp.Key;
        //        ColumnObjectModel[] cols = kvp.Value;

        //        // Child columns
        //        var childCols = string.Join(", ", cols.Select(c => QuoteIdentifier(c.ColumnName)));
        //        // Parent table and columns (assume same parent table for group)
        //        string parentTable = cols[0].ForeignKeyTable;
        //        string parentCols = string.Join(", ", cols.Select(c => QuoteIdentifier(c.ForeignKeyColumn)));


        //        result.AppendLine("IF NOT EXISTS(SELECT 1");
        //        result.AppendLine("    FROM pg_constraint");
        //        result.AppendLine($"    WHERE conname = '{fkName}'");
        //        result.AppendLine(") THEN");
        //        result.AppendLine($"    ALTER TABLE {table.FullNamePostgreFormat()}");
        //        result.AppendLine($"    ADD CONSTRAINT \"{fkName}\"");
        //        result.AppendLine("    FOREIGN KEY(\"WorkflowTypeID\")");
        //        result.AppendLine("    REFERENCES \"public\".\"SystemWorkflowType\"(\"WorkflowTypeID\");");
        //        result.AppendLine("  END IF;");


        //        //result.AppendLine($"IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = '{fkName}') THEN");
        //        //result.AppendLine($"    ALTER TABLE {table.FullNamePostgreFormat()}");
        //        //result.AppendLine($"    ADD CONSTRAINT {QuoteIdentifier(fkName)} FOREIGN KEY ({childCols}) REFERENCES {TableNameQualified(table.SchemaName, parentTable)} ({parentCols});");
        //        //result.AppendLine($"END IF;");
        //        //result.AppendLine();
        //    }

        //    return result.ToString();
        //}

        public string BuildeColumnCreate(string schemaName, string tableName, ColumnObjectModel column)
        {
            StringBuilder result = new StringBuilder();

            Tuple<object, object> originalName = column["ColumnName"];

            if (originalName != null)
            {
                // Rename column if original name present
                string oldName = originalName.Item1?.ToString();
                string newName = originalName.Item2?.ToString();

                if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
                {
                    result.AppendLine($"IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = '{schemaName}' AND table_name = '{tableName}' AND column_name = '{oldName}') AND NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = '{schemaName}' AND table_name = '{tableName}' AND column_name = '{newName}') THEN");
                    result.AppendLine($"    ALTER TABLE {TableNameQualified(schemaName, tableName)} RENAME COLUMN {QuoteIdentifier(oldName)} TO {QuoteIdentifier(newName)};");
                    result.AppendLine($"END IF;");
                    result.AppendLine();
                }
            }

            if (column.SqlDataType == SqlDbType.Timestamp)
            {
                return result.ToString();
            }

            result.AppendLine($"ALTER TABLE {TableNameQualified(schemaName, tableName)} ADD COLUMN IF NOT EXISTS {QuoteIdentifier(column.ColumnName)} {this.ColumnDataType(column)}{this.FieldLength(column)} {(column.AllowNulls ? "" : "NOT NULL")};");
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

            // Alter column type and nullability
            result.AppendLine($"ALTER TABLE {tableName} ALTER COLUMN {QuoteIdentifier(column.ColumnName)} TYPE {this.ColumnDataType(column)}{this.FieldLength(column)} USING {QuoteIdentifier(column.ColumnName)}:: {this.ColumnDataType(column)};");
            if (column.AllowNulls)
            {
                result.AppendLine($"ALTER TABLE {tableName} ALTER COLUMN {QuoteIdentifier(column.ColumnName)} DROP NOT NULL;");
            }
            else
            {
                result.AppendLine($"ALTER TABLE {tableName} ALTER COLUMN {QuoteIdentifier(column.ColumnName)} SET NOT NULL;");
            }

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
                        return "text";
                    }
                    goto default;

                case SqlDbType.Variant:
                    return "jsonb";

                default:
                    return $"{this.ColumnDataType(column)}";
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
                case SqlDbType.VarBinary:
                    return string.Empty; // bytea doesn't need length
                case SqlDbType.Char:
                case SqlDbType.NChar:
                    return $"({column.MaxLength})";
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                    if (column.MaxLength <= 0) return string.Empty; // map to text
                    return $"({column.MaxLength})";
                case SqlDbType.Decimal:
                case SqlDbType.Variant:
                    return $"({column.Precision}, {column.Scale})";
                case SqlDbType.DateTimeOffset:
                case SqlDbType.DateTime2:
                    return string.Empty;
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
                    result.Append(this.BuildColumnAlter(table.FullNamePostgreFormat(), column));
                }
            }

            return result.ToString();
        }

        //private string BuildPrimaryKeys(TableModel table, int keyCount, out bool haveChanges)
        //{
        //    // In PostgreSQL primary keys are declared inline in CREATE TABLE or via ALTER TABLE ADD CONSTRAINT
        //    haveChanges = false;
        //    StringBuilder result = new StringBuilder();

        //    var pkCols = table.Columns.Where(pkc => pkc.InPrimaryKey).Select(c => QuoteIdentifier(c.ColumnName)).ToArray();

        //    if (pkCols.Length == 0)
        //    {
        //        return string.Empty;
        //    }

        //    string pkName = string.IsNullOrEmpty(table.PrimaryKeyClusterConstraintName) ? $"{table.TableName}_pkey" : table.PrimaryKeyClusterConstraintName;

        //    result.AppendLine($"ALTER TABLE {table.FullNamePostgreFormat()} ADD CONSTRAINT {QuoteIdentifier(pkName)} PRIMARY KEY ({string.Join(", ", pkCols)});");

        //    foreach (var c in table.Columns.Where(pk => pk.InPrimaryKey))
        //    {
        //        if (c.HasModelChanged)
        //        {
        //            haveChanges = true;
        //            break;
        //        }
        //    }

        //    return result.ToString();
        //}

        //private string BuildDropAndCreatePrimaryKeys(TableModel table, int keyCount)
        //{
        //    StringBuilder result = new StringBuilder();

        //    string pkName = string.IsNullOrEmpty(table.PrimaryKeyClusterConstraintName) ? $"{table.TableName}_pkey" : table.PrimaryKeyClusterConstraintName;

        //    result.AppendLine($"ALTER TABLE {table.FullNamePostgreFormat()} DROP CONSTRAINT IF EXISTS {QuoteIdentifier(pkName)};");
        //    result.AppendLine();

        //    var pkCols = table.Columns.Where(pkc => pkc.InPrimaryKey).Select(c => QuoteIdentifier(c.ColumnName)).ToArray();

        //    result.AppendLine($"ALTER TABLE {table.FullNamePostgreFormat()} ADD CONSTRAINT {QuoteIdentifier(pkName)} PRIMARY KEY ({string.Join(", ", pkCols)});");
        //    result.AppendLine();

        //    return result.ToString();
        //}

        //private string BuildePrimaryKeyClusterCheck(string constarintName, string tableName, string[] primaryKeyColumns)
        //{
        //    if (!primaryKeyColumns.HasElements())
        //    {
        //        return string.Empty;
        //    }

        //    StringBuilder result = new StringBuilder();

        //    string pkCols = string.Join(", ", primaryKeyColumns.Select(c => QuoteIdentifier(c)));
        //    // Check and add primary key if not exists
        //    result.AppendLine($"DO $$");
        //    result.AppendLine($"BEGIN");
        //    result.AppendLine($"    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = '{constarintName}') THEN");
        //    result.AppendLine($"        ALTER TABLE {tableName} ADD CONSTRAINT {QuoteIdentifier(constarintName)} PRIMARY KEY ({pkCols});");
        //    result.AppendLine($"    END IF;");
        //    result.AppendLine($"END$$;");
        //    result.AppendLine();

        //    return result.ToString();
        //}

        private string ColumnDataType(ColumnObjectModel column)
        {
            if (!column.SqlDataType.HasValue)
                return "text";

            switch (column.SqlDataType.Value)
            {
                case SqlDbType.BigInt:
                    return "bigint";
                case SqlDbType.Int:
                    return "integer";
                case SqlDbType.SmallInt:
                    return "smallint";
                case SqlDbType.TinyInt:
                    return "smallint";
                case SqlDbType.Bit:
                    return "boolean";
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return "numeric";
                case SqlDbType.Float:
                    return "double precision";
                case SqlDbType.Real:
                    return "real";
                case SqlDbType.VarBinary:
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                    return "bytea";
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                    if (column.MaxLength <= 0) return "text";
                    if (column.MaxLength == 8016) return "text";
                    return "varchar";
                case SqlDbType.Char:
                case SqlDbType.NChar:
                    return "char";
                case SqlDbType.Text:
                case SqlDbType.NText:
                    return "text";
                case SqlDbType.UniqueIdentifier:
                    return "uuid";
                case SqlDbType.DateTime:
                    return "timestamp";
                case SqlDbType.Date:
                    return "date";
                case SqlDbType.Time:
                    return "time";
                case SqlDbType.DateTime2:
                    return "timestamp";
                case SqlDbType.DateTimeOffset:
                    return "timestamptz";
                case SqlDbType.Xml:
                    return "xml";
                case SqlDbType.Variant:
                    return "jsonb";
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                    return "jsonb";
                default:
                    return column.SqlDataType.ToString().ToLowerInvariant();
            }
        }

        private string FieldLength(ColumnObjectModel column)
        {
            if (!column.SqlDataType.HasValue)
                return string.Empty;

            switch (column.SqlDataType.Value)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                    if (column.MaxLength <= 0) return string.Empty;
                    return $"({column.MaxLength})";
                case SqlDbType.Decimal:
                    return $"({column.Precision}, {column.Scale})";
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
