using ERD.Base;
using ERD.Models;

namespace ERD.Viewer.Database
{
  internal interface IScripting
  {
    DatabaseTypeEnum DatabaseScriptingType { get; }

    string ScriptTableCreate(TableModel table);

    string BuildForeignKey(TableModel table);

    string DropForeignKey(string tableName, string constaintName);
  }
}
