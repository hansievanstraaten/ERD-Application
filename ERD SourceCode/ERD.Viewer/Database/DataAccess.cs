using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ERD.Models;
using ERD.Viewer.Common;
using ERD.Viewer.Database.MsSql;
using ERD.Viewer.Models;
using GeneralExtensions;

namespace ERD.Viewer.Database
{
  internal class DataAccess
  {
    private IDataAccess accessModel;

    public DataAccess(DatabaseModel databaseModel)
    {
      this.accessModel = this.CreateClass();

      this.accessModel.Construct(databaseModel);
    }

    public XDocument ExecuteQuery(string sqlQuery)
    {
      return this.accessModel.ExecuteQuery(sqlQuery);
    }

    public List<dynamic> ExecuteQueryDynamic(string sqlQuery)
    {
      return this.accessModel.ExecuteQueryDynamic(sqlQuery);
    }

    public void ExecuteNonQuery(string sqlQuery)
    {
      this.accessModel.ExecuteNonQuery(sqlQuery);
    }
    
    private IDataAccess CreateClass()
    {
      IDataAccess result;

      switch (Connections.DatabaseModel.DatabaseType)
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
