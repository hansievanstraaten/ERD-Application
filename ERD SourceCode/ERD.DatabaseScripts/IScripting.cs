using ERD.Base;
using ERD.Models;

namespace ERD.Viewer.Database
{
  internal interface IScripting
  {
    DatabaseTypeEnum DatabaseScriptingType { get; }

    string ScriptTableCreate(TableModel table);

    string BuildeColumnCreate(string tableName, ColumnObjectModel column);

    string BuildColumnAlter(string tableName, ColumnObjectModel column);

    string BuildForeignKey(TableModel table);

    string DropForeignKey(string tableName, string constaintName);

    string DropTable(TableModel table);

    string DropColumn(string tableName, string columnName);
  }
}
