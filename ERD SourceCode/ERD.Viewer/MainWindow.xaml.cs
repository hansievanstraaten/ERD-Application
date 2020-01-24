using ERD.Base;
using ERD.Build;
using ERD.Build.Models;
using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DatabaseScripts.Compare;
using ERD.DatabaseScripts.Engineering;
using ERD.FileManagement;
using ERD.Models;
using ERD.Viewer.Build;
using ERD.Viewer.Comparer;
using ERD.Viewer.Database;
using ERD.Viewer.Tools;
using GeneralExtensions;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ViSo.Common;
using WPF.Tools.BaseClasses;
using WPF.Tools.Exstention;
using WPF.Tools.Functions;

namespace ERD.Viewer
{
    public partial class MainWindow : WindowBase
    {
        private Dictionary<string, ErdCanvasModel> canvasDictionary = new Dictionary<string, ErdCanvasModel>();

        private List<string> connectionsInMenue = new List<string>();

        private CanvasLocksListener listener = new CanvasLocksListener();
        
        public MainWindow()
        {
            this.InitializeComponent();

            this.Closing += this.MainWindow_Closing;

            this.DataContext = this;

            EventParser.ParseErrorObject += this.Error_Parsed;

            EventParser.ParseMessageObject += this.Message_Parsed;

            EventParser.ParseQueryObject += this.Query_Parsed;

            this.listener.FileChanged += this.ProjectFiles_Changed;

            this.listener.FileLockChanged += this.ProjectLocks_Changed;

            Connections.Instance.ConnectionChanged += this.Connections_Changed;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                foreach (ErdCanvasModel segment in this.canvasDictionary.Values)
                {
                    string lockedByFileUser = CanvasLocks.Instance.LockedByUser(segment.ModelSegmentControlName);

                    if (lockedByFileUser.IsNullEmptyOrWhiteSpace())
                    {
                        // No Lock Applied
                        continue;
                    }

                    if (segment.IsLocked)
                    {
                        // The user worked on this file but did not save the changes
                        CanvasLocks.Instance.UnlockFile(segment.ModelSegmentControlName);
                    }
                }

                CanvasLocks.Instance.RemoveUserFileLockObject();

                this.Dispatcher.InvokeShutdown();
            }
            catch (Exception err)
            {
                // DO NOTHING. Deconstructing
            }
        }

        private void ExitProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void Connections_Changed(object sender, DatabaseModel model)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.SetForwardEngineerOptions(Connections.Instance.IsDefaultConnection);
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private object Query_Parsed(object sender, ParseQueryEventArguments e)
        {
            switch (e.Title)
            {
                case "QueryTablesStack":

                    List<TableMenuItem> tablesStackResult = new List<TableMenuItem>();

                    foreach (TableMenuItem menuItem in this.uxTableStack.FindVisualControls(typeof(TableMenuItem)).Where(t => !((TableMenuItem) t).TableModelObject.IsDeleted))
                    {
                        tablesStackResult.Add(menuItem);
                    }

                    return tablesStackResult;

                case "TableObjectCanvasQuery":

                    string tableName = e.Arguments[0].ToString();

                    return this.canvasDictionary.Values.FirstOrDefault(t => t.SegmentTables.Any(a => a.TableName == tableName));

                default:
                    return null;
            }
        }

