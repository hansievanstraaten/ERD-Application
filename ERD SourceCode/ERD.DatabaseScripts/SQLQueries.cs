using ERD.Base;
using ERD.Common;
using ERD.DatabaseScripts.Postgres;

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
                    case DatabaseTypeEnum.POSTGRES:
                        result = new PgQueries();

                        break;
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
