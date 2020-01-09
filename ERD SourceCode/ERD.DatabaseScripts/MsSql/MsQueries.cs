using GeneralExtensions;
using System.Text;

namespace ERD.DatabaseScripts
{
  internal class MsQueries : ISQLQueries
  {
    public string DatabaseTablesQuery(string databaseName)
    {
      string query = "SELECT [TABLE_NAME] FROM [INFORMATION_SCHEMA].[TABLES] WHERE ([TABLE_TYPE] = 'BASE TABLE') AND ([TABLE_CATALOG] = '{0}')";

      return string.Format(query, databaseName);
    }

    public string DatabaseColumnUsageQuery(string columnName)
    {
      string query = "SELECT [TAB].[NAME] AS [TABLENAME]," +
                     "  [COL].[MAX_LENGTH]," +
                     "  [COL].[PRECISION]," +
                     "  [COL].[SCALE]," +
                     "  [COL].[IS_NULLABLE]," +
                     "  [COL].[IS_IDENTITY]," +
                     "  [PK_TAB].[NAME] AS [PRIMARY_TABLE]," +
                     "  [PK_COL].[NAME] AS [PRIMARY_COLUMNNAME]," +
                     "  [FK].[NAME] AS [FK_CONSTRAINT_NAME], " +
                     "  [INF_COL].[DATA_TYPE] " +
                     "FROM  [SYS].[TABLES] TAB " +
                     "      INNER JOIN [SYS].[COLUMNS] COL " +
                     "        ON  [COL].[OBJECT_ID]            = [TAB].[OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[FOREIGN_KEY_COLUMNS] FK_COLS" +
                     "        ON  [FK_COLS].[PARENT_OBJECT_ID] = [TAB].[OBJECT_ID]" +
                     "        AND [FK_COLS].[PARENT_COLUMN_ID] = [COL].[COLUMN_ID]" +
                     "      LEFT OUTER JOIN [SYS].[FOREIGN_KEYS] FK" +
                     "        ON  [FK].[OBJECT_ID]             = [FK_COLS].[CONSTRAINT_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[TABLES] PK_TAB" +
                     "        ON  [PK_TAB].[OBJECT_ID]         = [FK_COLS].[REFERENCED_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[COLUMNS] PK_COL" +
                     "        ON  [PK_COL].[COLUMN_ID]         = [FK_COLS].[REFERENCED_COLUMN_ID]" +
                     "        AND [PK_COL].[OBJECT_ID]         = [FK_COLS].[REFERENCED_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [INFORMATION_SCHEMA].[COLUMNS] AS INF_COL" +
                     "        ON  [INF_COL].[TABLE_NAME] = [TAB].[NAME]" +
                     "        AND [INF_COL].[COLUMN_NAME] = [COL].[NAME]" +
                     "WHERE [COL].[NAME] = '{0}'" +
                     "ORDER BY  SCHEMA_NAME([TAB].[SCHEMA_ID]) + '.' + [TAB].[NAME]," +
                     "          [COL].[COLUMN_ID]";

      return string.Format(query, columnName);
    }
    
