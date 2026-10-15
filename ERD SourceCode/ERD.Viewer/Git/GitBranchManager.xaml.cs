using ERD.Common;
using ERD.Git;
using ERD.Git.Models.GitModels;
using GeneralExtensions;
using IconSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Helpers;
using static Microsoft.FSharp.Core.ByRefKinds;

namespace ERD.Viewer.Git
{
    /// <summary>
    /// Interaction logic for GitBranchManager.xaml
    /// </summary>
    public partial class GitBranchManager : WindowBase
    {
        private bool isSyncingScroll = false;
        public GitBranchManager()
        {
            this.InitializeComponent();

            this.InitializeBranchTree();
        }

        public bool BranchChanged { get; private set; } = false;

        private void TreeItem_Clicked(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TreeViewItemTool treeItem = (TreeViewItemTool)sender;
                GitBranchInfo branch = (GitBranchInfo)treeItem.Tag;

                IReadOnlyList<GitCommitInfo> history = ERDGit.GetHistory(General.ProjectModel.FileDirectory, branch.Name);

                this.uxHistory.ItemsSource = history;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void TreeItem_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TreeViewItemTool treeItem = (TreeViewItemTool)sender;
                GitBranchInfo branch = (GitBranchInfo)treeItem.Tag;
                bool isRemote = (bool)treeItem.MetaData;

                foreach (TreeViewItemTool item in this.uxBanches.Items)
                {
                    this.SetNestedTreeItemsWeight(item, FontWeights.Normal);
                }

                if (isRemote)
                {
                    GitBranchInfo? localBranch = ERDGit.GetLocalBranches(General.ProjectModel.FileDirectory)
                        .FirstOrDefault(lb => lb.Name.Equals(branch.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (localBranch == null)
                    {
                        ERDGit.CreateBranchFromRemote(General.ProjectModel.FileDirectory, branch.Name);

                        this.uxBanches.Items.Clear();

                        this.InitializeBranchTree();
                    }
                    else
                    {
                        ERDGit.Checkout(General.ProjectModel.FileDirectory, localBranch.Name);

                        TreeViewItemTool localNode = this.GetTreeNode((TreeViewItemTool)this.uxBanches.Items[0], branch.Name);
                        localNode.FontWeight = FontWeights.Bold;
                    }
                }
                else
                {
                    ERDGit.Checkout(General.ProjectModel.FileDirectory, branch.Name);
                    treeItem.FontWeight = FontWeights.Bold;
                }

                this.BranchChanged = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void HistoryRow_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid grid
                && grid.SelectedItem is GitCommitInfo selectedCommit)
                {
                    string commitSha = selectedCommit.Sha;

                    IReadOnlyList<GitFileChangeInfo> changes = ERDGit.GetCommitChanges(General.ProjectModel.FileDirectory, commitSha);

                    this.uxCommitFiles.ItemsSource = changes;
                }

                this.uxPreviousFile.Document.Blocks.Clear();
                this.uxCommitedFile.Document.Blocks.Clear();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private async void CommitedRow_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.uxPreviousFile.Document.Blocks.Clear();
                this.uxCommitedFile.Document.Blocks.Clear();

                if (sender is DataGrid grid
                && grid.SelectedItem is GitFileChangeInfo selectedCommit)
                {
                    await this.LoadComparison(selectedCommit.PreviousContent, selectedCommit.CommitedContent);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
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
                    this.uxPreviousFile,
                    this.uxCommitedFile
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

        private void InitializeBranchTree()
        {
            IReadOnlyList<GitBranchInfo> localBranches = ERDGit.GetLocalBranches(General.ProjectModel.FileDirectory);

            this.LoadBranch(localBranches, "Local Branches", false);

            IReadOnlyList<GitBranchInfo> remoteBranches = ERDGit.GetRemoteBranches(General.ProjectModel.FileDirectory);

            Dictionary<string, IReadOnlyList<GitBranchInfo>> remoteGroup = remoteBranches
                .GroupBy(rn => rn.RemoteName)
                .ToDictionary(g => g.Key, g => (IReadOnlyList<GitBranchInfo>)g.ToList());

            foreach (KeyValuePair<string, IReadOnlyList<GitBranchInfo>> remoteBranch in remoteGroup)
            {
                this.LoadBranch(remoteBranch.Value, remoteBranch.Key, true);
            }
        }

        private void LoadBranch(IReadOnlyList<GitBranchInfo> branches, string rootFolderName, bool isRemote)
        {
            string currentBranch = ERDGit.GetCurrentBranch(General.ProjectModel.FileDirectory).Name;

            TreeViewItemTool rootNode = this.CreateFolder(isRemote ? $"remote/{rootFolderName}" : rootFolderName);
            Dictionary<string, TreeViewItemTool> localFolders = new Dictionary<string, TreeViewItemTool>();

            foreach (GitBranchInfo branch in branches.OrderBy(n => n.Name))
            {
                string nameToSplit = isRemote ? branch.Name.Replace($"{rootFolderName}/", string.Empty) : branch.Name;
                string[] split = nameToSplit.Split('/');
                FontWeight fontWeight = branch.Name.Equals(currentBranch, System.StringComparison.CurrentCultureIgnoreCase) ? FontWeights.Bold : FontWeights.Normal;

                if (split.Length == 1)
                {
                    TreeViewItemTool child = new TreeViewItemTool
                    {
                        Header = split[0],
                        Tag = branch,
                        FontWeight = fontWeight,
                        MetaData = isRemote
                    };
                    child.MouseDoubleClick += this.TreeItem_DoubleClicked;
                    child.MouseUp += this.TreeItem_Clicked;
                    if (!isRemote)
                    {
                        this.SetContextMenu(child);
                    }
                    rootNode.Items.Add(child);
                    continue;
                }

                if (!localFolders.ContainsKey(split[0]))
                {
                    TreeViewItemTool folder = this.CreateFolder(split[0]); // new TreeViewItemTool { Header = split[0] };
                    rootNode.Items.Add(folder);
                    localFolders.Add(split[0], folder);
                }

                TreeViewItemTool folderChild = new TreeViewItemTool
                {
                    Header = split[1],
                    Tag = branch,
                    FontWeight = fontWeight,
                    MetaData = isRemote
                };
                folderChild.MouseDoubleClick += this.TreeItem_DoubleClicked;
                folderChild.MouseUp += this.TreeItem_Clicked;
                if (!isRemote)
                {
                    this.SetContextMenu(folderChild);
                }

                localFolders[split[0]].Items.Add(folderChild);
            }

            if (!isRemote)
            {
                this.ExpandNodes(rootNode);
            }

            this.uxBanches.Items.Add(rootNode);
        }

        private void SetContextMenu(TreeViewItemTool node)
        {
            ContextMenu menu = new ContextMenu();

            MenuItem deleteBranch = new MenuItem { Header = "Delete Branch" };
            deleteBranch.Click += (s, e) =>
            {
                GitBranchInfo currentBranch = ERDGit.GetCurrentBranch(General.ProjectModel.FileDirectory);
                TreeViewItem clickedItem = node;
                GitBranchInfo branch = (GitBranchInfo)clickedItem.Tag;

                if (branch.Name.Equals(currentBranch.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    MessageBox.Show("Cannot delete the current branch.");
                    return;
                }

                if (MessageBox.Show($"Are you sure you want to delete the branch '{branch.Name}'?", "Confirm Delete", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }

                ERDGit.DeleteBranch(General.ProjectModel.FileDirectory, branch.Name);
                TreeViewItemTool parent = (TreeViewItemTool)clickedItem.Parent;
                parent.Items.Remove(clickedItem);
            };
            menu.Items.Add(deleteBranch);

            node.ContextMenu = menu;
        }

        private void ExpandNodes(TreeViewItemTool treeItem)
        {
            treeItem.IsExpanded = true;
            foreach (TreeViewItemTool child in treeItem.Items)
            {
                this.ExpandNodes(child);
            }
        }

        private void SetNestedTreeItemsWeight(TreeViewItemTool treeItem, FontWeight fontWeight)
        {
            treeItem.FontWeight = fontWeight;

            foreach (TreeViewItemTool child in treeItem.Items)
            {
                this.SetNestedTreeItemsWeight(child, fontWeight);
            }
        }

        private async Task LoadComparison(string? previousContent, string? commitedContent)
        {

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(previousContent))
                        {
                            previousContent = "{}";
                        }

                        if (string.IsNullOrEmpty(commitedContent))
                        {
                            commitedContent = "{}";
                        }

                        ConflictMapper mapper = new ConflictMapper();

                        var (incomingMap, localMap, mergeMap) = mapper.Map(this.MergeFieJson(previousContent), this.MergeFieJson(commitedContent));

                        this.Dispatcher.Invoke(() =>
                        {
                            this.uxPreviousFile.Document = mapper.BuildTextDocument(incomingMap);
                            this.uxCommitedFile.Document = mapper.BuildTextDocument(localMap);
                        });
                    }
                    catch (Exception err)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(err.InnerExceptionMessage());
                        });
                    }
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private TreeViewItemTool? GetTreeNode(TreeViewItemTool parent, string name)
        {
            foreach (TreeViewItemTool child in parent.Items)
            {
                if (child.Header.ToString().Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return child;
                }

                return this.GetTreeNode(child, name);
            }

            return null;
        }

        private TreeViewItemTool CreateFolder(string headerName)
        {
            StackPanel headerObject = new StackPanel { Orientation = Orientation.Horizontal };

            Image image = new Image
            {
                Source = IconSets.ResourceImageSource("Folder", 45),
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, 5, 0)
            };

            TextBlock textBlock = new TextBlock
            {
                Text = headerName,
                VerticalAlignment = VerticalAlignment.Center
            };

            headerObject.Children.Add(image);
            headerObject.Children.Add(textBlock);

            TreeViewItemTool result = new TreeViewItemTool { Header = headerObject };

            return result;
        }

        private string MergeFieJson(string fileText)
        {
            List<string> fileJson = new();

            foreach (string line in fileText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    using JsonDocument doc = JsonDocument.Parse(line);

                    if (doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        JsonElement clonedElement = doc.RootElement.Clone();

                        fileJson.Add(clonedElement.GetRawText());
                    }
                }
                catch (JsonException)
                {
                    // Ignore lines that are not valid JSON.
                }
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine("[");


            for (int i = 0; i < fileJson.Count; i++)
            {
                result.AppendLine(fileJson[i] + (i < fileJson.Count - 1 ? "," : ""));
            }

            result.AppendLine("]");

            return result.ToString();
        }
    }
}
