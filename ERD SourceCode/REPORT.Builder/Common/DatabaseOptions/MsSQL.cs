using REPORT.Data.Models;
using System.Text;
using GeneralExtensions;
using System.Collections.Generic;
using System.Linq;
using ViSo.SharedEnums;
using System.Windows.Documents;

namespace REPORT.Builder.Common.DatabaseOptions
{
    internal class MsSQL : IDataToSQL
    {
        public string BuildSelectQuery(ReportColumnModel[] columns, List<WhereParameterModel> whereParameterModel, List<ReportXMLPrintParameterModel> reportFilters)
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

                if (x == columns.Length -1)
                {
                    result.AppendLine($"             [{column.ColumnName}] ");
                }
                else if (x == 0)
                {
                    result.AppendLine($"[{column.ColumnName}], ");
                }
                else
                {
                    result.AppendLine($"             [{column.ColumnName}], ");
                }
            }

            result.AppendLine($" FROM [{columns[0].TableName}] WITH(NOLOCK) ");

            bool haveWhereClause = false;

            if (whereParameterModel.Count > 0)
            {
                result.Append("WHERE ");

                haveWhereClause = true;
            }

            foreach(WhereParameterModel parameter in  whereParameterModel.OrderBy(so => so.OperatorIndex))
            {
                result.AppendLine($"{parameter.ColumnName} = @{parameter.ParameterName} {(parameter.AndOrOperator == SqlWhereOperatorsEnum.None ? string.Empty : parameter.AndOrOperator.ParseToString())} ");
            }

            List<ReportXMLPrintParameterModel> validFilters = reportFilters
                .Where(rf => !rf.FilterValue.IsNullEmptyOrWhiteSpace())
                .ToList();

            if (reportFilters.Count > 0 
                && !haveWhereClause
                && validFilters.Count > 0)
			{
                result.Append(" WHERE ");
            }
            else if (haveWhereClause && validFilters.Count > 0)
            {
                result.Append(" AND ");
            }

            for (int x = 0; x < validFilters.Count; ++x)
			{
                ReportXMLPrintParameterModel filter = validFilters[x];

                if (filter.FilterValue.IsNullEmptyOrWhiteSpace())
                {
                    continue;
                }

                result.AppendLine($"{filter.TableName}.{filter.ColumnName} = '{filter.FilterValue}'");
                
                if (x < validFilters.Count - 1)
				{
                    result.Append(" AND ");
				}
            }

            return result.ToString();
        }
    }
}
