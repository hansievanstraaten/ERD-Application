using GeneralExtensions;
using MdXaml;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace REPORT.Builder.Helpers
{
    public class DocumentHelper
    {
        public static void ShowDocument(string documentText)
        {
            try
            {
                Markdown markdown = new Markdown();
                FlowDocument flowDocument = markdown.Transform(documentText);

                flowDocument.FontFamily = new FontFamily("Arial");
                flowDocument.FontSize = 12;
                flowDocument.PageWidth = 816;
                flowDocument.PageHeight = 1056;
                flowDocument.PagePadding = new Thickness(60);
                flowDocument.ColumnWidth = double.PositiveInfinity;

                string xpsPath = Path.Combine(Path.GetTempPath(), $"ERD-{Guid.NewGuid():N}.xps");

                using (var xps = new XpsDocument(xpsPath, FileAccess.ReadWrite))
                {
                    var paginator11 = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;

                    XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator11);
                }

                using (XpsDocument xps = new XpsDocument(xpsPath, FileAccess.Read))
                {
                    FixedDocumentSequence sequence = xps.GetFixedDocumentSequence();

                    DocumentViewer viewer = new DocumentViewer
                    {
                        Document = sequence
                    };

                    Window window = new Window
                    {
                        Title = "XPS Test",
                        Width = 900,
                        Height = 700,
                        Content = viewer
                    };

                    window.Show();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}