    public string DatabaseTableColumnsQuery(string tableName)
    {
      string query = "SELECT [TAB].[NAME] AS [TABLENAME]," +
                     "  [COL].[NAME] AS [COLUMNNAME]," +
                     "  [COL].[MAX_LENGTH]," +
                     "  [COL].[PRECISION]," +
                     "  [COL].[SCALE]," +
                     "  [COL].[IS_NULLABLE]," +
                     "  [COL].[IS_IDENTITY]," +
                     "  [PK_TAB].[NAME] AS [PRIMARY_TABLE]," +
                     "  [PK_COL].[NAME] AS [PRIMARY_COLUMNNAME]," +
                     "  [FK].[NAME] AS [FK_CONSTRAINT_NAME], " +
                     "  [INF_COL].[DATA_TYPE], " +
                     "  [COL].[COLUMN_ID], " +
                     "  [INF_USE].[ORDINAL_POSITION] " +
                     "FROM  [SYS].[TABLES] TAB " +
                     "      INNER JOIN [SYS].[COLUMNS] COL " +
                     "        ON  [COL].[OBJECT_ID]            = [TAB].[OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[FOREIGN_KEY_COLUMNS] FK_COLS" +
                     "        ON  [FK_COLS].[PARENT_OBJECT_ID] = [TAB].[OBJECT_ID]" +
                     "        AND [FK_COLS].[PARENT_COLUMN_ID] = [COL].[COLUMN_ID]" +
                     "      LEFT OUTER JOIN [SYS].[FOREIGN_KEYS] FK" +
                     "        ON  [FK].[OBJECT_ID]             = [FK_COLS].[CONSTRAINT_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[TABLES] PK_TAB" +
                     "        ON  [PK_TAB].[OBJECT_ID]         = [FK_COLS].[REFERENCED_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[COLUMNS] PK_COL" +
                     "        ON  [PK_COL].[COLUMN_ID]         = [FK_COLS].[REFERENCED_COLUMN_ID]" +
                     "        AND [PK_COL].[OBJECT_ID]         = [FK_COLS].[REFERENCED_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [INFORMATION_SCHEMA].[COLUMNS] AS INF_COL" +
                     "        ON  [INF_COL].[TABLE_NAME] = [TAB].[NAME]" +
                     "        AND [INF_COL].[COLUMN_NAME] = [COL].[NAME]" +
                     "      LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE INF_USE" +
                     "        ON  [INF_USE].[TABLE_NAME]      = [TAB].[NAME]" +
                     "        AND [INF_USE].[COLUMN_NAME]     = [COL].[NAME]" +
                     "WHERE     [TAB].[NAME] = '{0}' " +
                     "ORDER BY  SCHEMA_NAME([TAB].[SCHEMA_ID]) + '.' + [TAB].[NAME]," +
                     "          [COL].[COLUMN_ID]";

      return string.Format(query, tableName);
    }

    public string DatabaseInTableColumnsQuery(string[] tableNamesArray)
    {
      string query = "SELECT [TAB].[NAME] AS [TABLENAME]," +
                     "  [COL].[NAME] AS [COLUMNNAME]," +
                     "  [COL].[MAX_LENGTH]," +
                     "  [COL].[PRECISION]," +
                     "  [COL].[SCALE]," +
                     "  [COL].[IS_NULLABLE]," +
                     "  [COL].[IS_IDENTITY]," +
                     "  [PK_TAB].[NAME] AS [PRIMARY_TABLE]," +
                     "  [PK_COL].[NAME] AS [PRIMARY_COLUMNNAME]," +
                     "  [FK].[NAME] AS [FK_CONSTRAINT_NAME], " +
                     "  [INF_COL].[DATA_TYPE], " +
                     "  [COL].[COLUMN_ID], " +
                     "  [INF_USE].[ORDINAL_POSITION] " +
                     "FROM  [SYS].[TABLES] TAB " +
                     "      INNER JOIN [SYS].[COLUMNS] COL " +
                     "        ON  [COL].[OBJECT_ID]            = [TAB].[OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[FOREIGN_KEY_COLUMNS] FK_COLS" +
                     "        ON  [FK_COLS].[PARENT_OBJECT_ID] = [TAB].[OBJECT_ID]" +
                     "        AND [FK_COLS].[PARENT_COLUMN_ID] = [COL].[COLUMN_ID]" +
                     "      LEFT OUTER JOIN [SYS].[FOREIGN_KEYS] FK" +
                     "        ON  [FK].[OBJECT_ID]             = [FK_COLS].[CONSTRAINT_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[TABLES] PK_TAB" +
                     "        ON  [PK_TAB].[OBJECT_ID]         = [FK_COLS].[REFERENCED_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [SYS].[COLUMNS] PK_COL" +
                     "        ON  [PK_COL].[COLUMN_ID]         = [FK_COLS].[REFERENCED_COLUMN_ID]" +
                     "        AND [PK_COL].[OBJECT_ID]         = [FK_COLS].[REFERENCED_OBJECT_ID]" +
                     "      LEFT OUTER JOIN [INFORMATION_SCHEMA].[COLUMNS] AS INF_COL" +
                     "        ON  [INF_COL].[TABLE_NAME] = [TAB].[NAME]" +
                     "        AND [INF_COL].[COLUMN_NAME] = [COL].[NAME]" +
                     "      LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE INF_USE" +
                     "        ON  [INF_USE].[TABLE_NAME]      = [TAB].[NAME]" +
                     "        AND [INF_USE].[COLUMN_NAME]     = [COL].[NAME]" +
                     "WHERE     [TAB].[NAME] IN ({0}) " +
                     "ORDER BY  SCHEMA_NAME([TAB].[SCHEMA_ID]) + '.' + [TAB].[NAME]," +
                     "          [COL].[COLUMN_ID]";

      return string.Format(query, this.BuildInString(tableNamesArray));
    }