        private void Message_Parsed(object sender, ParseMessageEventArguments e)
        {
            try
            {
                switch (e.Title)
                {
                    case "DatabaseConnection":

                        this.Dispatcher.Invoke(() =>
                        {
                            this.uxDatabaseConnection.Content = $"Connection: {e.Message}";
                        });

                        break;

                    case "SetForwardEngineerOption":

                        this.Dispatcher.Invoke(() =>
                        {
                            this.SetForwardEngineerOptions(false);
                        });

                        break;

                    default:

                        this.Dispatcher.Invoke(() =>
                        {
                            this.uxMessage.Content = $"{(e.Title.IsNullEmptyOrWhiteSpace() ? string.Empty : $"{e.Title}: ")}{e.Message} ";
                        });

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void Error_Parsed(object sender, Exception err)
        {
            MessageBox.Show(err.InnerExceptionMessage());
        }

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProjectSetup setup = new ProjectSetup(new ProjectModel(), new DatabaseModel());

                bool? result = setup.ShowDialog();

                if (result.HasValue && !result.Value)
                {
                    return;
                }

                General.ProjectModel = setup.SelectedProjectModel;

                Connections.Instance.SetDefaultDatabaseModel(setup.SelectedDatabaseModel, false);

                this.ActivateMenu();

                this.LoadToolsStack();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.Filter = $"(*.{FileTypes.eprj})|*.{FileTypes.eprj}";

                bool? result = dlg.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                this.OpenProject(dlg.FileName);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (General.ProjectModel == null)
                {
                    MessageBox.Show("No Project Selected");

                    return;
                }

                ProjectSetup setup = new ProjectSetup(General.ProjectModel, Connections.Instance.DefaultDatabaseModel);

                bool? result = setup.ShowDialog();

                if (result.HasValue && !result.Value)
                {
                    return;
                }

                this.listener.StartWatcher();

                this.LoadConnectionsMenue();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SaveModel();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void GetDbTables_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem) e.Source;

                this.ReverseEngineer(item);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void RefreshFromDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem) e.Source;

                this.RefreshFromDB(item);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void CompareToDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem) e.Source;

