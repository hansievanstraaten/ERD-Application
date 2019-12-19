using ERD.Viewer.Common;
using ERD.Viewer.Database.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Viewer.Database
{
  internal static class SQLQueries
  {
    internal static ISQLQueries DatabaseQueries
    {
      get
      {
        ISQLQueries result;

        switch (Connections.DatabaseModel.DatabaseType)
        {
          case DatabaseTypeEnum.SQL:
          default:
            result = new MsQueries();

            break;
        }

        return result;
      }
    }
  }
}
