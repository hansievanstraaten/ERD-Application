using GeneralExtensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder.Printing
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : UserControlBase
    {
        private string printReportName;

        private Dictionary<int, PrintCanvas> printPages;

        public PrintPreview(string reportName, Dictionary<int, PrintCanvas> pages)
        {
            this.InitializeComponent();

            this.printReportName = reportName;

            this.printPages = pages;

            foreach (KeyValuePair<int, PrintCanvas> pageKeyPair in this.printPages)
            {
                pageKeyPair.Value.Margin = new System.Windows.Thickness(5);

                this.uxPageStack.Children.Add(pageKeyPair.Value);
            }
        }

        private void ReportPrint_Cliked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ExportPDF_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dlg = new SaveFileDialog();

                dlg.FileName = this.printReportName;

                dlg.OverwritePrompt = false;

                dlg.Filter = "PDF Files | *.pdf";

                if (dlg.ShowDialog().IsFalse())
                {
                    return;
                }

                CanvasToPDF pdf = new CanvasToPDF();

                pdf.ConvertToPdf(dlg.FileName, this.printPages);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}
