using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace REPORT.Builder.Printing
{
    public class CanvasToPDF
    {
        public void ConvertToPdf(string outputFile, Dictionary<int, PrintCanvas> pages)
        {
            if (!outputFile.EndsWith(".xps"))
            {
                outputFile = $"{outputFile}.xps";
            }

            string formatedFileName = outputFile;

            int fileNameIndex = 1;

            //PdfDocument
            while(File.Exists(formatedFileName))
            {
                string fileName = Path.GetFileNameWithoutExtension(outputFile);

                string fileDirectory = Path.GetDirectoryName(outputFile);

                formatedFileName = Path.Combine(fileDirectory, $"{fileName}({fileNameIndex}).xps");

                ++fileNameIndex;
            }

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
        }
    }
}
