using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Viewer.Database
{
  internal interface ISQLQueries
  {
    string DatabaseTablesQuery(string databaseName);
    
    string DatabaseColumnUsageQuery(string columnName);

    string DatabaseTableColumnsQuery(string tableName);

    string DatabaseColumnKeysQuery(string tableName, string columnName);

    string DatabasePrimaryClusterName(string tableName);
  }
}
