using ERD.Base;
using GeneralExtensions;
using REPORT.Data;
using REPORT.Data.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.Exstention;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportSystemSetup.xaml
    /// </summary>
    public partial class ReportSystemSetup : UserControlBase
    {
        public ReportSystemSetup(string projectFileDirectory)
        {
            InitializeComponent();

            if (projectFileDirectory.IsNullEmptyOrWhiteSpace())
            {
                MessageBox.Show("Project Setup Required");

                this.CloseIfNotMainWindow(false);

                return;
            }

            this.ReportFileName = Path.Combine(projectFileDirectory, Constants.ReportSetupFileName);

            DbConfiguration.Instance.Initialize(this.ReportFileName);

            this.Loaded += this.ReportSystemSetup_Loaded;

            this.Unloaded += this.ReportSystemSetup_Unloaded;
        }

        public string ReportFileName { get; private set; }

        public bool Save()
        {
            try
            {
                return DbConfiguration.Instance.Save();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
        }

        private void ReportSystemSetup_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.uxReportSetup.Items.Add(DbConfiguration.Instance.ReportSetup);

                //this.uxReportSetup["Database Type"].IsEnabled = false;

                DbConfiguration.Instance.ReportSetup.PropertyChanged += this.ReportSetup_Changed;

                if (this.uxReportSetup["Database Type"].GetValue() == null)
                {
                    DbConfiguration.Instance.ReportSetup.StorageType = StorageTypeEnum.SQLite;
                }
                else
                {
                    this.DoStorateTypeSetup();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportSystemSetup_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DbConfiguration.Instance.ReportSetup.PropertyChanged -= this.ReportSetup_Changed;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void OnModelViewer_Browse(object sender, string buttonKey)
        {
            switch (buttonKey)
            {
                case "SaveDirectory":

                    System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

                    if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }

                    DbConfiguration.Instance.ReportSetup.FileDirectory = folder.SelectedPath;

                    break;
            }
        }

        private void ReportSetup_Changed(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "StorageType":

                    this.DoStorateTypeSetup();

                    break;
            }
        }

        private void DoStorateTypeSetup()
        {
            if (this.uxReportSetup.Items.Count > 1)
            {
                this.uxReportSetup[1].Visibility = Visibility.Collapsed;
            }

            switch (DbConfiguration.Instance.ReportSetup.StorageType)
            {

                case StorageTypeEnum.MsSql:

                    if (this.uxReportSetup.Items.Count > 1)
                    {
                        this.uxReportSetup[1].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.uxReportSetup.Items.Add(DbConfiguration.Instance.ReportSetup.DataBaseSource);
                    }

                    this.uxReportSetup[1]["Database Type"].SetValue(DatabaseTypeEnum.SQL);
                    this.uxReportSetup[1]["Database Type"].IsReadOnly = true;
                    this.uxReportSetup["DB File Location"].Visibility = Visibility.Collapsed;
                    this.uxReportSetup[1]["Port"].Visibility = Visibility.Collapsed;
                    this.uxReportSetup[1]["SSL Mode"].Visibility = Visibility.Collapsed;
                    break;

                case StorageTypeEnum.Postgres:

                    if (this.uxReportSetup.Items.Count > 1)
                    {
                        this.uxReportSetup[1].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.uxReportSetup.Items.Add(DbConfiguration.Instance.ReportSetup.DataBaseSource);
                    }

                    this.uxReportSetup[1]["Database Type"].SetValue(DatabaseTypeEnum.POSTGRES);
                    this.uxReportSetup[1]["Database Type"].IsReadOnly = true;
                    this.uxReportSetup["DB File Location"].Visibility = Visibility.Collapsed;
                    this.uxReportSetup[1]["Port"].Visibility = Visibility.Visible;
                    this.uxReportSetup[1]["SSL Mode"].Visibility = Visibility.Visible;

                    break;

                //case StorageTypeEnum.FileSystem:
                case StorageTypeEnum.SQLite:
                default:
                    this.uxReportSetup["DB File Location"].Visibility = Visibility.Visible;

                    break;

            }
        }
    }
}
