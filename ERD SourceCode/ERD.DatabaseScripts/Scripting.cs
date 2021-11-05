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

        public static string BuildeColumnCreate(string schemaName, string tableName, ColumnObjectModel column)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scriptor.BuildeColumnCreate(schemaName, tableName, column);
        }

        public static string BuildColumnAlter(string tableName, ColumnObjectModel column)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scripting.BuildColumnAlter(tableName, column);
        }

        public static string BuildForeignKey(TableModel table)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scriptor.BuildForeignKey(table);
        }

        public static string DropForeignKey(string integrityDropString)
        {
            IScripting scriptor = Scripting.CreateClass();

            string[] keySplit = integrityDropString.Split(new string[] {"||"}, StringSplitOptions.RemoveEmptyEntries);

            return scripting.DropForeignKey(Integrity.GetTableSchema(keySplit[0]),keySplit[0], keySplit[1]);
        }

        public static string DropTable(TableModel table)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scriptor.DropTable(table);
        }

        public static string DatabaseDataType(ColumnObjectModel column)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scriptor.DatabaseDataType(column);
        }

        public static string DatafieldLength(ColumnObjectModel column)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scriptor.DatafieldLength(column);
        }

        public static string DropColumn(string schema, string tableName, string columnName)
        {
            IScripting scriptor = Scripting.CreateClass();

            return scriptor.DropColumn(schema, tableName, columnName);
        }

        private static IScripting CreateClass()
        {
            if (Scripting.scripting != null && Scripting.scripting.DatabaseScriptingType == Connections.Instance.DatabaseModel.DatabaseType)
            {
                return Scripting.scripting;
            }

            switch (Connections.Instance.DatabaseModel.DatabaseType)
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
