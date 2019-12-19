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
