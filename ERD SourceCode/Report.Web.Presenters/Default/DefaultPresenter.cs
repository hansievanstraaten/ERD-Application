using REPORT.Data;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.IO;

namespace Report.Web.Presenters.Default
{
	public class DefaultPresenter
	{
		private IDefault view;

		public DefaultPresenter(IDefault defaultView)
		{
			this.view = defaultView;
		}

		public void CreateConnection()
		{
			if (!File.Exists(this.view.ReportSystemSetupFile))
			{
				throw new ApplicationException("Setup not Found");
			}

			DbConfiguration.Instance.Initialize(this.view.ReportSystemSetupFile);

			DatabaseConnection.Instance.InitializeConnectionString(DbConfiguration.Instance.ReportSetup);
		}

		public void LoadReportCategories()
		{
			ReportTablesRepository repo = new ReportTablesRepository();

			this.view.ReportCategories = repo.GetActiveCategories();
		}
	
		public void LoadCategoryReports()
		{
			ReportTablesRepository repo = new ReportTablesRepository();

			this.view.ReportsList = repo.GetReportMasterByCategoryId(this.view.SelectedCategoryId);
		}
	}
}
