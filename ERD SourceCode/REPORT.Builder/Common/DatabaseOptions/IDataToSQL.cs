using REPORT.Data.Models;
using System.Collections.Generic;

namespace REPORT.Builder.Common.DatabaseOptions
{
    internal interface IDataToSQL
    {
        string BuildSelectQuery(
            ReportColumnModel[] columns, 
            List<WhereParameterModel> whereParameterModel, 
            List<ReportXMLPrintParameterModel> reportFilters,
            Dictionary<string, ReportSQLReplaceHeaderModel> replacementColumns,
            string orderByString);

         string UpdateStatements(UpdateStatementModel updateStatements, out List<string> columnValues);

    }
}
