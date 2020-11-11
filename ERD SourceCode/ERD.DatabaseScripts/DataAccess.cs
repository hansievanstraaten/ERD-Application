using ERD.Base;
using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DatabaseScripts.MsSql;
using ERD.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ERD.Viewer.Database
{
    public class DataAccess
    {
        private IDataAccess accessModel;

        public DataAccess(DatabaseModel databaseModel)
        {
            this.accessModel = this.CreateClass();

            this.accessModel.Construct(databaseModel);
        }

        public DataAccess(DatabaseTypeEnum databaseType, Dictionary<string, string> connectionValues)
        {
            this.accessModel = this.CreateClass(databaseType);

            this.accessModel.Construct(connectionValues);
        }

        public XDocument ExecuteQuery(string sqlQuery, int commandTimeout = 30)
        {
            return this.accessModel.ExecuteQuery(sqlQuery, commandTimeout);
        }

        public List<dynamic> ExecuteQueryDynamic(string sqlQuery, int commandTimeout = 30)
        {
            return this.accessModel.ExecuteQueryDynamic(sqlQuery, commandTimeout);
        }

        public void ExecuteNonQuery(string sqlQuery, int commandTimeout = 30)
        {
            this.accessModel.ExecuteNonQuery(sqlQuery, commandTimeout);
        }

        private IDataAccess CreateClass()
        {
            return this.CreateClass(Connections.Instance.DatabaseModel.DatabaseType);
        }

        private IDataAccess CreateClass(DatabaseTypeEnum databaseType)
        {
            IDataAccess result;

            switch (databaseType)
            {
                case DatabaseTypeEnum.SQL:
                default:
                    result = new MsDataAccess();

                    break;
            }

            return result;
        }
    }
}
