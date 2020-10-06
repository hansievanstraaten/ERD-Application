﻿using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System;
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
        private ReportTypeEnum selectedreportType;
        private ReportMasterModel[] headersAndFooters;
        private ReportMasterModel selectedHeaderAndFooter;

        public ReportHeaderFooters(ReportTypeEnum reportType)
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.selectedreportType = reportType;

            ReportTablesRepository repo = new ReportTablesRepository();

            this.uxButtonAdd.ToolTip = $"Add new {this.selectedreportType.GetDescriptionAttribute()}";

            this.HeadersAndFooters = repo.GetReportMasterByReportTypeEnum((int)this.selectedreportType).ToArray();

            if (reportType == ReportTypeEnum.ReportContent)
            {
                foreach (ReportMasterModel report in this.HeadersAndFooters)
                {
                    ReportConnectionModel connection = repo.GetProductionOrConnectionModel(report.MasterReport_Id);

                    report.ProductionConnection = connection == null ? string.Empty : connection.ReportConnectionName;
                }
            }
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

        private void OnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                ReportDesigner designer = new ReportDesigner(new ReportMasterModel 
                { 
                    ReportTypeEnum = (int)this.selectedreportType,
                    PaperKindEnum = (int)PaperKind.A4,
                    PageOrientationEnum = (int)PageOrientationEnum.Portrait,
                    PageMarginTop = 100,
                    PageMarginBottom = 100,
                    PageMarginLeft = 100,
                    PageMarginRight = 100
                });
                
                if (ControlDialog.ShowDialog($"New {this.selectedreportType.GetDescriptionAttribute()}", designer, "Save", windowState: WindowState.Maximized).IsFalse())
                {
                    return;
                }

                this.HeadersAndFooters = this.HeadersAndFooters.Add(designer.ReportMaster);
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

                if (ControlDialog.ShowDialog($"Edit {this.selectedreportType.GetDescriptionAttribute()}", designer, "Save", windowState: WindowState.Maximized).IsFalse())
                {
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}