    public string DatabaseColumnKeysQuery(string tableName, string columnName)
    {
      string query = "SELECT [TAB].[NAME] AS [TABLE_NAME]," +
                     "  [COL].[NAME] AS [COLUMN_NAME]," +
                     "  [TAB_CON].[CONSTRAINT_TYPE]," +
                     "  [COL].[COLUMN_ID], " +
                     "  [INF_USE].[ORDINAL_POSITION] " +
                     "FROM [SYS].TABLES TAB" +
                     "  INNER JOIN [SYS].COLUMNS COL " +
                     "    ON  [COL].[OBJECT_ID]           = [TAB].[OBJECT_ID]" +
                     "  LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE INF_USE" +
                     "    ON  [INF_USE].[TABLE_NAME]      = [TAB].[NAME]" +
                     "    AND [INF_USE].[COLUMN_NAME]     = [COL].[NAME]" +
                     "  LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TAB_CON" +
                     "    ON  [TAB_CON].[TABLE_NAME]      = [TAB].[NAME]" +
                     "    AND [TAB_CON].[CONSTRAINT_NAME] = [INF_USE].[CONSTRAINT_NAME]" +
                     "WHERE ([TAB].[NAME] = '{0}') AND ([COL].[NAME] = '{1}')";

      return string.Format(query, tableName, columnName);
    }

    public string DatabaseInColumnKeysQuery(string tableName, string[] columnNamesArray)
    {
      string query = "SELECT [TAB].[NAME] AS [TABLE_NAME]," +
                     "  [COL].[NAME] AS [COLUMN_NAME]," +
                     "  [TAB_CON].[CONSTRAINT_TYPE]," +
                     "  [COL].[COLUMN_ID], " +
                     "  [INF_USE].[ORDINAL_POSITION] " +
                     "FROM [SYS].TABLES TAB" +
                     "  INNER JOIN [SYS].COLUMNS COL " +
                     "    ON  [COL].[OBJECT_ID]           = [TAB].[OBJECT_ID]" +
                     "  LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE INF_USE" +
                     "    ON  [INF_USE].[TABLE_NAME]      = [TAB].[NAME]" +
                     "    AND [INF_USE].[COLUMN_NAME]     = [COL].[NAME]" +
                     "  LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TAB_CON" +
                     "    ON  [TAB_CON].[TABLE_NAME]      = [TAB].[NAME]" +
                     "    AND [TAB_CON].[CONSTRAINT_NAME] = [INF_USE].[CONSTRAINT_NAME]" +
                     "WHERE ([TAB].[NAME] = '{0}') AND ([COL].[NAME] IN ({1}))";

      return string.Format(query, tableName, this.BuildInString(columnNamesArray));
    }

    public string DatabasePrimaryClusterName(string tableName)
    {
      return "SELECT name " +
             "  FROM sys.indexes  " +
             $" WHERE object_id = OBJECT_ID('dbo.{tableName}')  " +
             "   AND index_id = 1 " +
             "   AND is_primary_key = 1";
    }
  
    private string BuildInString(string[] itemsArray)
    {
      StringBuilder result = new StringBuilder();

      result.Append("'");

      result.Append(itemsArray.Concatenate("', '"));

      result.Remove((result.Length - 3), 3);

      return result.ToString();
    }
  }
}
