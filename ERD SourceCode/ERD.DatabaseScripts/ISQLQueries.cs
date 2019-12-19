namespace ERD.DatabaseScripts
{
  public interface ISQLQueries
  {
    string DatabaseTablesQuery(string databaseName);
    
    string DatabaseColumnUsageQuery(string columnName);

    string DatabaseTableColumnsQuery(string tableName);

    string DatabaseColumnKeysQuery(string tableName, string columnName);

    string DatabasePrimaryClusterName(string tableName);
  }
}
