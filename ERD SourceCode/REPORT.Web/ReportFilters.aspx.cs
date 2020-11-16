using Report.Web.Presenters.ReportFilters;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Web.UI;
using GeneralExtensions;
using Report.Web.Presenters.Exstentions;
using System.Web.UI.HtmlControls;

namespace REPORT.Web
{
	public partial class ReportFilters : Page, IReportFilters
	{
		private ReportFiltersPresenter presenter;

		protected void Page_Load(object sender, EventArgs e)
		{
			Page senderPage = sender as Page;

			Dictionary<string, string> clientQueries = senderPage.ClientQueryString.SplitClientQueryString();

			this.presenter = new ReportFiltersPresenter(this);

			if (clientQueries.ContainsKey("MasterReport_Id"))
			{
				this.MasterReport_Id = clientQueries["MasterReport_Id"].ToInt64();

				this.uxReportName.InnerText = $"Report: {clientQueries["ReportName"]}";

				this.presenter.GetReportFilters();
			}
		}

		public long MasterReport_Id { get; private set; }

		public List<ReportXMLPrintParameterModel> ReportParameters 
		{ 
			set
			{
				foreach (ReportXMLPrintParameterModel item in value)
				{
					this.AddFilterItem(item);
				}
			}
		}
	
		public void AddFilterItem(ReportXMLPrintParameterModel filterModel)
		{
			HtmlGenericControl filter = new HtmlGenericControl("div");

			filter.InnerText = filterModel.FilterCaption.IsNullEmptyOrWhiteSpace() ? filterModel.ColumnName : filterModel.FilterCaption;

			this.uxFilters.Controls.Add(filter);
		}
	}
}