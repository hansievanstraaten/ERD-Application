/* PSEUDOCODE / PLAN
1. Purpose: Replace MS SQL Server specific queries with equivalent PostgreSQL queries.
2. Maintain existing public API (ISQLQueries) and method signatures.
3. Keep BuildSelectTop unchanged (already Postgres-friendly using LIMIT).
4. Replace MS SQL Server queries that reference [sys], square-bracket identifiers, or SQL Server-specific functions with PostgreSQL-compatible queries using:
   - information_schema.columns
   - information_schema.table_constraints
   - information_schema.key_column_usage
   - information_schema.constraint_column_usage
   - basic information_schema.tables / pg_catalog when necessary
5. For methods that return primary key / constraint names:
   - Use information_schema.table_constraints filtered by constraint_type = 'PRIMARY KEY'
6. For FK / PK and column metadata:
   - Use information_schema joins (key_column_usage, constraint_column_usage, table_constraints) to obtain:
     table_name, max_length, numeric_precision, numeric_scale, is_nullable, is_identity (detect via column_default LIKE 'nextval(%' or is_identity = 'YES'), constraint names, referenced table/column, data_type
7. Implement BuildInString to safely produce SQL IN list like: 'a','b','c' with simple SQL-escaping for single quotes.
8. Keep StringBuilder and minimal string.Format usage. Ensure all string literals are valid for Postgres and order by schema.table and ordinal_position where appropriate.
9. Output the modified C# file preserving namespace and class structure.

Now emit the updated C# file implementing the above plan.
*/

using ERD.Models;
using ERD.Models.ModelExstentions;
using System.Data.SqlTypes;
using System.Text;

namespace ERD.DatabaseScripts.Postgres
{
    internal class PgQueries : ISQLQueries
    {
        public string BuildSelectTop(TableModel table, int topValue = 100)
        {
            if (table.Columns.Length == 0)
                return string.Empty;

            StringBuilder result = new StringBuilder();

            result.Append("SELECT ");

            for (int x = 0; x < table.Columns.Length; x++)
            {
                if (x == 0)
                {
                    result.Append($"\"{table.Columns[x].ColumnName}\"");
                }
                else
                { 
                    result.Append($"            \"{table.Columns[x].ColumnName}\""); 
                }

                if (x < table.Columns.Length - 1)
                    result.AppendLine(", ");
            }

            result.AppendLine();
            result.AppendLine($"FROM {table.FullNamePostgreFormat()}");

            if (topValue > 0)
                result.AppendLine($"LIMIT {topValue}");

            return result.ToString();
        }

        public string DatabaseTablesQuery(string databaseName)
        {
            // Using information_schema; Postgres exposes table_catalog as database name.
            string query = "SELECT table_name AS TABLE_NAME, table_schema AS TABLE_SCHEMA " +
                           "FROM information_schema.tables " +
                           "WHERE table_type = 'BASE TABLE' " +
                           " AND table_schema NOT IN ('pg_catalog', 'information_schema')" +
                           " AND table_catalog = '{0}';";  

            return string.Format(query, databaseName);
        }

        public string DatabaseColumnUsageQuery(string columnName)
        {
            // Returns column usages across all tables, plus FK/PK and basic metadata.
            string query = "SELECT c.table_name AS TABLENAME," +
                           "  c.character_maximum_length AS MAX_LENGTH," +
                           "  c.numeric_precision AS PRECISION," +
                           "  c.numeric_scale AS SCALE," +
                           " (CASE WHEN c.is_nullable = 'NO' THEN FALSE ELSE TRUE END) AS IS_NULLABLE," +
                           " (CASE WHEN c.column_default LIKE 'nextval(%' OR c.is_identity = 'YES' THEN TRUE ELSE FALSE END) AS IS_IDENTITY," +
                           "  ccu.table_name AS PRIMARY_TABLE," +
                           "  ccu.column_name AS PRIMARY_COLUMNNAME," +
                           "  kcu.constraint_name AS FK_CONSTRAINT_NAME, " +
                           "  c.data_type AS DATA_TYPE " +
                           "FROM information_schema.columns c " +
                           "LEFT JOIN information_schema.key_column_usage kcu " +
                           "  ON kcu.table_schema = c.table_schema " +
                           "  AND kcu.table_name = c.table_name " +
                           "  AND kcu.column_name = c.column_name " +
                           "LEFT JOIN information_schema.constraint_column_usage ccu " +
                           "  ON ccu.constraint_name = kcu.constraint_name " +
                           "  AND ccu.constraint_schema = kcu.constraint_schema " +
                           "LEFT JOIN information_schema.table_constraints tc " +
                           "  ON tc.constraint_name = kcu.constraint_name " +
                           "  AND tc.table_schema = kcu.constraint_schema " +
                           "  AND tc.table_name = kcu.table_name " +
                           "WHERE c.column_name = '{0}' " +
                           "ORDER BY c.table_schema || '.' || c.table_name, c.ordinal_position;";

            return string.Format(query, columnName);
        }

