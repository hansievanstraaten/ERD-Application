using System;
using WPF.Tools.BaseClasses;
using GeneralExtensions;
using System.Windows;
using REPORT.Builder.Models;
using System.IO;
using WPF.Tools.Exstention;
using ERD.Base;
using Newtonsoft.Json;
using System.ComponentModel;
using REPORT.Builder.ReportEnums;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportSystemSetup.xaml
    /// </summary>
    public partial class ReportSystemSetup : UserControlBase
    {
        public ReportSystemSetup(string projectFileDirectory)
        {
            this.InitializeComponent();

            if (projectFileDirectory.IsNullEmptyOrWhiteSpace())
            {
                MessageBox.Show("Project Setup Required");

                this.CloseIfNotMainWindow(false);

                return;
            }

            this.ReportFileName = Path.Combine(projectFileDirectory, Constants.Constants.ReportSetupFileName);

            this.Loaded += this.ReportSystemSetup_Loaded;
        }

        public string ReportFileName { get; private set; }

        public ReportSetupModel ReportSetup { get; private set; }

        public bool Save()
        {
            try
            {
                string fileObject = JsonConvert.SerializeObject(this.ReportSetup);

                File.WriteAllText(this.ReportFileName, fileObject);

                return true;
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
                if (File.Exists(this.ReportFileName))
                {
                    string fileContent = File.ReadAllText(this.ReportFileName);

                    this.ReportSetup = JsonConvert.DeserializeObject(fileContent, typeof(ReportSetupModel)) as ReportSetupModel;
                }
                else
                {
                    this.ReportSetup = new ReportSetupModel();
                }

                this.uxReportSetup.Items.Add(this.ReportSetup);

                this.ReportSetup.PropertyChanged += this.ReportSetup_Changed;

                if (this.uxReportSetup["Save Report In"].GetValue() == null)
                {
                    this.ReportSetup.StorageType = StorageTypeEnum.DatabaseSystem;
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

        private void OnModelViewer_Browse(object sender, string buttonKey)
        {
            switch(buttonKey)
            {
                case "SaveDirectory":

                    System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

                    if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }

                    this.ReportSetup.FileDirectory = folder.SelectedPath;

                    break;
            }
        }

        private void ReportSetup_Changed(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
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

            switch (this.ReportSetup.StorageType)
            {
                case StorageTypeEnum.FileSystem:

                    this.uxReportSetup["Save Report Setup In"].Visibility = Visibility.Visible;

                    break;

                case StorageTypeEnum.DatabaseSystem:
                default:

                    this.uxReportSetup["Save Report Setup In"].Visibility = Visibility.Collapsed;

                    if (this.uxReportSetup.Items.Count > 1)
                    {
                        this.uxReportSetup[1].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.uxReportSetup.Items.Add(this.ReportSetup.DataBaseSource);
                    }

                    break;
            }
        }
    }
}
