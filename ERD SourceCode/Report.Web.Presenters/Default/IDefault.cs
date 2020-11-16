using REPORT.Data.Models;
using System.Collections.Generic;

namespace Report.Web.Presenters.Default
{
	public interface IDefault
	{
		long SelectedCategoryId { get; }

		string ReportSystemSetupFile { get; set; }

		List<ReportCategoryModel> ReportCategories { set; }

		List<ReportMasterModel> ReportsList { set; }
	}
}
