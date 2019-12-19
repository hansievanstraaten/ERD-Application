using ERD.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ERD.Viewer.Database
{
  internal interface IDataAccess
  {
    void Construct(DatabaseModel databaseModel);

    XDocument ExecuteQuery(string sqlQuery);

    List<dynamic> ExecuteQueryDynamic(string sqlQuery);

    int ExecuteNonQuery(string sqlQuery);
  }
}
