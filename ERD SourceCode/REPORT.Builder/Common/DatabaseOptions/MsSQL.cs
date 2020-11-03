using GeneralExtensions;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using ViSo.SharedEnums;

namespace REPORT.Builder.Common.DatabaseOptions
{
	internal class MsSQL : IDataToSQL
    {
        public string UpdateStatements(UpdateStatementModel updateStatements, out List<string> columnValues)
		{
            columnValues = new List<string>();

            List<string> columnResults = new List<string>();

            string result = this.BuildUpdateSql(updateStatements, out columnResults);

            columnValues.AddRange(columnResults.Distinct());

            return result;
		}

        public string BuildSelectQuery(
            ReportColumnModel[] columns, 
            List<WhereParameterModel> whereParameterModel, 
            List<ReportXMLPrintParameterModel> reportFilters,
            Dictionary<string, ReportSQLReplaceHeaderModel> replacementColumns,
            string orderByString)
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
                    result.AppendLine($"             [{columns[0].TableName}].[{column.ColumnName}] ");
                }
                else if (x == 0)
                {
                    result.AppendLine($"[{columns[0].TableName}].[{column.ColumnName}], ");
                }
                else
                {
                    result.AppendLine($"             [{columns[0].TableName}].[{column.ColumnName}], ");
                }
            }

            // Add replacement SQL here
            foreach(KeyValuePair<string, ReportSQLReplaceHeaderModel> replacekey in replacementColumns)
			{
                result.Replace(replacekey.Key, this.BuildReplacementSQL(replacekey));
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

            if (!orderByString.IsNullEmptyOrWhiteSpace())
			{
                result.Append($" ORDER BY {orderByString} ASC");
			}

            return result.ToString();
        }

        private string BuildUpdateSql(UpdateStatementModel statement, out List<string> columnValues)
		{
            columnValues = new List<string>();

            StringBuilder whereCaluse = new StringBuilder();

            foreach (UpdateValueModel item in statement.WhereValus)
            {
                if (item.IsDatabaseValue)
                {
                    string[] columnSplit = item.UpdateValue.Split('.', StringSplitOptions.None);

                    whereCaluse.Append($"{item.ColumnName} = '[[{columnSplit[1]}]]' AND ");

                    columnValues.Add(columnSplit[1]);
                }
                else
                {
                    whereCaluse.Append($"{item.ColumnName} = '{item.UpdateValue}' AND ");
                }
            }

            whereCaluse.Remove(whereCaluse.Length - 4, 4);

            StringBuilder result = new StringBuilder();

            result.AppendLine($"IF EXISTS (SELECT 1");
            result.AppendLine($"            FROM [{statement.UpdateTableName}]");
            result.AppendLine($"            WHERE {whereCaluse.ToString()})");
            result.AppendLine("BEGIN");

            result.AppendLine($"    UPDATE [{statement.UpdateTableName}]");

            result.Append("    SET ");

            foreach (UpdateValueModel valueItem in statement.Values)
			{
                if (valueItem.IsDatabaseValue)
                {
                    string[] columnSplit = valueItem.UpdateValue.Split('.', StringSplitOptions.None);

                    result.Append($"{valueItem.ColumnName} = '[[{columnSplit[1]}]]',");

                    columnValues.Add(columnSplit[1]);
                }
                else
                {
                    result.Append($"{valueItem.ColumnName} = '{valueItem.UpdateValue}',");
                }
			}

            result.Remove(result.Length - 1, 1);

            result.AppendLine($"    WHERE {whereCaluse.ToString()}");

            result.AppendLine("END");
            result.AppendLine("ELSE");
            result.AppendLine("BEGIN");

            result.AppendLine($"    INSERT INTO {statement.UpdateTableName} (");
            
            foreach (UpdateValueModel valueItem in statement.Values)
            {
                result.Append($"{valueItem.ColumnName},");
            }

            result.Remove(result.Length - 1, 1);

            result.AppendLine(")");

            result.AppendLine($"    VALUES (");

            foreach (UpdateValueModel valueItem in statement.Values)
            {
                if (valueItem.IsDatabaseValue)
                {
                    string[] columnSplit = valueItem.UpdateValue.Split('.', StringSplitOptions.None);

                    result.Append($"'[[{columnSplit[1]}]]',");

                    columnValues.Add(columnSplit[1]);
                }
                else
                {
                    result.Append($"'{valueItem.UpdateValue}',");
                }
            }

            result.Remove(result.Length - 1, 1);

            result.AppendLine(")");

            result.AppendLine("END");

            return result.ToString();
		}

        private string BuildReplacementSQL(KeyValuePair<string, ReportSQLReplaceHeaderModel> replacementKey)
		{
            ReportSQLReplaceHeaderModel value = replacementKey.Value;

            StringBuilder result = new StringBuilder();

            result.Append("(SELECT CONCAT(");

            foreach (string column in value.UseColumns)
            {
                result.Append($"(SELECT {column} FROM {value.UseTable} WHERE ");

                for (int x = 0; x < value.WhereDetails.Count; ++x)
                {
                    ReportSQLReplaceDetailModel detail = value.WhereDetails[x];

                    if (detail.IsColumn)
                    {
                        if (x == (value.WhereDetails.Count - 1))
                        {
                            result.Append($"{detail.WhereOption} = {detail.WhereValue} ");
                        }
                        else
                        {
                            result.Append($"{detail.WhereOption} = {detail.WhereValue} AND ");
                        }
                    }
                    else
                    {
                        if (x == (value.WhereDetails.Count - 1))
                        {
                            result.Append($"{detail.WhereOption} = '{detail.WhereValue}' ");
                        }
                        else
                        {
                            result.Append($"{detail.WhereOption} = '{detail.WhereValue}' AND ");
                        }
                    }

                }

                result.Append("), ' ', ");
            }

            result.Remove(result.Length - 2, 2);

            result.Append($")) AS {value.ReplaceColumn}, ");

            result.Append($"{replacementKey.Key} AS {value.ReplaceColumn}_Value ");

            return result.ToString();
		}

		
	}
}
