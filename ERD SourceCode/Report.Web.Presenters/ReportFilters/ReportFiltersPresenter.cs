using REPORT.Data.SQLRepository.Repositories;

namespace Report.Web.Presenters.ReportFilters
{
	public class ReportFiltersPresenter
	{
		private IReportFilters view;

		public ReportFiltersPresenter(IReportFilters filterView)
		{
			this.view = filterView;
		}

		public void GetReportFilters()
		{
			ReportTablesRepository repo = new ReportTablesRepository();

			this.view.ReportParameters = repo.GetPrintparameters(this.view.MasterReport_Id);

		}
	}
}
