using ERD.Models;

namespace ERD.DatabaseScripts
{
    public interface ISQLQueries
    {
        string BuildSelectTop(TableModel table, int topValue = 100);
        string DatabaseTablesQuery(string databaseName);

        string DatabaseColumnUsageQuery(string columnName);

        string DatabaseTableColumnsQuery(string schema, string tableName);

        string DatabaseInTableColumnsQuery(string[] tableNamesArray);

        string DatabaseColumnKeysQuery(string tableName, string columnName);

        string DatabaseInColumnKeysQuery(string tableName, string[] columnNamesArray);

        string DatabasePrimaryClusterName();

        string DatabasePrimaryClusterName(string tableName);
    }
}
