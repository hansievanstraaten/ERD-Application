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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using static System.Windows.Forms.LinkLabel;

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

        public static void ShowMarkDownDocument(string documentText)
        {
            try
            {
                Markdown markdown = new Markdown();
                FlowDocument flowDocument = markdown.Transform(documentText);

                List<DocumentHeading> headings = documentText
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(line => Regex.Match(line, @"^\s{0,3}(#{1,6})\s+(.+?)\s*#*\s*$"))
                    .Where(m => m.Success)
                    .Select(m => new DocumentHeading
                    {
                        Level = m.Groups[1].Length,
                        Text = NormalizeHeadingText(m.Groups[2].Value)
                    })
                    .ToList();

                flowDocument.FontFamily = new FontFamily("Arial");
                flowDocument.FontSize = 12;
                                
                Dictionary<string, Paragraph> headingTargets = new Dictionary<string, Paragraph>();

                foreach (Paragraph paragraph in GetAllParagraphs(flowDocument))
                {
                    string text = NormalizeHeadingText(new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text);

                    DocumentHeading heading = headings.FirstOrDefault(h => h.Text == text);
                    if (heading == null)
                        continue;

                    paragraph.FontSize = heading.Level switch
                    {
                        1 => 20,
                        2 => 16,
                        3 => 14,
                        _ => 12
                    };

                    paragraph.FontWeight = FontWeights.Bold;
                    paragraph.Margin = new Thickness(0, 12, 0, 6);
                    paragraph.KeepWithNext = true;

                    string slug = CreateHeadingSlug(heading.Text);
                    headingTargets[slug] = paragraph;
                }

                foreach (Paragraph paragraph in GetAllParagraphs(flowDocument))
                {
                    foreach (Hyperlink link in GetAllHyperlinks(paragraph.Inlines))
                    {
                        string linkText = NormalizeHeadingText(new TextRange(link.ContentStart, link.ContentEnd).Text);

                        string slug = CreateHeadingSlug(linkText);

                        if (headingTargets.TryGetValue(slug, out Paragraph target))
                        {
                            link.NavigateUri = null;
                            link.Cursor = Cursors.Hand;
                        }
                    }
                }

                FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer
                {
                    Document = flowDocument,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    IsToolBarVisible = true
                };

                viewer.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler((sender, e) => Hyperlink_MouseLeftButtonUp(sender, e, viewer, headingTargets)), true);

                Window window = new Window
                {
                    Title = "ERD Viewer",
                    Width = 900,
                    Height = 700,
                    Content = viewer
                };

                window.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private static void Hyperlink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e, FlowDocumentScrollViewer viewer, Dictionary<string, Paragraph> headingTargets)
        {
            Point position = e.GetPosition(viewer);

            TextPointer pointer = viewer.Document.ContentStart.GetPositionAtOffset(0);

            if (pointer == null)
                return;

            IInputElement element = viewer.InputHitTest(position);

            if ((element is DependencyObject current) == false)
                return;

            Hyperlink hyperlink = null;

            while (current != null)
            {
                if (current is Hyperlink found)
                {
                    hyperlink = found;
                    break;
                }

                current = GetParentObject(current);
            }

            if (hyperlink == null)
                return;

            string linkText = NormalizeHeadingText(new TextRange(hyperlink.ContentStart, hyperlink.ContentEnd).Text);

            string slug = CreateHeadingSlug(linkText);

            if (headingTargets.TryGetValue(slug, out Paragraph target))
            {
                target.BringIntoView();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentObject(DependencyObject child)
        {
            if (child is ContentElement contentElement)
            {
                return ContentOperations.GetParent(contentElement)
                    ?? (contentElement as FrameworkContentElement)?.Parent;
            }

            return VisualTreeHelper.GetParent(child);
        }

        private static IEnumerable<Paragraph> GetAllParagraphs(FlowDocument document)
        {
            return GetParagraphsFromBlocks(document.Blocks);
        }

        private static IEnumerable<Paragraph> GetParagraphsFromBlocks(BlockCollection blocks)
        {
            foreach (Block block in blocks)
            {
                if (block is Paragraph paragraph)
                {
                    yield return paragraph;
                }
                else if (block is Section section)
                {
                    foreach (Paragraph child in
                             GetParagraphsFromBlocks(section.Blocks))
                        yield return child;
                }
                else if (block is System.Windows.Documents.List list)
                {
                    foreach (ListItem item in list.ListItems)
                    {
                        foreach (Paragraph child in
                                 GetParagraphsFromBlocks(item.Blocks))
                            yield return child;
                    }
                }
                else if (block is Table table)
                {
                    foreach (TableRowGroup group in table.RowGroups)
                    {
                        foreach (TableRow row in group.Rows)
                        {
                            foreach (TableCell cell in row.Cells)
                            {
                                foreach (Paragraph child in
                                         GetParagraphsFromBlocks(cell.Blocks))
                                    yield return child;
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Hyperlink> GetAllHyperlinks(InlineCollection inlines)
        {
            foreach (Inline inline in inlines)
            {
                if (inline is Hyperlink hyperlink)
                    yield return hyperlink;

                if (inline is Span span)
                {
                    foreach (Hyperlink nested in
                             GetAllHyperlinks(span.Inlines))
                        yield return nested;
                }
            }
        }

        private static string CreateHeadingSlug(string text)
        {
            text = NormalizeHeadingText(text).ToLowerInvariant();

            // Remove punctuation but preserve existing hyphens.
            text = Regex.Replace(text, @"[^\w\s-]", "");

            // Convert whitespace to hyphens.
            text = Regex.Replace(text.Trim(), @"\s+", "-");

            return text;
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





