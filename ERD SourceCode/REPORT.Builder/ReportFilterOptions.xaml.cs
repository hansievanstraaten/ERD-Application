using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for ReportFilterOptions.xaml
	/// </summary>
	public partial class ReportFilterOptions : UserControlBase
	{
		public ReportFilterOptions()
		{
			this.InitializeComponent();

			this.Loaded += this.ReportFilterOptions_Loaded;
		}
				
		public ReportFilterOptions(long masterReport_Id) : this()
		{
			this.MasterReport_Id = masterReport_Id;

			ReportTablesRepository repo = new ReportTablesRepository();

			this.ReportXMLVersion = repo.GetReportXMLVersion(this.MasterReport_Id);

			this.ReportParameters = repo.GetPrintparameters(this.MasterReport_Id, this.ReportXMLVersion);

			this.LoadFIlters();
		}

    public ReportFilterOptions(List<ReportXMLPrintParameterModel> parameterFilters) : this()
		{
			this.ReportParameters = parameterFilters;

			this.LoadFIlters();
		}

		public int ReportXMLVersion { get; private set; }

		public long MasterReport_Id { get; private set; }

		public List<ReportXMLPrintParameterModel> ReportParameters { get; private set; }

		public bool ValidateFilters()
		{
			try
			{
				return !this.uxParameters.HasValidationError;
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());

				return false;
			}
		}

		private void ReportFilterOptions_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				this.uxParameters.AllignAllCaptions();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void LoadFIlters()
		{
			for (int x = 0; x < this.ReportParameters.Count; ++x)
			{
				ReportXMLPrintParameterModel item = this.ReportParameters[x];

				this.uxParameters.Items.Add(item);

				this.uxParameters[x].HideHeader(true);

				this.uxParameters[x, 0].Caption = item.FilterCaption;
			}

			//this.uxParameters.AllignAllCaptions();
		}

	}
}
