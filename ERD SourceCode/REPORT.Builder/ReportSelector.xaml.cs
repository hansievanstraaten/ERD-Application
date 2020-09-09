using GeneralExtensions;
using Newtonsoft.Json;
using REPORT.Builder.Models;
using System;
using System.IO;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.Exstention;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportSelector.xaml
    /// </summary>
    public partial class ReportSelector : UserControlBase
    {
        public ReportSelector(string projectFileDirectory)
        {
            this.InitializeComponent();

            this.ReportFileName = Path.Combine(projectFileDirectory, Constants.Constants.ReportSetupFileName);

            this.Loaded += this.ReportSelector_Loaded;
        }

        public string ReportFileName { get; private set; }

        public ReportSetupModel ReportSetup { get; private set; }

        private void ReportSelector_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!File.Exists(this.ReportFileName))
                {
                    MessageBox.Show("Setup not Found");

                    this.CloseIfNotMainWindow(false);

                    return;
                }

                string fileContent = File.ReadAllText(this.ReportFileName);

                this.ReportSetup = JsonConvert.DeserializeObject(fileContent, typeof(ReportSetupModel)) as ReportSetupModel;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}
