using ERD.Models;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ERD.Common
{
  public static class Connections
  {
    public static string DefaultConnectionName = "Default";

    public static DatabaseModel DefaultDatabaseModel { get; set; }

    public static DatabaseModel DatabaseModel { get; set; }

    public static Dictionary<string, UserNameAndPasswordModel> SessionPasswords = new Dictionary<string, UserNameAndPasswordModel>();

    public static Dictionary<string, AltDatabaseModel> AlternativeModels = new Dictionary<string, AltDatabaseModel>();

    public static void SetConnection(MenuItem item)
    {
      if (item.Tag == null || item.Tag.ToString() == Connections.DefaultConnectionName)
      {
        Connections.DatabaseModel = Connections.DefaultDatabaseModel;

        return;
      }

      Connections.DatabaseModel = Connections.AlternativeModels[item.Tag.ToString()];
    }

    public static void SetDefaultConnection()
    {
      Connections.DatabaseModel = Connections.DefaultDatabaseModel;
    }
  }
}
