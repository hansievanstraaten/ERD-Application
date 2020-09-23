using REPORT.Data.Models;

namespace REPORT.Builder.Common.DatabaseOptions
{
    internal interface IDataToSQL
    {
        string BuildSelectQuery(ReportColumnModel[] columns);

    }
}
