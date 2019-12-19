using System.Collections.Generic;
using System.Windows.Threading;

namespace ERD.Viewer.Database
{
  internal class ReverseEngineer
  {
    /// <summary>
    /// Loads the tables from the database
    /// </summary>
    /// <param name="databaseModel">the DatabaseModel object to exeute the query against</param>
    /// <returns>returns a List<TableModel> (Columns not loaded)</returns>
    public List<TableModel> GetTables(Dispatcher dispatcher)
    {
      IReverseEngineer result = this.CreateClass(dispatcher);

      return result.GetTables(dispatcher);
    }

    public List<ColumnObjectModel> GetTableColumns(string tableName, Dispatcher dispatcher)
    {
      IReverseEngineer result = this.CreateClass(dispatcher);

      return result.GetTableColumns(tableName);
    }

    public string GetTablePrimaryKeyCluster(string tableName, Dispatcher dispatcher)
    {
      IReverseEngineer result = this.CreateClass(dispatcher);

      return result.GetTablePrimaryKeyCluster(tableName);
    }

    private IReverseEngineer CreateClass(Dispatcher dispatcher)
    {
      IReverseEngineer result;

      switch (Connections.DatabaseModel.DatabaseType)
      {
        case DatabaseTypeEnum.SQL:
        default:
          result = new MsReverseEngineer(dispatcher);

          break;
      }

      return result;
    }
  }
}
