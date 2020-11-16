using GeneralExtensions;
using Report.Web.Presenters.Default;
using REPORT.Data;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Repositories;
using REPORT.Web.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace REPORT.Web
{
	public partial class _Default : Page, IDefault
	{
		private DefaultPresenter presenter;

		protected void Page_Load(object sender, EventArgs e)
		{
			this.presenter = new DefaultPresenter(this);
			
			if (!this.IsPostBack)
			{

				string fileName = WebConfigurationManager.AppSettings["ReportSystemSetup.erpt"];

				if (!File.Exists(fileName))
				{
					throw new ApplicationException("Setup not Found");
				}

				this.ReportSystemSetupFile = fileName;

				this.presenter.CreateConnection();
			
				this.presenter.LoadReportCategories();
			}
		}

		public long SelectedCategoryId { get; private set; }

		public string ReportSystemSetupFile { get; set; }

		public List<ReportCategoryModel> ReportCategories 
		{ 
			set
			{
				this.LoadCategoryTree(value);
			}
		}

		public List<ReportMasterModel> ReportsList 
		{ 
			set
			{
				this.LoadReports(value);
			}
		}

		protected void CategoryTree_NodeChanged(object sender, EventArgs e)
		{
			try
			{
				TreeNode treeNode = this.uxTree.SelectedNode.To<TreeNode>();

				ViSoTreeNode node = new ViSoTreeNode(treeNode);

				this.SelectedCategoryId = node.CarierId;

				this.presenter.LoadCategoryReports();
			}
			catch (Exception err)
			{
				throw;
			}
		}

		private void LoadCategoryTree(List<ReportCategoryModel> categories, ViSoTreeNode parentNode = null)
		{
			foreach(ReportCategoryModel parent in categories.Where(p => (parentNode == null ? !p.ParentCategoryId.HasValue : p.ParentCategoryId == parentNode.CarierId)))
			{
				ViSoTreeNode node = new ViSoTreeNode(parent.CategoryId, parent.CategoryName);

				node.CarierId = parent.CategoryId;
									
				this.LoadCategoryTree(categories, node);
				
				node.CollapseAll();

				if (parentNode == null)
				{
					this.uxTree.Nodes.Add(node);
				}
				else
				{
					parentNode.ChildNodes.Add(node);
				}
			}
		}

		private void LoadReports(List<ReportMasterModel> reports)
		{
			this.uxReportsTable.Rows.Clear();

			this.CreateTableHeader();

			foreach (ReportMasterModel report in reports)
			{
				this.CreateTableRow(report);
			}
		}

		private void CreateTableRow(ReportMasterModel report)
		{
			HtmlTableRow tableRow = new HtmlTableRow();

			tableRow.Cells.Add(this.CreateTableDataCell(report.ReportName, report.MasterReport_Id));

			tableRow.Cells.Add(this.CreateTableDataCell(report.ReportXMLVersion.ParseToString(), report.MasterReport_Id));

			tableRow.Cells.Add(this.CreateTableDataCell(report.ProjectName, report.MasterReport_Id));

			this.uxReportsTable.Rows.Add(tableRow);
		}
	
		private void CreateTableHeader()
		{
			HtmlTableRow tableRow = new HtmlTableRow();

			tableRow.Cells.Add(this.CreateTableHeaderCell("Report Name"));

			tableRow.Cells.Add(this.CreateTableHeaderCell("Active Version"));

			tableRow.Cells.Add(this.CreateTableHeaderCell("Source Project"));

			this.uxReportsTable.Rows.Add(tableRow);
		}
		private HtmlTableCell CreateTableDataCell(string cellText, long masterReport_Id)
		{
			HtmlTableCell result = new HtmlTableCell();

			result.Attributes.Add("runat", "server");

			result.Attributes.Add("onclick", $"RowSelected('{masterReport_Id}', '{cellText}', '')");

			result.InnerText = cellText;			

			result.Attributes.Add("class", "ViSo-Table-Cell");

			return result;
		}
		
		private HtmlTableCell CreateTableHeaderCell(string cellText)
		{
			HtmlTableCell result = new HtmlTableCell();

			result.InnerText = cellText;

			result.Attributes.Add("class", "ViSo-Table-Header");

			return result;
		}
	}
}