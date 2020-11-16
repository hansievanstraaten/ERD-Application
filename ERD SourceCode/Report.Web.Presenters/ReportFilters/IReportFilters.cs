using REPORT.Data.Models;
using System.Collections.Generic;

namespace Report.Web.Presenters.ReportFilters
{
	public interface IReportFilters
	{
		long MasterReport_Id { get; }

		List<ReportXMLPrintParameterModel> ReportParameters { set; }
	}
}
