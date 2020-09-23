using REPORT.Data.Models;
using System.Text;
using GeneralExtensions;

namespace REPORT.Builder.Common.DatabaseOptions
{
    internal class MsSQL : IDataToSQL
    {
        public string BuildSelectQuery(ReportColumnModel[] columns)
        {
            if (!columns.HasElements())
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();

            result.Append("SELECT ");

            for(int x = 0; x < columns.Length; ++x)
            {
                ReportColumnModel column = columns[x];

                if (x == 0)
                {
                    result.AppendLine($"[{column.ColumnName}], ");
                }
                else if (x == columns.Length -1)
                {
                    result.AppendLine($"             [{column.ColumnName}] ");
                }
                else
                {
                    result.AppendLine($"             [{column.ColumnName}], ");
                }
            }

            result.AppendLine($" FROM [{columns[0].TableName}] WITH(NOLOCK)");

            // TODO: Add Where Clause

            return result.ToString();
        }
    }
}
