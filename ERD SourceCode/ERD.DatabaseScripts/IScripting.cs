using ERD.Base;
using ERD.Models;

namespace ERD.Viewer.Database
{
    internal interface IScripting
    {
        DatabaseTypeEnum DatabaseScriptingType
        {
            get;
        }

        string ScriptTableCreate(TableModel table);

        string CreateSchema(string schemaName);

        string BuildeColumnCreate(string schemaName, string tableName, ColumnObjectModel column);

        string BuildColumnAlter(string tableName, ColumnObjectModel column);

        string BuildForeignKey(TableModel table);

        string DropForeignKey(string schema, string tableName, string constaintName);

        string DropTable(TableModel table);

        string DropColumn(string schema, string tableName, string columnName);

        string DatabaseDataType(ColumnObjectModel column);

        string DatafieldLength(ColumnObjectModel column);
    }
}
