using REPORT.Data.Models;
using System.Text;
using GeneralExtensions;
using System.Collections.Generic;
using System.Linq;
using ViSo.SharedEnums;

namespace REPORT.Builder.Common.DatabaseOptions
{
    internal class MsSQL : IDataToSQL
    {
        public string BuildSelectQuery(ReportColumnModel[] columns, List<WhereParameterModel> whereParameterModel)
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

            result.AppendLine($" FROM [{columns[0].TableName}] WITH(NOLOCK) ");

            if (whereParameterModel.Count > 0)
            {
                result.Append("WHERE ");
            }

            // TODO: Add Where Clause
            foreach(WhereParameterModel parameter in  whereParameterModel.OrderBy(so => so.OperatorIndex))
            {
                result.AppendLine($"{parameter.ColumnName} = @{parameter.ParameterName} {(parameter.AndOrOperator == SqlWhereOperatorsEnum.None ? string.Empty : parameter.AndOrOperator.ParseToString())} ");
            }

            return result.ToString();
        }
    }
}
