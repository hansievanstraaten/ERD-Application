using ERD.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ERD.DatabaseScripts
{
  internal interface IDataAccess
  {
    void Construct(DatabaseModel databaseModel);

    XDocument ExecuteQuery(string sqlQuery);

    List<dynamic> ExecuteQueryDynamic(string sqlQuery);

    int ExecuteNonQuery(string sqlQuery);
  }
}
