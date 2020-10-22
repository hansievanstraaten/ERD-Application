using ERD.Models.ReportModels;
using GeneralExtensions;
using Newtonsoft.Json;
using REPORT.Data;
using REPORT.Data.SQLRepository;
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

        private ReportCategories uxDataPages;

        public ReportSelector(string projectFileDirectory)
        {
            this.InitializeComponent();

            this.ReportFileName = Path.Combine(projectFileDirectory, Constants.ReportConstants.ReportSetupFileName);

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
                    throw new ApplicationException("Setup not Found");
                }

                string fileContent = File.ReadAllText(this.ReportFileName);

                this.ReportSetup = JsonConvert.DeserializeObject(fileContent, typeof(ReportSetupModel)) as ReportSetupModel;

                if (this.ReportSetup.DataBaseSource.IsEncrypted)
				{
                    this.ReportSetup.DataBaseSource.Password = this.ReportSetup.DataBaseSource.Password.Decrypt();

                    this.ReportSetup.DataBaseSource.IsEncrypted = false;
                }

                DbScript script = new DbScript();

                script.InitializeReportsDB(this.ReportSetup);

                DatabaseConnection.Instance.InitializeConnectionString(this.ReportSetup);

                #endregion

                #region INITIALIZE TABS

                this.uxDataPages = new ReportCategories() { Title = "Data Report" };

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
