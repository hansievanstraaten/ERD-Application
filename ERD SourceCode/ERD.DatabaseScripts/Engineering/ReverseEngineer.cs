using ERD.Base;
using ERD.Common;
using ERD.Models;
using ERD.Viewer.Database.MsSql;
using System.Collections.Generic;
using System.Windows.Threading;

namespace ERD.DatabaseScripts.Engineering
{
  public class ReverseEngineer
  {
    private Dispatcher dispatch;

    public ReverseEngineer(Dispatcher dispatcher)
    {
      this.dispatch = dispatcher;
    }

    /// <summary>
    /// Loads the tables from the database
    /// </summary>
    /// <param name="databaseModel">the DatabaseModel object to exeute the query against</param>
    /// <returns>returns a List<TableModel> (Columns not loaded)</returns>
    public List<TableModel> GetTables()
    {
      IReverseEngineer result = this.CreateClass(this.dispatch);

      return result.GetTables(this.dispatch);
    }

    public List<ColumnObjectModel> GetTableColumns(string tableName)
    {
      IReverseEngineer result = this.CreateClass(this.dispatch);

      return result.GetTableColumns(tableName);
    }

    public string GetTablePrimaryKeyCluster(string tableName)
    {
      IReverseEngineer result = this.CreateClass(this.dispatch);

      return result.GetTablePrimaryKeyCluster(tableName);
    }
    
    private IReverseEngineer CreateClass(Dispatcher dispatcher)
    {
      IReverseEngineer result;

      switch (Connections.DatabaseModel.DatabaseType)
      {
        case DatabaseTypeEnum.SQL:
        default:
          result = new MsReverseEngineer(this.dispatch);

          break;
      }

      return result;
    }
  }
}