        public string DatabaseTableColumnsQuery(string schema, string tableName)
        {
            string query = "SELECT tab.table_name AS \"TABLENAME\", " +
                           "     col.column_name AS \"COLUMNNAME\", " +
                           "     col.character_maximum_length AS \"MAX_LENGTH\", " +
                           "     col.numeric_precision AS \"PRECISION\", " +
                           "     col.numeric_scale AS \"SCALE\", " +
                           "     CASE WHEN col.is_nullable = 'YES' THEN TRUE ELSE FALSE END AS \"IS_NULLABLE\", " +
                           "     CASE WHEN pg_get_serial_sequence(quote_ident(tab.table_schema) || '.' || quote_ident(tab.table_name), col.column_name " +
                           "         ) IS NOT NULL THEN TRUE  ELSE FALSE " +
                           "     END AS \"IS_IDENTITY\", " +
                           "     pk_tab.table_name AS \"PRIMARY_TABLE\", " +
                           "     pk_col.column_name AS \"PRIMARY_COLUMNNAME\", " +
                           "     tc.constraint_name AS \"FK_CONSTRAINT_NAME\", " +
                           "     col.data_type AS \"DATA_TYPE\", " +
                           "     col.ordinal_position AS \"COLUMN_ID\", " +
                           "     kcu.ordinal_position AS \"ORDINAL_POSITION\" " +
                           " FROM information_schema.tables tab " +
                           "     INNER JOIN information_schema.columns col ON col.table_schema = tab.table_schema AND col.table_name = tab.table_name " +
                           "     LEFT JOIN information_schema.key_column_usage kcu ON kcu.table_schema = tab.table_schema AND kcu.table_name = tab.table_name " +
                           "         AND kcu.column_name = col.column_name " +
                           "     LEFT JOIN information_schema.table_constraints tc ON tc.constraint_schema = kcu.constraint_schema AND tc.constraint_name = kcu.constraint_name " +
                           "         AND tc.constraint_type = 'FOREIGN KEY' " +
                           "     LEFT JOIN information_schema.referential_constraints rc ON rc.constraint_schema = tc.constraint_schema AND rc.constraint_name = tc.constraint_name " +
                           "     LEFT JOIN information_schema.table_constraints pk_tc ON pk_tc.constraint_schema = rc.unique_constraint_schema AND pk_tc.constraint_name = rc.unique_constraint_name " +
                           "     LEFT JOIN information_schema.key_column_usage pk_col ON pk_col.constraint_schema = pk_tc.constraint_schema AND pk_col.constraint_name = pk_tc.constraint_name " +
                           "         AND pk_col.ordinal_position = kcu.position_in_unique_constraint " +
                           "     LEFT JOIN information_schema.tables pk_tab ON pk_tab.table_schema = pk_col.table_schema AND pk_tab.table_name = pk_col.table_name " +
                           " WHERE tab.table_type = 'BASE TABLE' " +
                           "     AND tab.table_name = '{0}' " +
                           "     AND tab.table_schema = '{1}' " +
                           " ORDER BY col.ordinal_position, kcu.ordinal_position";

            return string.Format(query, tableName, schema);
        }

