using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
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
        private int pdfBatchSize = 50;

        public string ConvertToPdf(string outputFile, Dictionary<int, PrintCanvas> pages)
        {
            // NOTE: PdfSharp prints only up to 50 pages in the conversion from xps to pdf.
            // In some documentation found it seems that this is a debug flag wrapped in #DEBUG
            // During release build I found that this flag does not work,
            // and I had to implement document merging to build documents with more than 50 pages

            if (pages.Count == 0)
			{
                pages.Add(0, new PrintCanvas {Width = 100, Height = 100 });
			}

            string formatedOutputFile = this.PreparePdfFileName(outputFile);

            string formatedFileName = this.ChangeFilenameToXPS(formatedOutputFile);

            try
            {
                List<string> fileNamesList = new List<string>();

                FixedDocument fixedDoc = new FixedDocument();

                int pageIndex = 0;

                int batchIndex = 1;

                formatedFileName = this.UpdateBatchfileName(formatedFileName, batchIndex);
                
                fileNamesList.Add(formatedFileName);

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

                    ++pageIndex;

                    if (pageIndex == this.pdfBatchSize)
					{
                        this.WriteXpsBatchDocument(batchIndex, formatedFileName, fixedDoc);

                        ++ batchIndex;

                        formatedFileName = this.UpdateBatchfileName(formatedFileName, batchIndex);
                        
                        fileNamesList.Add(formatedFileName);

                        fixedDoc = new FixedDocument();

                        pageIndex = 0;
					}
                }

                if (pageIndex > 0 && pageIndex < this.pdfBatchSize)
				{
                    this.WriteXpsBatchDocument(batchIndex, formatedFileName, fixedDoc);
				}

                List<string> pdfFileNames = this.ConvertXpsBatchesToPdf(fileNamesList, this.PreparePdfFileName(formatedOutputFile));

                this.MergePdfFiles(pdfFileNames, formatedOutputFile);

                return formatedOutputFile;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        private List<string> ConvertXpsBatchesToPdf(List<string> fileNames, string outputPath)
		{
            List<string> pdfFileNames = new List<string>();

            foreach(string file in fileNames)
			{
                PdfSharp.Xps.XpsModel.XpsDocument pdfDocument = PdfSharp.Xps.XpsModel.XpsDocument.Open(file);

                PdfSharp.Xps.XpsConverter.Convert(pdfDocument, Path.ChangeExtension(file, "pdf"), 0);

                this.DeleteXpsFile(file);

                pdfDocument.Close();

                pdfFileNames.Add(Path.ChangeExtension(file, "pdf"));
            }

            return pdfFileNames;
		}

        private void MergePdfFiles(List<string> fileNames, string outputPath)
        {
            PdfDocument outputDocument = new PdfDocument();

            foreach (string file in fileNames)
            {
                PdfDocument inputDocument = PdfReader.Open(file, PdfDocumentOpenMode.Import);

                int count = inputDocument.PageCount;

                for (int idx = 0; idx < count; idx++)
                {
                    PdfPage page = inputDocument.Pages[idx];

                    outputDocument.AddPage(page);
                }
            }

            foreach (string file in fileNames)
			{
                File.Delete(file);
			}

            outputDocument.Save(outputPath);
        }

        private void WriteXpsBatchDocument(int batchNumber, string outputFile, FixedDocument fixedDoc)
		{
            using XpsDocument xpsDocument = new XpsDocument(outputFile, FileAccess.ReadWrite);

            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

            writer.Write(fixedDoc);

            xpsDocument.Close();
        }
    
        private string UpdateBatchfileName(string filename, int batchNumber)
		{
            string result = Path.Combine(Path.GetDirectoryName(filename), $"{Path.GetFileNameWithoutExtension(filename)}.{batchNumber}{Path.GetExtension(filename)}");

            result = result.Replace($"{(batchNumber - 1)}.", string.Empty);

            return result;
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
