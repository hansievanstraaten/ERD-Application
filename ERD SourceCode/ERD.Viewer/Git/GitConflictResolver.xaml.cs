using ERD.Common;
using ERD.Git;
using ERD.Git.Models.ConflictMapperModels;
using ERD.Viewer.Tools.Common;
using GeneralExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ViSo.Dialogs.Input;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Helpers;
using System.Text.RegularExpressions;

namespace ERD.Viewer.Comparer
{
    /// <summary>
    /// Interaction logic for GitConflictResolver.xaml
    /// </summary>
    public partial class GitConflictResolver : WindowBase
    {
        private int lastKnownIndex = -1;
        private int conflictIndex = -1;
        private bool isSyncingScroll;
        private string conflictFileName = string.Empty;
        private ProgressResponce progress = null;
        private Dictionary<int, string> localJson = new Dictionary<int, string>();
        private Dictionary<int, string> incomingJson = new Dictionary<int, string>();
        //private static readonly Regex JsonTokenRegex =
        //    new Regex(@"(?<property>""(?:\\.|[^""\\])*"")(?=\s*:)|(?<string>""(?:\\.|[^""\\])*"")|(?<number>-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)|(?<literal>\b(?:true|false|null)\b)", RegexOptions.Compiled);

        public GitConflictResolver()
        {
            this.InitializeComponent();

            this.InitializeFiles();
        }

        public bool Resolved { get; private set; } = false;

        private async void Conflict_Selected(object sender, MouseButtonEventArgs e)
        {
            this.localJson.Clear();
            this.incomingJson.Clear();

            LableItem item = (LableItem)sender;
            this.conflictFileName = item.Content.ToString();

            //LableItem selectedConflict = this.uxConflicts.Children.OfType<LableItem>().FirstOrDefault(l => l.Content.ToString() == this.conflictFileName);
            //this.uxConflicts.Children.Remove(selectedConflict);
            foreach(LableItem child in this.uxConflicts.Children)
            {
                child.FontWeight = this.conflictFileName.Equals(child.Content.ToString(), StringComparison.CurrentCultureIgnoreCase) ? FontWeights.Bold : FontWeights.Normal;
            }

            await this.LoadConflict();
        }

        private void MapperProgress_Changed(object sender, int max, int progress)
        {
            int percentage = (int)((double)progress / max * 100);

            this.Dispatcher.Invoke(() =>
            {
                this.progress.UpdateMessage($"Mapping progress: {percentage}%");
                this.progress.UpdateProgress(percentage);
            });
        }

        private void Paragraph_Clicked(object sender, MouseButtonEventArgs e)
        {
            Paragraph paragraph = (Paragraph)sender;
            this.ActionParagraphMerge(paragraph);

            e.Handled = true;
        }

        private void RichTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (isSyncingScroll)
                return;

            if (e.VerticalChange == 0 && e.HorizontalChange == 0)
                return;

            RichTextBox source = (RichTextBox)sender;

            ScrollViewer? sourceScrollViewer = WPFHelpers.FindVisualChild<ScrollViewer>(source);

            if (sourceScrollViewer is null)
                return;

            try
            {
                isSyncingScroll = true;

                foreach (RichTextBox target in new[]
                {
                    this.uxIncoming,
                    this.uxLocal,
                    this.uxMerged
                })
                {
                    if (target == source)
                        continue;

                    ScrollViewer? targetScrollViewer = WPFHelpers.FindVisualChild<ScrollViewer>(target);

                    if (targetScrollViewer is null)
                        continue;

                    targetScrollViewer.ScrollToVerticalOffset(sourceScrollViewer.VerticalOffset);
                    targetScrollViewer.ScrollToHorizontalOffset(sourceScrollViewer.HorizontalOffset);
                }
            }
            finally
            {
                isSyncingScroll = false;
            }
        }

        private void Next_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Paragraph? current = this.uxMerged.Document.Blocks
                .OfType<Paragraph>()
                .FirstOrDefault(p => p.Tag is LineMapping
                         && ((LineMapping)p.Tag).LineIndex > this.lastKnownIndex);

                current?.BringIntoView();

                SetAllParagraphMarkers(current);

                this.lastKnownIndex = current is null ? this.lastKnownIndex : ((LineMapping)current.Tag).LineIndex;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void Back_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Paragraph? current = this.uxMerged.Document.Blocks
                .OfType<Paragraph>()
                .LastOrDefault(p => p.Tag is LineMapping
                         && ((LineMapping)p.Tag).LineIndex < this.lastKnownIndex);

                current?.BringIntoView();

                SetAllParagraphMarkers(current);

