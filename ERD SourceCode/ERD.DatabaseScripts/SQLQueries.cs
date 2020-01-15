using ERD.Base;
using ERD.Common;

namespace ERD.DatabaseScripts
{
  public static class SQLQueries
  {
    public static ISQLQueries DatabaseQueries
    {
      get
      {
        ISQLQueries result;

        switch (Connections.Instance.DatabaseModel.DatabaseType)
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
