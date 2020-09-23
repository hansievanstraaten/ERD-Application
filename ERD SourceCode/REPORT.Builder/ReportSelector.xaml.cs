using ERD.Models.ReportModels;
using GeneralExtensions;
using Newtonsoft.Json;
using REPORT.Data;
using System;
using System.IO;
using System.Windows;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.Exstention;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportSelector.xaml
    /// </summary>
    public partial class ReportSelector : UserControlBase
    {
        private ReportHeaderFooters uxCoverPage;

        private ReportHeaderFooters uxHeadersAndFooters;

        private ReportHeaderFooters uxFinalPage;

        private ReportHeaderFooters uxDataPages;

        public ReportSelector(string projectFileDirectory)
        {
            this.InitializeComponent();

            this.ReportFileName = Path.Combine(projectFileDirectory, Constants.Constants.ReportSetupFileName);

            this.InitializeTabs();
        }

        public string ReportFileName { get; private set; }

        public ReportSetupModel ReportSetup { get; private set; }

        private void InitializeTabs()
        {
            try
            {
                #region DB SETUP OPTIONS

                if (!File.Exists(this.ReportFileName))
                {
                    MessageBox.Show("Setup not Found");

                    this.CloseIfNotMainWindow(false);

                    return;
                }

                string fileContent = File.ReadAllText(this.ReportFileName);

                this.ReportSetup = JsonConvert.DeserializeObject(fileContent, typeof(ReportSetupModel)) as ReportSetupModel;

                DatabaseConnection.Instance.InitializeConnectionString(this.ReportSetup);

                #endregion

                #region INITIALIZE TABS

                this.uxDataPages = new ReportHeaderFooters(ReportTypeEnum.ReportContent) { Title = "Data Report" };

                this.uxCoverPage = new ReportHeaderFooters(ReportTypeEnum.CoverPage) { Title = "Cover Pages" };

                this.uxHeadersAndFooters = new ReportHeaderFooters(ReportTypeEnum.PageHeaderAndFooter) { Title = "Headers and Footers" };

                this.uxFinalPage = new ReportHeaderFooters(ReportTypeEnum.FinalPage) { Title = "Final Pages" };

                this.uxMainTab.Items.Add(this.uxDataPages);

                this.uxMainTab.Items.Add(this.uxCoverPage);

                this.uxMainTab.Items.Add(this.uxHeadersAndFooters);

                this.uxMainTab.Items.Add(this.uxFinalPage);

                this.uxMainTab.SetActive(0);
                
                #endregion
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}
