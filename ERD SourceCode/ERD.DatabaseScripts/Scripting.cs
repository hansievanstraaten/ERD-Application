using ERD.Base;
using ERD.Common;
using ERD.Models;
using ERD.Viewer.Database;
using ERD.Viewer.Database.MsSql;
using System;

namespace ERD.DatabaseScripts
{
  public static class Scripting
  {
    private static IScripting scripting;

    public static string ScriptTableCreate(TableModel table)
    {
      IScripting scriptor = Scripting.CreateClass();

      return scriptor.ScriptTableCreate(table);
    }

    public static string BuildForeignKey(TableModel table)
    {
      IScripting scriptor = Scripting.CreateClass();

      return scriptor.BuildForeignKey(table);
    }

    public static string DropForeignKey(string integrityDropString)
    {
      IScripting scriptor = Scripting.CreateClass();

      string[] keySplit = integrityDropString.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

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