        public string DatabaseInTableColumnsQuery(string[] tableNamesArray)
        {
            //string query = "SELECT c.table_name AS TABLENAME," +
            //               "  c.column_name AS COLUMNNAME," +
            //               "  c.character_maximum_length AS MAX_LENGTH," +
            //               "  c.numeric_precision AS PRECISION," +
            //               "  c.numeric_scale AS SCALE," +
            //               "  (CASE WHEN c.is_nullable = 'NO' THEN FALSE ELSE TRUE END) AS IS_NULLABLE," +
            //               "  (CASE WHEN c.column_default LIKE 'nextval(%' OR c.is_identity = 'YES' THEN TRUE ELSE FALSE END) AS IS_IDENTITY," +
            //               "  ccu.table_name AS PRIMARY_TABLE," +
            //               "  ccu.column_name AS PRIMARY_COLUMNNAME," +
            //               "  kcu.constraint_name AS FK_CONSTRAINT_NAME, " +
            //               "  c.data_type AS DATA_TYPE, " +
            //               "  c.ordinal_position AS COLUMN_ID, " +
            //               "  c.ordinal_position AS ORDINAL_POSITION " +
            //               "FROM information_schema.columns c " +
            //               "LEFT JOIN information_schema.key_column_usage kcu " +
            //               "  ON kcu.table_schema = c.table_schema " +
            //               "  AND kcu.table_name = c.table_name " +
            //               "  AND kcu.column_name = c.column_name " +
            //               "LEFT JOIN information_schema.constraint_column_usage ccu " +
            //               "  ON ccu.constraint_name = kcu.constraint_name " +
            //               "  AND ccu.constraint_schema = kcu.constraint_schema " +
            //               "LEFT JOIN information_schema.table_constraints tc " +
            //               "  ON tc.constraint_name = kcu.constraint_name " +
            //               "  AND tc.table_schema = kcu.constraint_schema " +
            //               "  AND tc.table_name = kcu.table_name " +
            //               "WHERE c.table_name IN ({0}) " +
            //               "ORDER BY c.table_schema || '.' || c.table_name, c.ordinal_position;";

            string query = "SELECT tab.table_name AS \"TABLENAME\", " +
                           "     col.column_name AS \"COLUMNNAME\", " +
                           "     col.character_maximum_length AS \"MAX_LENGTH\", " +
                           "     col.numeric_precision AS \"PRECISION\", " +
                           "     col.numeric_scale AS \"SCALE\", " +
                           "     CASE WHEN col.is_nullable = 'YES' THEN TRUE ELSE FALSE END AS \"IS_NULLABLE\", " +
                           "     CASE WHEN pg_get_serial_sequence(quote_ident(tab.table_schema) || '.' || quote_ident(tab.table_name), col.column_name " +
                           "         ) IS NOT NULL THEN TRUE  ELSE FALSE " +
                           "     END AS \"IS_IDENTITY\", " +
                           "     pk_tab.table_name AS \"PRIMARY_TABLE\", " +
                           "     pk_col.column_name AS \"PRIMARY_COLUMNNAME\", " +
                           "     tc.constraint_name AS \"FK_CONSTRAINT_NAME\", " +
                           "     col.data_type AS \"DATA_TYPE\", " +
                           "     col.ordinal_position AS \"COLUMN_ID\", " +
                           "     kcu.ordinal_position AS \"ORDINAL_POSITION\" " +
                           " FROM information_schema.tables tab " +
                           "     INNER JOIN information_schema.columns col ON col.table_schema = tab.table_schema AND col.table_name = tab.table_name " +
                           "     LEFT JOIN information_schema.key_column_usage kcu ON kcu.table_schema = tab.table_schema AND kcu.table_name = tab.table_name " +
                           "         AND kcu.column_name = col.column_name " +
                           "     LEFT JOIN information_schema.table_constraints tc ON tc.constraint_schema = kcu.constraint_schema AND tc.constraint_name = kcu.constraint_name " +
                           "         AND tc.constraint_type = 'FOREIGN KEY' " +
                           "     LEFT JOIN information_schema.referential_constraints rc ON rc.constraint_schema = tc.constraint_schema AND rc.constraint_name = tc.constraint_name " +
                           "     LEFT JOIN information_schema.table_constraints pk_tc ON pk_tc.constraint_schema = rc.unique_constraint_schema AND pk_tc.constraint_name = rc.unique_constraint_name " +
                           "     LEFT JOIN information_schema.key_column_usage pk_col ON pk_col.constraint_schema = pk_tc.constraint_schema AND pk_col.constraint_name = pk_tc.constraint_name " +
                           "         AND pk_col.ordinal_position = kcu.position_in_unique_constraint " +
                           "     LEFT JOIN information_schema.tables pk_tab ON pk_tab.table_schema = pk_col.table_schema AND pk_tab.table_name = pk_col.table_name " +
                           " WHERE tab.table_type = 'BASE TABLE' " +
                           "     AND tab.table_name IN ({0}) " +
                           " ORDER BY tab.table_schema || '.' || tab.table_name, col.ordinal_position, kcu.ordinal_position";

            return string.Format(query, this.BuildInString(tableNamesArray));
        }

