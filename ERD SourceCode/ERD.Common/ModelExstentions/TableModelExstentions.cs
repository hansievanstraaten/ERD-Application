using ERD.Base;
using ERD.Models;
using GeneralExtensions;

namespace ERD.Common.ModelExstentions
{
    public static class TableModelExstentions
    {
        public static string FullName (this TableModel table)
        {
            if (table.SchemaName.IsNullEmptyOrWhiteSpace())
            {
                return table.TableName;
            }

            return $"{table.SchemaName}.{table.TableName}";
        }

        public static string FullNameSQLFormat(this TableModel table)
        {
            if (table.SchemaName.IsNullEmptyOrWhiteSpace())
            {
                return $"[{table.TableName}]";
            }

            string schemaName = Integrity.SchemaValidation(DatabaseTypeEnum.SQL, table.SchemaName);

            return $"[{schemaName}].[{table.TableName}]";
        }

        public static string FullNamePostgreFormat(this TableModel table)
        {
            if (table.SchemaName.IsNullEmptyOrWhiteSpace())
            {
                return $"\"{table.TableName}\"";
            }

            string schemaName = Integrity.SchemaValidation(DatabaseTypeEnum.POSTGRES, table.SchemaName);

            return $"\"{schemaName}\".\"{table.TableName}\"";
        }

        public static string SchemaNameSQL(this TableModel table, bool addDot)
        {
            string schemaName = Integrity.SchemaValidation(DatabaseTypeEnum.SQL, table.SchemaName);

            return schemaName.IsNullEmptyOrWhiteSpace() ? string.Empty :
                addDot ? $"[{schemaName}]."
                :
                $"[{schemaName}]";
        }
    }
}
