using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Models.ModelExstentions
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

            return $"[{table.SchemaName}].[{table.TableName}]";
        }

        public static string FullNamePostgreFormat(this TableModel table)
        {
            if (table.SchemaName.IsNullEmptyOrWhiteSpace())
            {
                return $"\"{table.TableName}\"";
            }

            string defaultName = table.SchemaName.ToLower() == "dbo" ? "public" : table.SchemaName;

            return $"\"{defaultName}\".\"{table.TableName}\"";
        }

        public static string QualifyingName(this TableModel table)
        {
            return $"{table.SchemaNameBraced(true)}{table.FullNameSQLFormat()}";
        }

        public static string SchemaNameBraced(this TableModel table, bool addDot)
        {
            return table.SchemaName.IsNullEmptyOrWhiteSpace() ? string.Empty :
                addDot ? $"[{table.SchemaName}]."
                :
                $"[{table.SchemaName}]";
        }
    }
}