        public string DatabaseColumnKeysQuery(string tableName, string columnName)
        {
            string query = "SELECT kcu.table_name AS TABLE_NAME," +
                           "  kcu.column_name AS COLUMN_NAME," +
                           "  tc.constraint_type AS CONSTRAINT_TYPE," +
                           "  kcu.ordinal_position AS COLUMN_ID, " +
                           "  kcu.position_in_unique_constraint AS ORDINAL_POSITION " +
                           "FROM information_schema.key_column_usage kcu " +
                           "LEFT JOIN information_schema.table_constraints tc " +
                           "  ON tc.constraint_name = kcu.constraint_name " +
                           "  AND tc.table_schema = kcu.table_schema " +
                           "  AND tc.table_name = kcu.table_name " +
                           "WHERE kcu.table_name = '{0}' AND kcu.column_name = '{1}';";

            return string.Format(query, tableName, columnName);
        }

        public string DatabaseInColumnKeysQuery(string tableName, string[] columnNamesArray)
        {
            string query = "SELECT kcu.table_name AS TABLE_NAME," +
                           "  kcu.column_name AS COLUMN_NAME," +
                           "  tc.constraint_type AS CONSTRAINT_TYPE," +
                           "  kcu.ordinal_position AS COLUMN_ID, " +
                           "  kcu.position_in_unique_constraint AS POSITION_IN_UNIQUE_CONSTRAINT " +
                           "FROM information_schema.key_column_usage kcu " +
                           "LEFT JOIN information_schema.table_constraints tc " +
                           "  ON tc.constraint_name = kcu.constraint_name " +
                           "  AND tc.table_schema = kcu.table_schema " +
                           "  AND tc.table_name = kcu.table_name " +
                           "WHERE kcu.table_name = '{0}' AND kcu.column_name IN ({1});";

            return string.Format(query, tableName, this.BuildInString(columnNamesArray));
        }

        public string DatabasePrimaryClusterName(string tableName)
        {
            // Return primary key constraint name for the given table (Postgres).
            return $"SELECT constraint_name AS NAME " +
                   $"FROM information_schema.table_constraints " +
                   $"WHERE table_name = '{tableName}' AND constraint_type = 'PRIMARY KEY';";
        }

        public string DatabasePrimaryClusterName()
        {
            // Return all primary key constraint names and their tables.
            return "SELECT constraint_name AS PK_NAME, table_name AS TABLE_NAME " +
                   "FROM information_schema.table_constraints " +
                   "WHERE constraint_type = 'PRIMARY KEY';";
        }

        private string BuildInString(string[] itemsArray)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < itemsArray.Length; i++)
            {
                if (i > 0)
                    result.Append(", ");

                // Simple SQL escape for single quotes
                string safe = itemsArray[i]?.Replace("'", "''") ?? string.Empty;
                result.Append("'").Append(safe).Append("'");
            }

            return result.ToString();
        }
    }
}