                this.lastKnownIndex = current is null ? this.lastKnownIndex : ((LineMapping)current.Tag).LineIndex;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private async void AcceptMerge_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ClearAllMarkers();

                TextRange range = new TextRange(this.uxMerged.Document.ContentStart, this.uxMerged.Document.ContentEnd);

                JObject jsonObject = JObject.Parse(range.Text);

                string fullText = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.None);

                int excludedIncome = this.uxIncoming.Document.Blocks
                    .OfType<Paragraph>()
                    .Where(p => p.Tag is LineMapping line && !line.IsSelected)
                    .Select(p => ((LineMapping)p.Tag).LineIndex)
                    .Count();

                int excludedLocal = this.uxLocal.Document.Blocks
                    .OfType<Paragraph>()
                    .Where(p => p.Tag is LineMapping line && !line.IsSelected)
                    .Select(p => ((LineMapping)p.Tag).LineIndex)
                    .Count();

                string message = $"Not Merged.\n\nExcluded Lines:\nIncoming: {excludedIncome}\nLocal: {excludedLocal}\n\nDo you want o continue?";

                if (MessageBox.Show(message,"Continue", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }

                this.ReplaceConflict(fullText);

                if (this.localJson.Keys.Count() > 1)
                {
                    await this.LoadConflict();
                }
                else
                {
                    LableItem selectedConflict = this.uxConflicts.Children.OfType<LableItem>().FirstOrDefault(l => l.Content.ToString() == this.conflictFileName);
                    this.uxConflicts.Children.Remove(selectedConflict);

                    if (this.uxConflicts.Children.Count == 0)
                    {
                        this.Resolved = true;
                        this.Close();
                    }
                }

                this.uxSelectIncoming.IsChecked = false;
                this.uxSelectLocal.IsChecked = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }   
        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SelectAllSelections(sender, true);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SelectAllSelections(sender, false);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
        
        private async Task LoadConflict()
        {
            string filePath = Path.Combine(General.ProjectModel.FileDirectory, this.conflictFileName);
            string[] lines = File.ReadAllLines(filePath);

            for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
            {
                string lineItem = lines[lineNumber];

                if (!lineItem.Contains(ERDGit.HEAD))
                {
                    continue;
                }

                StringBuilder localString = new StringBuilder();
                StringBuilder incomingString = new StringBuilder();

                ++lineNumber;

                while (!lines[lineNumber].StartsWith(ERDGit.MERGE)
                    && !lines[lineNumber].StartsWith(ERDGit.Base))
                {
                    localString.AppendLine(lines[lineNumber]);
                    ++lineNumber;
                }

                while (!lines[lineNumber].StartsWith(ERDGit.MERGE))
                {
                    ++lineNumber;
                }

                ++lineNumber;

                while (!lines[lineNumber].StartsWith(ERDGit.END))
                {
                    incomingString.AppendLine(lines[lineNumber]);
                    ++lineNumber;
                }

                this.localJson.Add(lineNumber, localString.ToString());
                this.incomingJson.Add(lineNumber, incomingString.ToString());

                lineNumber += 4;
            }

            this.conflictIndex = this.localJson.Keys.Min();

            this.progress = new ProgressResponce(100);

            await this.LoadComparison(this.conflictIndex);
        }

        private async Task LoadComparison(int index)
        {

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            this.progress.Show();
                            this.progress.UpdateMessage($"Reading Files. This may take a minute or two");
                        });

                        string incomingJsonString = incomingJson[index];
                        string localJsonString = localJson[index];

                        ConflictMapper mapper = new ConflictMapper();

                        mapper.ProgressChanged += this.MapperProgress_Changed;
                        mapper.ParagraphClicked += this.Paragraph_Clicked;

                        var (incomingMap, localMap, mergeMap) = mapper.Map(incomingJsonString, localJsonString);

                        this.Dispatcher.Invoke(() =>
                        {
                            //this.uxIncoming.Document = this.BuildTextDocument(incomingMap);
                            //this.uxLocal.Document = this.BuildTextDocument(localMap);
                            //this.uxMerged.Document = this.BuildTextDocument(mergeMap, false);
                            this.uxIncoming.Document = mapper.BuildTextDocument(incomingMap);
                            this.uxLocal.Document = mapper.BuildTextDocument(localMap);
                            this.uxMerged.Document = mapper.BuildTextDocument(mergeMap, false);

                            int conflictCount = incomingMap.Lines
                            .Where(c => c.Value.Diff != null)
                            .Count();

                            this.uxConflictCount.Content = $"Conflicts Found: {conflictCount}";
                        });
                    }
                    catch (Exception err)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(err.InnerExceptionMessage());
                        });
                    }
                    finally
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            this.progress.Close();
                        });
                    }
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        //private FlowDocument BuildTextDocument(MappedFileModel mapedFile, bool addClickEvent = true)
        //{

        //    FlowDocument document = new FlowDocument();

        //    foreach (LineMapping line in mapedFile.Lines.Values)
        //    {
        //        Paragraph paragraph = new Paragraph
        //        {
        //            Margin = new Thickness(0),
        //            Padding = new Thickness(2, 0, 2, 0)
        //        };

        //        this.AddJsonHighlighting(paragraph, line.Text ?? string.Empty);

        //        if (line.Diff != null)
        //        {
        //            paragraph.Tag = line;

        //            if (addClickEvent)
        //            {
        //                paragraph.Background = line.IsSelected
        //                    ? Brushes.LightGreen
        //                    : Brushes.LightCoral;

        //                paragraph.MouseLeftButtonDown += this.Paragraph_Clicked;
        //            }
        //        }

        //        document.Blocks.Add(paragraph);
        //    }

        //    return document;
        //}
                
        //private void AddJsonHighlighting(Paragraph paragraph, string text)
        //{
        //    int position = 0;

        //    foreach (Match match in JsonTokenRegex.Matches(text))
        //    {
        //        // Preserve uncoloured text before the token.
        //        if (match.Index > position)
        //        {
        //            paragraph.Inlines.Add(new Run(text.Substring(position, match.Index - position)));
        //        }

        //        Brush colour =
        //            match.Groups["property"].Success ? Brushes.Firebrick :
        //            match.Groups["string"].Success ? Brushes.RoyalBlue :
        //            match.Groups["number"].Success ? Brushes.DarkGreen :
        //            match.Groups["literal"].Success ? Brushes.Purple :
        //            Brushes.Black;

        //        paragraph.Inlines.Add(new Run(match.Value)
        //        {
        //            Foreground = colour
        //        });

        //        position = match.Index + match.Length;
        //    }

        //    // Preserve the remaining text.
        //    if (position < text.Length)
        //    {
        //        paragraph.Inlines.Add(new Run(text.Substring(position)));
        //    }
        //}

        private void SelectAllSelections(object sender,bool preSelectionDefault)
        {
            CheckBoxItem source = (CheckBoxItem)sender;

            if (source.Name == nameof(this.uxSelectIncoming))
            {
                if (this.uxIncoming == null)
                {   // This is null when window is loading
                    return;
                }

                List<Paragraph> paragraphs = this.uxIncoming.Document.Blocks.OfType<Paragraph>()
                    .Where(p => p.Tag != null && p.Tag is LineMapping line)
                    .ToList();

                foreach (Paragraph paragraph in paragraphs)
                {
                    ((LineMapping)paragraph.Tag).IsSelected = preSelectionDefault;
                    this.ActionParagraphMerge(paragraph);
                }
            }
            else if (source.Name == nameof(this.uxSelectLocal))
            {
                if (this.uxLocal == null)
                {   // This is null when window is loading
                    return;
                }

                List<Paragraph> paragraphs = this.uxLocal.Document.Blocks.OfType<Paragraph>()
                    .Where(p => p.Tag != null && p.Tag is LineMapping line)
                    .ToList();

                foreach (Paragraph paragraph in paragraphs)
                {
                    ((LineMapping)paragraph.Tag).IsSelected = preSelectionDefault;
                    this.ActionParagraphMerge(paragraph);
                }
            }
        }

        private void ReplaceConflict(string mergeFile)
        {
            if (InputBox.ShowDialog("Email", "Your email address").IsFalse())
            {
                return;
            }

            string filePath = Path.Combine(General.ProjectModel.FileDirectory, this.conflictFileName);
            string[] lines = File.ReadAllLines(filePath);

            StringBuilder mergedFile = new StringBuilder();
            bool mergeCompleted = false;

            for (int lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
            {
                string lineItem = lines[lineNumber];

                if (!lineItem.Contains(ERDGit.HEAD)
                  || mergeCompleted)
                {
                    mergedFile.AppendLine(lineItem);
                    continue;
                }
                else
                {
                    mergedFile.AppendLine(mergeFile);
                    while (!lines[lineNumber].StartsWith(ERDGit.END))
                    {
                        ++lineNumber;
                    }
                    mergeCompleted = true;
                }
            }

            string safeFilePaht = Path.Combine(General.ProjectModel.FileDirectory, this.conflictFileName);

            File.WriteAllText(safeFilePaht, mergedFile.ToString());

            ERDGit.Stage(General.ProjectModel.FileDirectory, this.conflictFileName);

            ERDGit.Commit(General.ProjectModel.FileDirectory, new ERD.Git.Models.GitModels.GitCommitRequest
            {
                CommitMessage = "Merge",
                Email = InputBox.Result,
            });
        }

        private void ActionParagraphMerge(Paragraph paragraph)
        {
            LineMapping line = (LineMapping)paragraph.Tag;

            line.IsSelected = !line.IsSelected;

            paragraph.Background = line.IsSelected
            ? Brushes.LightGreen
            : Brushes.LightCoral;

            this.UpdateMergedParagraph(line);
        }
        
        private void UpdateMergedParagraph(LineMapping line)
        {
            foreach (Paragraph mergedParagraph in this.uxMerged.Document.Blocks
                .OfType<Paragraph>()
                .Where(p => p.Tag is LineMapping))
            {
                if (mergedParagraph.Tag is not LineMapping mergedLine)
                    continue;

                if (mergedLine.LineIndex != line.LineIndex)
                    continue;

                mergedParagraph.Inlines.Clear();

                if (line.IsSelected)
                    ConflictMapper.AddJsonHighlighting(mergedParagraph, line.Text ?? string.Empty);

                //mergedParagraph.Inlines.Add(new Run(line.Text ?? string.Empty));

                mergedParagraph.Background = line.IsSelected && line.Diff.FileLocation == ERD.Git.Enums.FileLocationEnum.Incoming
                            ? Brushes.LightSteelBlue
                            : line.IsSelected == false && line.Diff.FileLocation == ERD.Git.Enums.FileLocationEnum.Local
                            ? Brushes.LightSteelBlue
                            : null;

                this.lastKnownIndex = line.LineIndex;
                this.SetAllParagraphMarkers(mergedParagraph);

                break;
            }
        }

        private void SetAllParagraphMarkers(Paragraph? paragraph)
        {
            if (paragraph is null)
                return;

            this.ClearAllMarkers();

            Paragraph? localParagraph = this.uxLocal.Document.Blocks
                .OfType<Paragraph>()
                .FirstOrDefault(p => p.Tag is LineMapping line && line.LineIndex == ((LineMapping)paragraph.Tag).LineIndex);

            Paragraph? incomingParagraph = this.uxIncoming.Document.Blocks
                .OfType<Paragraph>()
                .FirstOrDefault(p => p.Tag is LineMapping line && line.LineIndex == ((LineMapping)paragraph.Tag).LineIndex);

            this.InsertMarker(paragraph);
            this.InsertMarker(localParagraph);
            this.InsertMarker(incomingParagraph);
        }

        private void ClearAllMarkers()
        {
            List<Paragraph> mergeMarkers = this.uxMerged.Document.Blocks
                .OfType<Paragraph>()
                .Where(p => p.Inlines.FirstInline is Run run && run.Tag?.ToString() == "SelectionMarker")
                .ToList();

            List<Paragraph> incommingMarkers = this.uxIncoming.Document.Blocks
                .OfType<Paragraph>()
                .Where(p => p.Inlines.FirstInline is Run run && run.Tag?.ToString() == "SelectionMarker")
                .ToList();

            List<Paragraph> localMarkers = this.uxLocal.Document.Blocks
                .OfType<Paragraph>()
                .Where(p => p.Inlines.FirstInline is Run run && run.Tag?.ToString() == "SelectionMarker")
                .ToList();

            this.RemoveMarkers(mergeMarkers);
            this.RemoveMarkers(incommingMarkers);
            this.RemoveMarkers(localMarkers);
        }

        private void InsertMarker(Paragraph? paragraph)
        {
            if (paragraph is null)
                return;

            if (paragraph.Inlines.FirstInline != null)
                paragraph.Inlines.InsertBefore(
                    paragraph.Inlines.FirstInline,
                    new Run("▶ ")
                    {
                        Foreground = Brushes.DodgerBlue,
                        FontWeight = FontWeights.Bold,
                        Tag = "SelectionMarker"
                    });
        }

        private void RemoveMarkers(List<Paragraph> markers)
        {
            foreach (Paragraph p in markers)
            {
                if (p.Tag is not LineMapping line)
                    continue;

                // Remove the marker from all paragraphs.
                if (p.Inlines.FirstInline is Run run &&
                    run.Tag?.ToString() == "SelectionMarker")
                {
                    p.Inlines.Remove(run);
                }
            }
        }

        private void InitializeFiles()
        {
            List<string?> filesList = ERDGit.GetConflicts(General.ProjectModel.FileDirectory);

            foreach (string item in filesList)
            {
                LableItem lable = new LableItem()
                {
                    Content = item
                };

                lable.MouseUp += this.Conflict_Selected;

                this.uxConflicts.Children.Add(lable);
            }
        }
    }
}