                this.ComapreToDatabase(item);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void ForwardEngineer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem) e.Source;

                Connections.Instance.SetConnection(item, true);

                ForwardEngineer forward = new ForwardEngineer();

                forward.ForwardEngineeringCompleted += this.ForwardEngineering_Completed;

                List<TableModel> tablesList = new List<TableModel>();

                List<TableMenuItem> deleteList = new List<TableMenuItem>();

                foreach (TableMenuItem menuItem in this.uxTableStack.FindVisualControls(typeof(TableMenuItem)).Where(t => ((TableMenuItem) t).TableModelObject.IsDeleted))
                {
                    tablesList.Add(menuItem.TableModelObject);

                    deleteList.Add(menuItem);
                }

                foreach (ErdCanvasModel canvas in this.canvasDictionary.Values)
                {
                    tablesList.AddRange(canvas.SegmentTables);
                }

                forward.BuildDatabase(tablesList.ToArray(), false);

                foreach (TableMenuItem xItem in deleteList)
                {
                    this.uxTableStack.Children.Remove(xItem);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void ForwardEngineering_Completed(object sender, bool completed)
        {
            if (completed)
            {
                if (Connections.Instance.IsDefaultConnection)
                {
                    this.RefreshColumnIds();

                    this.SetForwardEngineerOptions(true);
                }
                
                this.SaveModel();
            }
        }

        private void ScriptDbChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ForwardEngineer forward = new ForwardEngineer();

                List<TableModel> tablesList = new List<TableModel>();

                List<TableMenuItem> deleteList = new List<TableMenuItem>();

                foreach (TableMenuItem menuItem in this.uxTableStack.FindVisualControls(typeof(TableMenuItem)).Where(t => ((TableMenuItem) t).TableModelObject.IsDeleted))
                {
                    tablesList.Add(menuItem.TableModelObject);

                    deleteList.Add(menuItem);
                }

                foreach (ErdCanvasModel canvas in this.canvasDictionary.Values)
                {
                    tablesList.AddRange(canvas.SegmentTables);
                }

                forward.BuildDatabase(tablesList.ToArray(), true);

                foreach (TableMenuItem xItem in deleteList)
                {
                    this.uxTableStack.Children.Remove(xItem);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void AddCanvas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErdCanvasModel segment = new ErdCanvasModel() {SegmentTables = new List<TableModel>()};

                ErdTabSetup tab = new ErdTabSetup(segment);

                bool? result = tab.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                if (canvasDictionary.ContainsKey(segment.ModelSegmentControlName))
                {
                    MessageBox.Show($"Duplicate Names {segment.ModelSegmentName} not allowed");

                    return;
                }

                this.AddCanvasObject(segment);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void TableSearch_Changed(object sender, RoutedEventArgs e)
        {
            UIElement[] tables = this.uxTableStack.FindVisualControls(typeof(TableMenuItem));

            if (this.uxTableSearch.Text.IsNullEmptyOrWhiteSpace())
            {
                foreach (TableMenuItem item in this.uxTableStack.FindVisualControls(typeof(TableMenuItem)))
                {
                    item.Visibility = Visibility.Visible;
                }

                return;
            }

            string searchText = this.uxTableSearch.Text.ToLower();

            foreach (TableMenuItem item in this.uxTableStack.FindVisualControls(typeof(TableMenuItem)))
            {
                item.Visibility = item.TableModelObject.TableName.ToLower().Contains(searchText) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void CanvasTable_Added(object sender, TableModel table)
        {
            try
            {
                UIElement[] tables = this.uxTableStack.FindVisualControls(typeof(TableMenuItem));

                string itemName = table.TableName.Replace(' ', '_');

                TableMenuItem tableItem = tables.FirstOrDefault(t => ((TableMenuItem) t).Name == itemName) as TableMenuItem;

                if (tableItem == null)
                {
                    tableItem = new TableMenuItem(table);

                    tableItem.TableMenuItemDoubleClick += this.TableMenuItem_DoubleClick;

                    this.uxTableStack.Children.Insert(0, tableItem);
                }

                string segmentName = ((TableCanvas) sender).ErdSegment.ModelSegmentName;

                tableItem.SetErdCanvas(segmentName, segmentName.IsNullEmptyOrWhiteSpace());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CanvasTable_Removed(object sender, TableModel tableModel)
        {
            try
            {
                TableCanvas canvas = (TableCanvas) sender;

                this.canvasDictionary[canvas.ErdSegment.ModelSegmentControlName].SegmentTables.Remove(tableModel);

                UIElement[] tables = this.uxTableStack.FindVisualControls(typeof(TableMenuItem));

                string itemName = tableModel.TableName.Replace(' ', '_');

                TableMenuItem tableItem = tables.FirstOrDefault(t => ((TableMenuItem) t).Name == itemName) as TableMenuItem;

                if (tableItem != null)
                {
                    tableItem.SetErdCanvas((tableModel.IsDeleted ? "DELETED" : string.Empty), true);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TableMenuItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TableMenuItem item = (TableMenuItem) sender;

                if (item.ErdCanvas.IsNullEmptyOrWhiteSpace())
                {
                    return;
                }

                this.uxTabControl.SetActive(item.ErdCanvas);

                TableCanvas tab = (TableCanvas) this.uxTabControl[item.ErdCanvas];

                TableModel table = tab[item.TableName];

                tab.ZoomTo(table.CanvasLocation);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ScriptBuildRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BuildScript.Setup == null)
                {
                    MessageBox.Show("Setup required");

                    return;
                }

                BuildClasses builder = new BuildClasses();

                builder.ScriptFiles(this.canvasDictionary.Values.ToArray(), this.Dispatcher);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ScriptBuildSetup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.canvasDictionary == null ||
                    this.canvasDictionary.Count == 0 ||
                    this.canvasDictionary.First().Value.SegmentTables == null ||
                    this.canvasDictionary.First().Value.SegmentTables.Count == 0 ||
                    this.canvasDictionary.First().Value.SegmentTables[0].Columns == null ||
                    this.canvasDictionary.First().Value.SegmentTables[0].Columns.Length == 0)
                {
                    throw new ApplicationException("You need at least one Canvas, with at least one Table and some Columns to use this function.");
                }

                BuildSetup setup = new BuildSetup(this.canvasDictionary.First().Value);

                setup.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ProjectFiles_Changed(object sender, string fullPathName, ErdCanvasModel changedModel)
        {
            this.LoadModelChanges(changedModel, fullPathName);
        }
        
        private void ProjectLocks_Changed(object sender, string fullPathName, FileSystemEventArgs e)
        {
            this.SetLocks(fullPathName);
        }
        
        private async void ReverseEngineer(object sender)
        {
            try
            {
                Exception error = null;

                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            Connections.Instance.SetConnection((MenuItem) sender, true);
                        });

                        ReverseEngineer reverse = new ReverseEngineer(this.Dispatcher);

                        List<TableModel> taleModelsList = reverse.GetTables();

                        this.Dispatcher.Invoke(() =>
                        {
                            this.UpdateTableList(taleModelsList);
                        });
                    }
                    catch (Exception innerErr)
                    {
                        error = innerErr;
                    }

                    EventParser.ParseMessage(this, this.Dispatcher, string.Empty, "Completed");

                });

                if (error != null)
                {
                    MessageBox.Show(error.InnerExceptionMessage());
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private async void RefreshFromDB(MenuItem sender)
        {
            try
            {
                Exception error = null;

                Connections.Instance.SetConnection(sender, true);

                ReverseEngineer reverse = new ReverseEngineer(this.Dispatcher);

                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        foreach (ErdCanvasModel segment in this.canvasDictionary.Values)
                        {
                            Dictionary<string, List<ColumnObjectModel>> tableAndColumns = reverse.GetInTableColumns(segment.SegmentTables.Select(tn => tn.TableName).ToArray());

                            foreach (TableModel table in segment.SegmentTables)
                            {
                                table.PrimaryKeyClusterConstraintName = reverse.GetTablePrimaryKeyCluster(table.TableName);

                                if (!tableAndColumns.ContainsKey(table.TableName))
                                {   // The table is on the canvas but not in the database
                                    // New table, or exists in the default DB but not in the connection DB
                                    continue;
                                }

                                List<ColumnObjectModel> databaseColumnList = tableAndColumns[table.TableName];

                                foreach (ColumnObjectModel databaseColumn in databaseColumnList)
                                {
                                    ColumnObjectModel tableColumn = table.Columns.FirstOrDefault(cn => cn.ColumnName == databaseColumn.ColumnName);

                                    if (tableColumn != null)
                                    {
                                        tableColumn.IsIdentity = databaseColumn.IsIdentity;

                                        tableColumn.AllowNulls = databaseColumn.AllowNulls;

                                        tableColumn.MaxLength = databaseColumn.MaxLength;

                                        tableColumn.Precision = databaseColumn.Precision;

                                        tableColumn.Scale = databaseColumn.Scale;

                                        tableColumn.SqlDataType = databaseColumn.SqlDataType;

                                        tableColumn.InPrimaryKey = databaseColumn.InPrimaryKey;

                                        tableColumn.Column_Id = databaseColumn.Column_Id;

                                        if (Connections.Instance.IsDefaultConnection)
                                        {
                                            tableColumn.OriginalPosition = databaseColumn.OriginalPosition;
                                        }

                                        if (databaseColumn.IsForeignkey)
                                        {
                                            // Protect Virtual Relations
                                            tableColumn.IsForeignkey = databaseColumn.IsForeignkey;

                                            tableColumn.ForeignKeyTable = databaseColumn.ForeignKeyTable;

                                            tableColumn.ForeignKeyColumn = databaseColumn.ForeignKeyColumn;

                                            tableColumn.ForeignConstraintName = databaseColumn.ForeignConstraintName;

                                        }
                                        else if (!tableColumn.IsVertualRelation)
                                        {
                                            tableColumn.IsForeignkey = false;

                                            tableColumn.ForeignKeyTable = string.Empty;

                                            tableColumn.ForeignKeyColumn = string.Empty;

                                            tableColumn.ForeignConstraintName = string.Empty;
                                        }

                                        continue;
                                    }

                                    table.Columns = table.Columns.Add(databaseColumn);
                                }

                                foreach (ColumnObjectModel tableColumn in table.Columns)
                                {
                                    if (databaseColumnList.Any(db => db.ColumnName == tableColumn.ColumnName))
                                    {
                                        continue;
                                    }

                                    table.Columns = table.Columns.Remove(tableColumn);
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        error = err;
                    }
                });

                if (error != null)
                {
                    MessageBox.Show(error.InnerExceptionMessage());
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private async void ComapreToDatabase(MenuItem sender)
        {
            try
            {
                Connections.Instance.SetConnection(sender, true);

                DatabaseCompare comparer = new DatabaseCompare(this.Dispatcher);

                List<TableModel> tablesList = new List<TableModel>();

                List<CompareResultModel> result = null;

                foreach (ErdCanvasModel canvas in this.canvasDictionary.Values)
                {
                    tablesList.AddRange(canvas.SegmentTables);
                }

                foreach (TableMenuItem menuItem in this.uxTableStack.FindVisualControls(typeof(TableMenuItem)).Where(t => !((TableMenuItem) t).TableModelObject.IsDeleted))
                {
                    if (!tablesList.Any(ta => ta.TableName == menuItem.TableName))
                    {
                        tablesList.Add(menuItem.TableModelObject);
                    }
                }

                await Task.Factory.StartNew(() =>
                {
                    result = comparer.RunComparison(tablesList);
                });

                if (result.Count == 0)
                {
                    MessageBox.Show("No Differences found between Database and ERD Model.");

                    return;
                }

                CompreResults compare = new CompreResults(result);

                compare.AutoSize = true;

                compare.Owner = this;

                compare.Topmost = true;

                compare.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private async void OpenProject(string fileName)
        {
            try
            {
                Exception error = null;

                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Paths.WaitFileRelease(fileName);

                        string[] fileLines = File.ReadAllLines(fileName);

                        #region PROJECT & DATABASE

                        General.ProjectModel = JsonConvert.DeserializeObject(fileLines[1], typeof(ProjectModel)) as ProjectModel;

                        this.listener.StartWatcher();

                        DatabaseModel databaseModel = JsonConvert.DeserializeObject(fileLines[4], typeof(DatabaseModel)) as DatabaseModel;

                        Connections.Instance.SetDefaultDatabaseModel(databaseModel, false);

                        if (fileLines.Length > 5)
                        {
                            for (int x = 5; x < fileLines.Length; ++x)
                            {
                                string read = fileLines[x];

                                if (read == "BUILD_SETUP")
                                {
                                    ++x;

                                    continue;
                                }

                                AltDatabaseModel alternativeConnection = JsonConvert.DeserializeObject(fileLines[x], typeof(AltDatabaseModel)) as AltDatabaseModel;

                                Connections.Instance.AlternativeModels.Add(alternativeConnection.ConnectionName, alternativeConnection);
                            }
                        }

                        #endregion

                        #region BUILD_SETUP

                        string buildFileName = Path.Combine(General.ProjectModel.FileDirectory, $"{General.ProjectModel.ModelName}.{FileTypes.estp}");

                        if (File.Exists(buildFileName))
                        {
                            Paths.WaitFileRelease(buildFileName);

                            string buildFile = File.ReadAllText(buildFileName);

                            BuildSetupModel buildSetup = JsonConvert.DeserializeObject(buildFile, typeof(BuildSetupModel)) as BuildSetupModel;

                            BuildScript.Setup = buildSetup;
                        }

                        #endregion

                        this.Dispatcher.Invoke(() =>
                        {
                            EventParser.ParseMessage(this, "Loading", General.ProjectModel.ModelName);
                        });

                        string directoryPath = Path.GetDirectoryName(fileName);

                        Integrity.KeepColumnsUnique = General.ProjectModel.KeepColumnsUnique;

                        Integrity.AllowDatabaseRelations = General.ProjectModel.AllowRelations;

                        Integrity.AllowVertualRelations = General.ProjectModel.AllowVertualRelations;

                        DirectoryInfo dir = new DirectoryInfo(directoryPath);

                        foreach (FileInfo fileInfo in dir.GetFiles($"{General.ProjectModel.ModelName}.*.{FileTypes.eclu}"))
                        {
                            Paths.WaitFileRelease(fileInfo.FullName);

                            string[] fileData = File.ReadAllLines(fileInfo.FullName);

                            ErdCanvasModel segment = JsonConvert.DeserializeObject(fileData[0], typeof(ErdCanvasModel)) as ErdCanvasModel;

                            EventParser.ParseMessage(this, this.Dispatcher, "Adding Canvas", segment.ModelSegmentControlName);

                            this.Dispatcher.Invoke(() =>
                            {
                                this.AddCanvasObject(segment);
                            });
                        }

                        ReverseEngineer reverse = new ReverseEngineer(this.Dispatcher);

                        List<TableModel> taleModelsList = reverse.GetTables();

                        this.Dispatcher.Invoke(() =>
                        {
                            this.UpdateTableList(taleModelsList);

                            this.ActivateMenu();

                            this.LoadToolsStack();

                            this.LoadConnectionsMenue();

                            this.LoadConnectionsMenue();

                            this.uxTabControl.SetActive(0);

                            this.uxMessage.DisplaySeconds = 5;

                            EventParser.ParseMessage(this, string.Empty, "Load Completed");
                        });
                    }
                    catch (Exception innerErr)
                    {
                        error = innerErr;
                    }

                });

                if (error != null)
                {
                    throw error;
                }

                this.SetLocks(string.Empty);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private async void RefreshColumnIds()
        {
            if (!Connections.Instance.IsDefaultConnection)
            {
                return;
            }

            Exception error = null;

            ForwardEngineer forward = new ForwardEngineer();

            DataAccess dataAccess = new DataAccess(Connections.Instance.DatabaseModel);

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (ErdCanvasModel canvas in this.canvasDictionary.Values)
                    {
                        foreach (TableModel table in canvas.SegmentTables)
                        {
                            EventParser.ParseMessage(this, this.Dispatcher, "Refresh table Columns", table.TableName);

                            foreach (ColumnObjectModel column in table.Columns)
                            {
                                if (column.Column_Id == int.MaxValue || (column.InPrimaryKey && column.OriginalPosition == 0))
                                {
                                    string sqlQuery = SQLQueries.DatabaseQueries.DatabaseColumnKeysQuery(table.TableName, column.ColumnName);

                                    XDocument sqlResult = dataAccess.ExecuteQuery(sqlQuery);

                                    column.Column_Id = sqlResult.Root.Element("Row_0").Element("COLUMN_ID").Value.ToInt32();

                                    string positionValue = sqlResult.Root.Element("Row_0").Element("ORDINAL_POSITION").Value;

                                    column.OriginalPosition = positionValue.IsNullEmptyOrWhiteSpace() ? 0 : positionValue.ToInt32();
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    error = err;
                }
            });

            if (error != null)
            {
                MessageBox.Show(error.InnerExceptionMessage());
            }

            foreach (ErdCanvasModel canvas in this.canvasDictionary.Values)
            {
                foreach (TableModel table in canvas.SegmentTables)
                {
                    table.HasModelChanged = false;

                    foreach (ColumnObjectModel column in table.Columns)
                    {
                        column.HasModelChanged = false;
                    }
                }
            }


        }

        private void SaveModel()
        {
            string saveFileFullName = Path.Combine(General.ProjectModel.FileDirectory, $"{General.ProjectModel.ModelName}.{FileTypes.eprj}");

            if (General.ProjectModel.FileDirectory.IsNullEmptyOrWhiteSpace())
            {
                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = $"(*.{FileTypes.eprj})|*.{FileTypes.eprj}";

                dlg.FileName = $"{General.ProjectModel.ModelName}.{FileTypes.eprj}";

                bool? result = dlg.ShowDialog();

                if (!result.HasValue || !result.Value)
                {
                    return;
                }

                saveFileFullName = dlg.FileName;
            }

            StringBuilder project = new StringBuilder();

            #region PROJECT

            project.AppendLine("PROJECT");

            General.ProjectModel.HasModelChanged = false;

            Connections.Instance.DefaultDatabaseModel.HasModelChanged = false;

            project.AppendLine(JsonConvert.SerializeObject(General.ProjectModel));

            project.AppendLine();

            #endregion

            #region DATABASE

            project.AppendLine("DATABASE");

            project.AppendLine(JsonConvert.SerializeObject(Connections.Instance.DefaultDatabaseModel));

            foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
            {
                project.AppendLine(JsonConvert.SerializeObject(connectionKey.Value));
            }

            File.WriteAllText(saveFileFullName, project.ToString());

            #endregion

            CanvasLocks.Instance.RemoveUserLocks(Environment.UserName);

            string directory = Path.GetDirectoryName(saveFileFullName);

            foreach (ErdCanvasModel segment in this.canvasDictionary.Values.Where(il => il.IsLocked))
            {   // Save only the files that was worked on

                segment.IsLocked = false;

                string segmentName = $"{General.ProjectModel.ModelName}.{segment.ModelSegmentControlName}.{FileTypes.eclu}";

                string segmentFilFullName = Path.Combine(directory, segmentName);

                lock (CanvasLocks.Instance.IgnoreFiles)
                {
                    CanvasLocks.Instance.IgnoreFiles.Add(segmentFilFullName);
                }

                File.WriteAllText(segmentFilFullName, JsonConvert.SerializeObject(segment));
            }

            this.uxMessage.Content = "Project Saved";

            lock (CanvasLocks.Instance.IgnoreFiles)
            {
                CanvasLocks.Instance.IgnoreFiles.Clear();
            }
        }

        private void ActivateMenu()
        {
            this.Title = $"ViSo - ERD Model - {General.ProjectModel.ModelName}";

            this.uxNewProject.IsEnabled = false;

            this.uxOpenProject.IsEnabled = false;

            this.uxMenuSaveProject.IsEnabled = true;

            this.uxMenuProject.IsEnabled = true;

            this.uxMenuDatabase.IsEnabled = true;

            this.uxMenuBuild.IsEnabled = true;
        }

        private void LoadConnectionsMenue()
        {
            if (Connections.Instance.AlternativeModels.Count > 0)
            {
                this.uxGetDbTables.Click -= this.GetDbTables_Click;

                this.uxRefreshFromDB.Click -= this.RefreshFromDB_Click;

                this.uxForwardEngineer.Click -= this.ForwardEngineer_Click;

                if (!this.connectionsInMenue.Contains(Connections.Instance.DefaultConnectionName))
                {
                    this.connectionsInMenue.Add(Connections.Instance.DefaultConnectionName);

                    MenuItem defaultReverseItem = new MenuItem {Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})", Tag = Connections.Instance.DefaultConnectionName};

                    MenuItem defaultRefreshItem = new MenuItem {Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})", Tag = Connections.Instance.DefaultConnectionName};

                    MenuItem defaultCompareItem = new MenuItem {Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})", Tag = Connections.Instance.DefaultConnectionName};

                    MenuItem defaultForwardItem = new MenuItem {Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})", Tag = Connections.Instance.DefaultConnectionName};

                    defaultReverseItem.Click += this.GetDbTables_Click;

                    defaultRefreshItem.Click += this.RefreshFromDB_Click;

                    defaultForwardItem.Click += this.ForwardEngineer_Click;

                    this.uxGetDbTables.Items.Add(defaultReverseItem);

                    this.uxRefreshFromDB.Items.Add(defaultRefreshItem);

                    this.uxCompareToDB.Items.Add(defaultCompareItem);

                    this.uxForwardEngineer.Items.Add(defaultForwardItem);
                }

                foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
                {
                    if (this.connectionsInMenue.Contains(connectionKey.Key))
                    {
                        continue;
                    }

                    this.connectionsInMenue.Add(connectionKey.Key);

                    MenuItem reverseItem = new MenuItem {Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})", Tag = connectionKey.Key};

                    MenuItem refreshItem = new MenuItem {Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})", Tag = connectionKey.Key};

                    MenuItem compareItem = new MenuItem {Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})", Tag = connectionKey.Key};

                    MenuItem forwardItem = new MenuItem {Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})", Tag = connectionKey.Key};

                    reverseItem.Click += this.GetDbTables_Click;

                    refreshItem.Click += this.RefreshFromDB_Click;

                    forwardItem.Click += this.ForwardEngineer_Click;

                    this.uxGetDbTables.Items.Add(reverseItem);

                    this.uxRefreshFromDB.Items.Add(refreshItem);

                    this.uxCompareToDB.Items.Add(compareItem);

                    this.uxForwardEngineer.Items.Add(forwardItem);
                }
            }
        }

        private void AddCanvasObject(ErdCanvasModel segment)
        {
            if (segment.SegmentTables == null)
            {
                segment.SegmentTables = new List<TableModel>();
            }

            this.canvasDictionary.Add(segment.ModelSegmentControlName, segment);

            TableCanvas canvas = new TableCanvas(segment, Connections.Instance.DatabaseModel) {Name = segment.ModelSegmentControlName, Title = segment.ModelSegmentName};

            canvas.TableAdded += this.CanvasTable_Added;

            canvas.TableRemoved += this.CanvasTable_Removed;

            this.uxTabControl.Items.Add(canvas);

            this.UpdateTableList(segment.SegmentTables);
        }

        private void UpdateTableList(List<TableModel> tablesList)
        {
            if (tablesList == null || tablesList.Count == 0)
            {
                return;
            }

            UIElement[] tables = this.uxTableStack.FindVisualControls(typeof(TableMenuItem));

            double maxWidth = tables.HasElements() ? tables.Max(m => ((TableMenuItem) m).DesiredNameWidth) : 0;

            foreach (TableModel tableItem in tablesList.OrderBy(n => n.TableName))
            {
                string itemName = tableItem.TableName.Replace(' ', '_');

                UIElement tableElement = tables.FirstOrDefault(t => ((TableMenuItem) t).Name == itemName);

                if (tableElement != null)
                {
                    continue;
                }

                TableMenuItem item = new TableMenuItem(tableItem) {Name = itemName};

                item.TableMenuItemDoubleClick += this.TableMenuItem_DoubleClick;

                double desiredWidth = item.DesiredNameWidth;

                if (desiredWidth > maxWidth)
                {
                    maxWidth = desiredWidth;
                }

                Integrity.MapTable(tableItem);

                tables = tables.Add(item);

                this.uxTableStack.Children.Add(item);
            }

            foreach (UIElement menuItemElement in tables)
            {
                ((TableMenuItem) menuItemElement).DesiredNameWidth = maxWidth;
            }
        }

        private void LoadToolsStack()
        {
            TableMenuItem tableTool = new TableMenuItem(new TableModel {TableName = "Table", IsNewTable = true}) {Name = "Table"};

            tableTool.TableMenuItemDoubleClick += this.TableMenuItem_DoubleClick;

            this.uxToolsStack.Children.Add(tableTool);

            if (General.ProjectModel.AllowRelations)
            {
                RelationMenuItem databaseRelation = new RelationMenuItem(RelationTypesEnum.DatabaseRelation);

                this.uxToolsStack.Children.Add(databaseRelation);
            }

            if (General.ProjectModel.AllowVertualRelations)
            {
                RelationMenuItem vertualRelation = new RelationMenuItem(RelationTypesEnum.VirtualRelation);

                this.uxToolsStack.Children.Add(vertualRelation);
            }
        }

        private async void LoadModelChanges(ErdCanvasModel changedModel, string fullPathName)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (!this.canvasDictionary.ContainsKey(changedModel.ModelSegmentName))
                        {
                            EventParser.ParseMessage(this, this.Dispatcher, "Adding Canvas", changedModel.ModelSegmentControlName);

                            this.Dispatcher.Invoke(() =>
                            {
                                int activeTabIndex = this.uxTabControl.SelectedIndex;

                                this.AddCanvasObject(changedModel);

                                this.uxTabControl.SetActive(activeTabIndex);
                            });
                        }
                        else
                        {
                            TableCanvas canvas = this.uxTabControl.Items
                                .FirstOrDefault(cn => ((TableCanvas) cn).ErdSegment.ModelSegmentControlName == changedModel.ModelSegmentControlName)
                                .To<TableCanvas>();

                            canvas.RefreshModel(changedModel);
                        }
                    }
                    catch (Exception err)
                    {

                    }
                    
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }

            this.SetLocks(fullPathName);
        }

        private async void SetLocks(string fullPathName)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        List<string> lockedFiles = CanvasLocks.Instance.GetLockedFiles();

                        foreach (TableCanvas canvas in this.uxTabControl.Items)
                        {
                            canvas.CheckLockStatus(lockedFiles);
                        }
                    }
                    catch (Exception err)
                    {

                    }
                });
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
            finally
            {
                // The initial lock was set in
                // ERD.FileManagement.CanvasLocksListener
                // private void File_Changed(object sender, FileSystemEventArgs e)
                lock (CanvasLocks.Instance.IgnoreFiles)
                {
                    CanvasLocks.Instance.IgnoreFiles.Remove(fullPathName);
                }
            }
        }

        private void SetForwardEngineerOptions(bool isEnabled)
        {
            if (this.uxForwardEngineer.Items == null || this.uxForwardEngineer.Items.Count == 0)
            {
                return;
            }

            foreach(MenuItem item in this.uxForwardEngineer.Items)
            {
                if (item.Tag.ParseToString() == Connections.Instance.DefaultConnectionName)
                {
                    continue;
                }

                item.IsEnabled = isEnabled;
            }
        }
    }
}
