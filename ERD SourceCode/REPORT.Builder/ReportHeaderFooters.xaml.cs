using ERD.Common;
using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using ViSo.Dialogs.Controls;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportHeaderFooters.xaml
    /// </summary>
    public partial class ReportHeaderFooters : UserControlBase
    {        
        private ReportTypeEnum selectedReportType;

        private ReportMasterModel[] headersAndFooters;

        private ReportMasterModel selectedHeaderAndFooter;

        public ReportHeaderFooters(ReportTypeEnum reportType)
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.selectedReportType = reportType;

            this.Loaded += this.ReportHeaderFooters_Loaded;
            
            this.uxButtonAdd.ToolTip = $"Add new {this.selectedReportType.GetDescriptionAttribute()}";            
        }

        public void SetReportTypeCategory(long? category)
        {
            this.reportCategoryId = category;

            ReportTablesRepository repo = new ReportTablesRepository();

            this.HeadersAndFooters = repo
                .GetReportMasterByReportTypeEnum(
                        (int)this.selectedReportType, 
                        General.ProjectModel.ModelName, 
                        this.reportCategoryId.HasValue ? this.reportCategoryId.Value : 0)
                .ToArray();
        }

        public ReportMasterModel SelectedHeaderAndFooter
        {
            get
            {
                return this.selectedHeaderAndFooter;
            }

            set
            {
                this.selectedHeaderAndFooter = value;

                base.OnPropertyChanged(() => this.SelectedHeaderAndFooter);

                if (value != null && value.Description.Length > 0)
                {
                    this.uxDescription.Content = value.Description.UnzipFile();
                }
                else
                {
                    this.uxDescription.Content = string.Empty;
                }
            }
        }

        public ReportMasterModel[] HeadersAndFooters
        {
            get
            {
                return this.headersAndFooters;
            }

            set
            {
                this.headersAndFooters = value;

                base.OnPropertyChanged(() => this.HeadersAndFooters);
            }
        }

        private void ReportHeaderFooters_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportTablesRepository repo = new ReportTablesRepository();

                if (this.selectedReportType == ReportTypeEnum.ReportContent)
                {
                    this.HeadersAndFooters = new ReportMasterModel[] { };

                    foreach (ReportMasterModel report in this.HeadersAndFooters)
                    {
                        ReportConnectionModel connection = repo.GetProductionOrConnectionModel(report.MasterReport_Id);

                        report.ProductionConnection = connection == null ? string.Empty : connection.ReportConnectionName;
                    }
                }
                else
                {
                    this.HeadersAndFooters = repo.GetReportMasterByReportTypeEnum((int)this.selectedReportType, General.ProjectModel.ModelName).ToArray();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void OnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (this.selectedReportType == ReportTypeEnum.ReportContent
                    && !this.reportCategoryId.HasValue)
                {
                    throw new ApplicationException("Invalid report category selected");
                }

                ReportDesigner designer = new ReportDesigner(new ReportMasterModel 
                { 
                    ReportTypeEnum = (int)this.selectedReportType,
                    PaperKindEnum = (int)PaperKind.A4,
                    PageOrientationEnum = (int)PageOrientationEnum.Portrait,
                    PageMarginTop = 100,
                    PageMarginBottom = 100,
                    PageMarginLeft = 100,
                    PageMarginRight = 100,
                    ProjectName = General.ProjectModel.ModelName,
                    CategoryId = this.reportCategoryId
                });

                ControlDialog.WindowsShowIsClosing += this.WindowsShow_IsClosing;

                ControlDialog.Show($"New {this.selectedReportType.GetDescriptionAttribute()}", designer, "Save", windowState: WindowState.Maximized);                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void WindowsShow_IsClosing(object sender,UserControlBase control, CancelEventArgs e)
        {
            try
            {
                ReportDesigner userControl = control.To<ReportDesigner>();

                ControlDialog.WindowsShowIsClosing -= this.WindowsShow_IsClosing;

                if (userControl.ReportMaster.MasterReport_Id <= 0)
                {   // User did not save the report
                    return;
                }

                this.HeadersAndFooters = this.HeadersAndFooters.Add(userControl.ReportMaster);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void OnEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.SelectedHeaderAndFooter == null)
            {
                MessageBox.Show("Please select a Report Object");

                return;
            }

            try
            {
                ReportDesigner designer = new ReportDesigner(this.SelectedHeaderAndFooter);

                ControlDialog.Show($"Edit {this.selectedReportType.GetDescriptionAttribute()}", designer, "Save", windowState: WindowState.Maximized);                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private long? reportCategoryId { get; set; }
    }
}
