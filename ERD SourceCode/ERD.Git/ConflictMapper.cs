using ERD.Git.Enums;
using ERD.Git.Models.ConflictMapperModels;
using Quibble.CSharp;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ERD.Git
{
    public class ConflictMapper
    {
        int incommingReadIndex = 0;
        int localReadIndex = 0;

        public delegate void ProgressChangedEvent(object sender, int max, int progress);
        public delegate void ParagraphClickedEvent(object sender, MouseButtonEventArgs e);
        public event ProgressChangedEvent? ProgressChanged;
        public event ParagraphClickedEvent? ParagraphClicked;
        private MappedFileModel incomingMap = new MappedFileModel();
        private MappedFileModel localMap = new MappedFileModel();
        private MappedFileModel mergeMap = new MappedFileModel();
        private static readonly Regex JsonTokenRegex =
            new Regex(@"(?<property>""(?:\\.|[^""\\])*"")(?=\s*:)|(?<string>""(?:\\.|[^""\\])*"")|(?<number>-?\d+(?:\.\d+)?(?:[eE][+-]?\d+)?)|(?<literal>\b(?:true|false|null)\b)", RegexOptions.Compiled);

        public (MappedFileModel incomingMap, MappedFileModel localMap, MappedFileModel mergeMap) Map(string incomingJsonString, string localJsonString)
        {
            JsonNode? incoming = JsonNode.Parse(incomingJsonString);
            JsonNode? local = JsonNode.Parse(localJsonString);

            JsonLineDocument incomingDocument = JsonLineDocument.Create(incoming);
            JsonLineDocument localDocument = JsonLineDocument.Create(local);

            string[] incomingLines = incomingDocument.Lines.ToArray();
            string[] localLines = localDocument.Lines.ToArray();

            IReadOnlyList<Diff> differences = JsonStrings.Diff(incomingJsonString, localJsonString);

            Dictionary<int, DiffItem> incomingDiffLocations = new();
            Dictionary<int, DiffItem> localDiffLocations = new();

            foreach (Diff diff in differences)
            {
                string path = diff.Path.ToString();

                if (diff.IsArrayDiff)
                {
                    ArrayDiff arrayDiff = (ArrayDiff)diff;

                    foreach (ItemMismatch mismatch in arrayDiff.Mismatches)
                    {
                        switch (mismatch)
                        {
                            case LeftOnlyItem leftOnlyItem:
                                {
                                    string nodePath = $"{path}[{leftOnlyItem.ItemIndex}]";
                                    MarkNode(nodePath, incomingDocument, incomingDiffLocations, FileLocationEnum.Incoming);
                                    break;
                                }

                            case RightOnlyItem rightOnlyItem:
                                {
                                    string nodePath = $"{path}[{rightOnlyItem.ItemIndex}]";
                                    MarkNode(nodePath, localDocument, localDiffLocations, FileLocationEnum.Local);
                                    break;
                                }

                            default:
                                throw new InvalidOperationException($"Unknown mismatch type: {mismatch.GetType().FullName}");
                        }
                    }
                }
                else
                {
                    var tt = diff.GetType().ToString();

                    MarkNode(path, incomingDocument, incomingDiffLocations, FileLocationEnum.Incoming);
                    MarkNode(path, localDocument, localDiffLocations, FileLocationEnum.Local);
                }
            }

            this.BuildMap(incomingLines, localLines, incomingDiffLocations, localDiffLocations);

            return (this.incomingMap, this.localMap, this.mergeMap);
        }

        private void MarkNode(string path, JsonLineDocument document, Dictionary<int, DiffItem> locations, FileLocationEnum fileLocation)
        {
            if (!document.NodeRanges.TryGetValue(path, out var range))
            {
                throw new InvalidOperationException($"Could not map JSON path '{path}' to formatted lines.");
            }

            for (int index = range.Start; index <= range.End; index++)
            {
                locations.TryAdd(index, new DiffItem
                {
                    Index = index,
                    Value = document.Lines[index],
                    FileLocation = fileLocation
                });
            }
        }

        private void BuildMap(string[] incomingLines, string[] localLines, Dictionary<int, DiffItem> incomingDiffLocations, Dictionary<int, DiffItem> localDiffLocations)
        {
            int maxIndex = Math.Max(incomingLines.Length, localLines.Length) + incomingDiffLocations.Count + localDiffLocations.Count;
            int executingIndex = 0;

            while (executingIndex < maxIndex)
            {
                try
                {
                    if (incomingDiffLocations.ContainsKey(this.incommingReadIndex)
                     && incomingDiffLocations[this.incommingReadIndex].FileLocation == FileLocationEnum.Incoming)
                    {
                        while (incomingDiffLocations.ContainsKey(this.incommingReadIndex))
                        {
                            this.LinemapIncomming(incomingDiffLocations, incomingLines, executingIndex);

                            ++executingIndex;
                            ++this.incommingReadIndex;
                        }
                    }
                    else if (localDiffLocations.ContainsKey(this.localReadIndex)
                     && localDiffLocations[this.localReadIndex].FileLocation == FileLocationEnum.Local)
                    {
                        while (localDiffLocations.ContainsKey(this.localReadIndex))
                        {
                            this.LinemapLocal(localDiffLocations, localLines, executingIndex);

                            ++executingIndex;
                            ++this.localReadIndex;
                        }
                    }
                    else
                    {
                        #region CREATE BOTH LINE MAPPINGS

                        LineMapping incomingLineMap = new LineMapping
                        {
                            LineIndex = executingIndex,
                            Text = incomingLines.Length > this.incommingReadIndex ? incomingLines[this.incommingReadIndex] : null
                        };

                        incomingMap.Lines.Add(executingIndex, incomingLineMap);

                        LineMapping localLineMap = new LineMapping
                        {
                            LineIndex = executingIndex,
                            Text = localLines.Length > this.localReadIndex ? localLines[this.localReadIndex] : null
                        };

                        localMap.Lines.Add(executingIndex, localLineMap);

                        LineMapping mergeLineMap = new LineMapping
                        {
                            LineIndex = executingIndex,
                            Text = localLines.Length > this.localReadIndex ? localLines[this.localReadIndex] : null
                        };

                        this.mergeMap.Lines.Add(executingIndex, mergeLineMap);

                        #endregion

                        ++this.incommingReadIndex;
                        ++this.localReadIndex;
                    }

                    this.ProgressChanged?.Invoke(this, maxIndex, executingIndex);
                }
                catch (Exception ex)
                {
                    throw;
                }

                ++executingIndex;
            }
        }

        private void LinemapIncomming(Dictionary<int, DiffItem> incomingDiffLocations, string[] incomingLines, int executingIndex)
        {
            DiffItem diffItem = incomingDiffLocations[this.incommingReadIndex];

            LineMapping incomingLineMap = new LineMapping
            {
                LineIndex = executingIndex,
                Text = incomingLines[this.incommingReadIndex],
                Diff = diffItem,
                IsSelected = false
            };

            this.incomingMap.Lines.Add(executingIndex, incomingLineMap);

            LineMapping localLineMap = new LineMapping
            {
                LineIndex = executingIndex,
                Text = null
            };

            this.localMap.Lines.Add(executingIndex, localLineMap);

            LineMapping mergeLineMap = new LineMapping
            {
                LineIndex = executingIndex,
                Text = null,
                Diff = diffItem,
                IsSelected = false
            };

            this.mergeMap.Lines.Add(executingIndex, mergeLineMap);
        }

        private void LinemapLocal(Dictionary<int, DiffItem> localDiffLocations, string[] localLines, int executingIndex)
        {
            DiffItem diffItem = localDiffLocations[this.localReadIndex];

            LineMapping incomingLineMap = new LineMapping
            {
                LineIndex = executingIndex,
                Text = null
            };

            incomingMap.Lines.Add(executingIndex, incomingLineMap);

            LineMapping localLineMap = new LineMapping
            {
                LineIndex = executingIndex,
                Text = localLines[this.localReadIndex],
                Diff = diffItem
            };

            localMap.Lines.Add(executingIndex, localLineMap);

            LineMapping mergeLineMap = new LineMapping
            {
                LineIndex = executingIndex,
                Text = localLines[this.localReadIndex],
                Diff = diffItem
            };

            this.mergeMap.Lines.Add(executingIndex, mergeLineMap);
        }

        public FlowDocument BuildTextDocument(MappedFileModel mapedFile, bool addClickEvent = true)
        {
            FlowDocument document = new FlowDocument();

            foreach (LineMapping line in mapedFile.Lines.Values)
            {
                Paragraph paragraph = new Paragraph
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(2, 0, 2, 0)
                };

                ConflictMapper.AddJsonHighlighting(paragraph, line.Text ?? string.Empty);

                if (line.Diff != null)
                {
                    paragraph.Tag = line;

                    if (addClickEvent)
                    {
                        paragraph.Background = line.IsSelected
                            ? Brushes.LightGreen
                            : Brushes.LightCoral;

                        paragraph.MouseLeftButtonDown += this.Paragraph_Clicked;
                    }
                }

                document.Blocks.Add(paragraph);
            }

            return document;
        }

        public static void AddJsonHighlighting(Paragraph paragraph, string text)
        {
            int position = 0;

            foreach (Match match in JsonTokenRegex.Matches(text))
            {
                // Preserve uncoloured text before the token.
                if (match.Index > position)
                {
                    paragraph.Inlines.Add(new Run(text.Substring(position, match.Index - position)));
                }

                Brush colour =
                    match.Groups["property"].Success ? Brushes.Firebrick :
                    match.Groups["string"].Success ? Brushes.RoyalBlue :
                    match.Groups["number"].Success ? Brushes.DarkGreen :
                    match.Groups["literal"].Success ? Brushes.Purple :
                    Brushes.Black;

                paragraph.Inlines.Add(new Run(match.Value)
                {
                    Foreground = colour
                });

                position = match.Index + match.Length;
            }

            // Preserve the remaining text.
            if (position < text.Length)
            {
                paragraph.Inlines.Add(new Run(text.Substring(position)));
            }
        }
        
        private void Paragraph_Clicked(object sender, MouseButtonEventArgs e)
        {
            this.ParagraphClicked?.Invoke(sender, e);
        }

    }
}
