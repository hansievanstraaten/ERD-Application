using GeneralExtensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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

            int printedPages = 0;

            foreach (KeyValuePair<int, PrintCanvas> pageKeyPair in this.printPages)
            {
                pageKeyPair.Value.Margin = new System.Windows.Thickness(5);

                this.uxPageStack.Children.Add(pageKeyPair.Value);

                ++printedPages;

                if (printedPages >= 15)
				{   // Do this to prevent over poading of report. This is to heavy
                    break;
				}
            }

            if (printedPages >= 15)
            {
                this.uxPageCount.Content = $"Max of Pages {printedPages} Reached for Print Preview. Select Print to PDF for the full report.";

                this.uxPageCount.FontSize = 15;

                this.uxPageCount.Foreground = Brushes.Red;

                this.uxPageCount.FontWeight = FontWeights.Bold;
            }
            else
			{
                this.uxPageCount.Content = $"{printedPages} Pages";
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
