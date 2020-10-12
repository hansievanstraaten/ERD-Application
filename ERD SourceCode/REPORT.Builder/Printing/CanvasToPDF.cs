using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using WPF.Tools.Functions;

namespace REPORT.Builder.Printing
{
    public class CanvasToPDF
    {
        public string ConvertToPdf(string outputFile, Dictionary<int, PrintCanvas> pages)
        {
            string formatedFileName = this.ChangeFilenameToXPS(this.PreparePdfFileName(outputFile));

            try
            {
                FixedDocument fixedDoc = new FixedDocument();

                foreach (KeyValuePair<int, PrintCanvas> pageKeyPair in pages)
                {
                    Size size = new Size(pageKeyPair.Value.Width, pageKeyPair.Value.Height);

                    PageContent pageContent = new PageContent();

                    FixedPage page = new FixedPage();

                    FixedPage.SetLeft(pageKeyPair.Value, 0);

                    FixedPage.SetTop(pageKeyPair.Value, 0);

                    page.Width = size.Width;

                    page.Height = size.Height;

                    page.Children.Add(pageKeyPair.Value);

                    page.Measure(size);

                    page.Arrange(new Rect(0, 0, size.Width, size.Height));

                    page.UpdateLayout();

                    fixedDoc.Pages.Add(pageContent);

                    ((System.Windows.Markup.IAddChild)pageContent).AddChild(page);
                }

                using XpsDocument xpsDocument = new XpsDocument(formatedFileName, FileAccess.ReadWrite);

                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                writer.Write(fixedDoc);

                xpsDocument.Close();

                PdfSharp.Xps.XpsModel.XpsDocument pdfDocument = PdfSharp.Xps.XpsModel.XpsDocument.Open(formatedFileName);

                PdfSharp.Xps.XpsConverter.Convert(pdfDocument, Path.ChangeExtension(formatedFileName, "pdf"), 0);
            
                this.DeleteXpsFile(formatedFileName);
                
                pdfDocument.Close();

                return Path.ChangeExtension(formatedFileName, "pdf");
            }
            catch (Exception err)
            {
                throw;
            }
        }
    
        private string PreparePdfFileName(string outputFile)
        {
            if (Path.GetExtension(outputFile) != ".pdf")
            {
                outputFile = $"{outputFile}.pdf";
            }

            string formatedFileName = outputFile;

            int fileNameIndex = 1;

            while (File.Exists(formatedFileName))
            {
                string fileName = Path.GetFileNameWithoutExtension(outputFile);

                string fileDirectory = Path.GetDirectoryName(outputFile);

                formatedFileName = Path.Combine(fileDirectory, $"{fileName}({fileNameIndex}).pdf");

                ++fileNameIndex;
            }

            return formatedFileName;
        }
    
        private string ChangeFilenameToXPS(string filePath)
        {
            string formatedFileName = Path.ChangeExtension(filePath, "xps");

            if (File.Exists(formatedFileName))
            {
                File.Delete(formatedFileName);
            }

            while (File.Exists(formatedFileName))
            {
                Sleep.ThreadWait(100);
            }

            return formatedFileName;
        }
    
        private async void DeleteXpsFile(string fileName)
        {
            await Task.Factory.StartNew(() => 
            {
                RETRY:

                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    goto RETRY;
                }
            });
        }
    }
}
