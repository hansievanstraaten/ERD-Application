using ERD.Models;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ERD.DatabaseScripts
{
    internal interface IDataAccess
    {
        void Construct(DatabaseModel databaseModel);

        void Construct(Dictionary<string, string> setupValues);

        XDocument ExecuteQuery(string sqlQuery, int commandTimeout = 30);

        List<dynamic> ExecuteQueryDynamic(string sqlQuery, int commandTimeout = 30);

        int ExecuteNonQuery(string sqlQuery, int commandTimeout = 30);
    }
}
