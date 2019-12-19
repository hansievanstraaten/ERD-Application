using ERD.Viewer.Common;
using ERD.Viewer.Database.MsSql;
using ERD.Viewer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Viewer.Database
{
  internal static class Scripting
  {
    private static IScripting scripting;

    internal static string ScriptTableCreate(TableModel table)
    {
      IScripting scriptor = Scripting.CreateClass();

      return scriptor.ScriptTableCreate(table);
    }

    internal static string BuildForeignKey(TableModel table)
    {
      IScripting scriptor = Scripting.CreateClass();

      return scriptor.BuildForeignKey(table);
    }

    internal static string DropForeignKey(string integrityDropString)
    {
      IScripting scriptor = Scripting.CreateClass();

      string[] keySplit = integrityDropString.Split(new string[] { "||"}, StringSplitOptions.RemoveEmptyEntries);

      return scripting.DropForeignKey(keySplit[0], keySplit[1]);
    }

    private static IScripting CreateClass()
    {
      if (Scripting.scripting != null && Scripting.scripting.DatabaseScriptingType == Connections.DatabaseModel.DatabaseType)
      {
        return Scripting.scripting;
      }

      switch (Connections.DatabaseModel.DatabaseType)
      {
        case DatabaseTypeEnum.SQL:
        default:
          Scripting.scripting = new MsScripting();

          break;
      }

      return Scripting.scripting;
    }
  }
}
