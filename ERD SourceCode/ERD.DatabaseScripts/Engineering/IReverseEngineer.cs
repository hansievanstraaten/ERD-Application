using ERD.Models;
using System.Collections.Generic;
using System.Windows.Threading;

namespace ERD.DatabaseScripts.Engineering
{
  public interface IReverseEngineer
  {
    List<TableModel> GetTables(Dispatcher dispatcher);

    List<ColumnObjectModel> GetTableColumns(string schema, string tableName);

    Dictionary<string, List<ColumnObjectModel>> GetInTableColumns(string[] tableNamesArray);

    string GetTablePrimaryKeyCluster(string tableName);
  }
}
