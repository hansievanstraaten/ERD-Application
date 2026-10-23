using GeneralExtensions;
using MdXaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace REPORT.Builder.Helpers
{
    public class DocumentHelper
    {
        public static void ShowMarkDownDocument(string documentText)
        {
            try
            {
                Markdown markdown = new Markdown();
                FlowDocument flowDocument = markdown.Transform(documentText);

                // STEP 1: Read heading levels and text from the original Markdown.
                List<DocumentHeading> headings = documentText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(line => Regex.Match(line, @"^\s{0,3}(#{1,6})\s+(.+?)\s*#*\s*$"))
                    .Where(match => match.Success)
                    .Select(match => new DocumentHeading
                    {
                        Level = match.Groups[1].Length,
                        Text = NormalizeHeadingText(match.Groups[2].Value)
                    })
                    .ToList();

                foreach (Paragraph paragraph in flowDocument.Blocks.OfType<Paragraph>())
                {
                    string paragraphText = NormalizeHeadingText(new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text);

                    DocumentHeading heading = headings.FirstOrDefault(h => h.Text == paragraphText);

                    if (heading == null)
                        continue;

                    // STEP 3: Apply formatting according to the Markdown level.
                    switch (heading.Level)
                    {
                        case 1:
                            paragraph.FontSize = 20;
                            break;

                        case 2:
                            paragraph.FontSize = 16;
                            break;

                        case 3:
                            paragraph.FontSize = 14;
                            break;

                        default:
                            paragraph.FontSize = 12;
                            break;
                    }

                    paragraph.FontWeight = FontWeights.Bold;
                    paragraph.Margin = new Thickness(0, 12, 0, 6);
                    paragraph.KeepWithNext = true;
                }

                flowDocument.FontFamily = new FontFamily("Arial");
                flowDocument.FontSize = 12;
                flowDocument.PageWidth = 816;
                flowDocument.PageHeight = 1056;
                flowDocument.PagePadding = new Thickness(60);
                flowDocument.ColumnWidth = double.PositiveInfinity;

                Style heading1 = new Style(typeof(Paragraph));
                heading1.Setters.Add(new Setter(Paragraph.FontSizeProperty, 18.0));
                heading1.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
                heading1.Setters.Add(new Setter(Paragraph.MarginProperty,new Thickness(0, 16, 0, 8)));
                heading1.Setters.Add(new Setter(Paragraph.KeepWithNextProperty, true));

                Style heading2 = new Style(typeof(Paragraph));
                heading2.Setters.Add(new Setter(Paragraph.FontSizeProperty, 15.0));
                heading2.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
                heading2.Setters.Add(new Setter(Paragraph.MarginProperty,new Thickness(0, 12, 0, 6)));
                heading2.Setters.Add(new Setter(Paragraph.KeepWithNextProperty, true));

                Style heading3 = new Style(typeof(Paragraph));
                heading3.Setters.Add(new Setter(Paragraph.FontSizeProperty, 13.0));
                heading3.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
                heading3.Setters.Add(new Setter(Paragraph.MarginProperty,new Thickness(0, 8, 0, 4)));
                heading3.Setters.Add(new Setter(Paragraph.KeepWithNextProperty, true));

                // Register styles for use in the document
                flowDocument.Resources.Add("Heading1", heading1);
                flowDocument.Resources.Add("Heading2", heading2);
                flowDocument.Resources.Add("Heading3", heading3);

                string xpsPath = Path.Combine(Path.GetTempPath(), $"ERD-{Guid.NewGuid():N}.xps");

                using (XpsDocument xps = new XpsDocument(xpsPath, FileAccess.ReadWrite))
                {
                    DocumentPaginator paginator11 = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;

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


        private static string NormalizeHeadingText(string text)
        {
            // Remove common inline Markdown formatting markers.
            text = Regex.Replace(text, @"\*\*(.*?)\*\*", "$1");
            text = Regex.Replace(text, @"__(.*?)__", "$1");
            text = Regex.Replace(text, @"(?<!\*)\*([^*]+)\*(?!\*)", "$1");
            text = Regex.Replace(text, @"(?<!_)_([^_]+)_(?!_)", "$1");
            text = Regex.Replace(text, @"`([^`]+)`", "$1");

            return Regex.Replace(text.Trim(), @"\s+", " ");
        }

        public class DocumentHeading
        {
            public int Level { get; set; }
            public string Text { get; set; }
        }

    }
}





