using ERD.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ERD.Viewer.Database
{
  internal interface IReverseEngineer
  {
    List<TableModel> GetTables(Dispatcher dispatcher);

    List<ColumnObjectModel> GetTableColumns(string tableName);

    string GetTablePrimaryKeyCluster(string tableName);
  }
}
