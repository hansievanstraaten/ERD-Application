using ERD.Base;
using ERD.Build;
using ERD.Build.Models;
using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DatabaseScripts.Engineering;
using ERD.Models;
using ERD.Build.BuildEnums;
using ERD.Viewer.Columns;
using ERD.Viewer.Database;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Tools.BaseClasses;
using WPF.Tools.Functions;
using ContextMenu = System.Windows.Controls.ContextMenu;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using ViSo.Dialogs.ModelViewer;

namespace ERD.Viewer.Tools
{
    /// <summary>
    /// Interaction logic for TableObject.xaml
    /// </summary>
    public partial class TableObject : UserControlBase
    {
        public delegate void TableMoveEvent(object sender, bool isDrag);

        public delegate void RemoveTableEvent(object sender, TableModel tableModel);

        public delegate bool ForeignKeyColumnAddedEvent(object sender, ColumnObjectModel column);

        public delegate void TableColumnChangedEvent(object sender, ColumnObjectModel column);

        public delegate void TableHeaderChangedEvent(object sender);

        public event TableMoveEvent TableMove;

        public event RemoveTableEvent RemoveTable;

        public event ForeignKeyColumnAddedEvent ForeignKeyColumnAdded;

        public event TableColumnChangedEvent TableColumnChanged;

        public event TableHeaderChangedEvent TableHeaderChanged;

        private Point location;

        private ColumnObjectModel selectedColumn;

        public List<string> LinkedRelations;

        public Dictionary<string, DatabaseRelation> ColumnRelationModel;

        public TableObject(TableModel table)
        {
            this.InitializeComponent();

            this.InitializeColumnsContextMenu();

            this.uxTableName.PreviewMouseRightButtonUp += this.TableName_RightButtonUp;

            this.DataContext = this;

            this.Table = table;

            this.LinkedRelations = new List<string>();

            this.InitializeColumnRemations();

            this.uxTableName.Content = this.Table.TableName;

            this.uxTableName.PreviewMouseDown += this.TableName_StartStopDrag;

            this.uxTableName.PreviewMouseUp += this.TableName_StartStopDrag;

            this.uxTableName.PreviewMouseMove += this.TableName_MouseMove;

            this.Table.Columns = this.Table.Columns
                .OrderByDescending(pk => pk.InPrimaryKey)
                .ThenBy(fk => fk.IsForeignkey)
                .ToArray();

            this.Location = table.CanvasLocation;
        }

        public TableModel Table
        {
            get;
            private set;
        }

        public ColumnObjectModel SelectedColumn
        {
            get
            {
                return this.selectedColumn;
            }

            set
            {
                this.selectedColumn = value;

                base.OnPropertyChanged(() => this.SelectedColumn);
            }
        }

        public Point Location
        {
            get
            {
                return this.location;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                this.location = value;

                Canvas.SetTop(this, value.Y);

                Canvas.SetLeft(this, value.X);

                this.Table.CanvasLocation = value;
            }
        }
        
        public void StopDrag()
        {
            if (this.TableMove != null)
            {
                this.TableMove(this, false);
            }
        }

        public void InitializeColumnRemations()
        {
            this.ColumnRelationModel = new Dictionary<string, DatabaseRelation>();

            Dictionary<string, ColumnRelationMapModel[]> foreignKeys = this.Table.Columns
                .Where(c => c.IsForeignkey)
                .GroupBy(t => t.ForeignKeyTable)
                .ToDictionary(d => d.Key, d => d
                    .Select(a => new ColumnRelationMapModel
                    {
                        ParentTable = a.ForeignKeyTable,
                        ParentColumn = a.ForeignKeyColumn,
                        ChildTable = this.Table.TableName,
                        ChildColumn = a.ColumnName,
                        ForeignConstraintName = a.ForeignConstraintName,
                        RelationTypes = a.IsVertualRelation ? RelationTypesEnum.VirtualRelation : RelationTypesEnum.DatabaseRelation
                    }).ToArray());

            foreach (KeyValuePair<string, ColumnRelationMapModel[]> item in foreignKeys)
            {
                DatabaseRelation relation = new DatabaseRelation
                {
                    RelationshipName = Guid.NewGuid().ToString().Replace('-', '_'),
                    ChildTable = this.Table.TableName,
                    ParentTable = item.Key,
                    Columns = new List<ColumnRelationMapModel>(item.Value),
                    RelationType = item.Value[0].RelationTypes
                };

                string dictionaryKey = $"{item.Key}||{this.Table.TableName}";

                this.ColumnRelationModel.Add(dictionaryKey, relation);
            }
        }

        private void TableName_StartStopDrag(object sender, MouseButtonEventArgs e)
        {
            if (this.TableMove != null)
            {
                this.TableMove(this, e.LeftButton == MouseButtonState.Pressed);
            }
        }

        private void TableName_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject obj = new DataObject();

                object[] dataValues = new object[] {this.GetType(), this};

                obj.SetData(DataFormats.Serializable, dataValues);

                DragDrop.DoDragDrop(this, obj, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            this.RemoveTable?.Invoke(this, this.Table);
        }

        private void DropTable_Clicked(object sender, RoutedEventArgs e)
        {
            string message = $"You are about to drop Table {this.Table.TableName} with the next Forward Engineering Build. Are you sure you would like to continue?";

            MessageBoxResult result = MessageBox.Show(message, "Warning", MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
            {
                return;
            }

            this.Table.IsDeleted = true;

            if (this.RemoveTable != null)
            {
                this.RemoveTable(this, this.Table);
            }
        }

        private void MenuColumnUsage_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedColumn == null)
                {
                    MessageBox.Show("Invalid Column Selection");

                    return;
                }

                ReverseEngineer reverse = new ReverseEngineer(this.Dispatcher);

                Connections.Instance.SetConnection((MenuItem) sender, false);

                string column = this.SelectedColumn.ColumnName;

                BrowseData browse = new BrowseData(SQLQueries.DatabaseQueries.DatabaseColumnUsageQuery(column), $"Column Usage for: {column}");

                browse.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void MenuAddColumn_Clicked(object sender, RoutedEventArgs e)
        {
            this.ShowAddColumn();
        }

        private void MenuEditColumn_Clicked(object sender, RoutedEventArgs e)
        {
            this.ShowEditColumn();
        }

        private void MenuEditColumnDelete_Clicked(object sender, RoutedEventArgs e)
        {
            string message = $"You are about to drop Column {this.SelectedColumn.ColumnName} with the next Forward Engineering Build. Are you sure you would like to continue?";

            MessageBoxResult result = MessageBox.Show(message, "Warning", MessageBoxButton.OKCancel);

            if (result != MessageBoxResult.OK)
            {
                return;
            }

            this.TableColumnChanged?.Invoke(this, this.SelectedColumn);

            this.Table.DeltedColumns = this.Table.DeltedColumns.Add(this.SelectedColumn);

            this.Table.Columns = this.Table.Columns.Remove(this.SelectedColumn);
        }

        private void ViewData_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                BrowseData browse = new BrowseData(this.Table, (MenuItem) sender);

                browse.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void ViewDataBaseScript_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder result = new StringBuilder();

                result.Append(Scripting.ScriptTableCreate(this.Table));

                result.AppendLine();

                result.Append(Scripting.BuildForeignKey(this.Table));

                string filePath = Path.Combine(Path.GetTempPath(), $"{this.Table.TableName}.txt");

                File.WriteAllText(filePath, result.ToString());

                Process.Start(filePath);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void ViewCSharpScript_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                ErdCanvasModel canvas = EventParser.ParseQuery(this, new ParseQueryEventArguments
                {
                    Title = "TableObjectCanvasQuery",
                    Arguments = new object[] {this.Table.TableName}
                }).To<ErdCanvasModel>();

                SampleScript scriptor = new SampleScript();

                foreach (OptionSetupModel option in BuildScript.Setup.BuildOptions.Where(o => o.RepeatOption == RepeatOptionEnum.ForeachTableProject))
                {
                    scriptor.LanguageOption = option.LanguageOption;

                    string fileName = option.OutputFileName.Replace("[[TableName]]", this.Table.TableName);

                    string filePath = Path.Combine(Path.GetTempPath(), $"{fileName}.txt");

                    List<ErdCanvasModel> allErdCancases = EventParser.ParseQuery(this, "GetAllErdCanvasesAsList").To<List<ErdCanvasModel>>();

                    string result = scriptor.BuildSampleForeachTableScript(canvas, allErdCancases, this.Table, option);

                    File.WriteAllText(filePath, result);

                    Process.Start(filePath);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void UndoChanges_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (ColumnObjectModel column in this.Table.Columns)
                {
                    Tuple<object, object> allowNull = column["AllowNulls"];

                    if (allowNull != null)
                    {
                        column.AllowNulls = (bool) allowNull.Item1;
                    }

                    Tuple<object, object> columnName = column["ColumnName"];

                    if (columnName != null)
                    {
                        column.ColumnName = columnName.Item1.ToString();
                    }

                    Tuple<object, object> dataType = column["DataType"];

                    if (dataType != null)
                    {
                        column.DataType = dataType.Item1.ToString();
                    }

                    Tuple<object, object> description = column["Description"];

                    if (description != null)
                    {
                        column.Description = description.Item1.ToString();
                    }

                    Tuple<object, object> friendlyName = column["FriendlyName"];

                    if (friendlyName != null)
                    {
                        column.FriendlyName = friendlyName.Item1.ToString();
                    }

                    Tuple<object, object> inprimaryKey = column["InPrimaryKey"];

                    if (inprimaryKey != null)
                    {
                        column.InPrimaryKey = (bool) inprimaryKey.Item1;
                    }

                    column.HasModelChanged = false;
                }

                this.Table.HasModelChanged = false;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void SetupModel_Browse(object sender, string buttonKey)
        {
            EventParser.ParseMessage(sender, new ParseMessageEventArguments {Title = "SetupModel_Browse", Arguments = new object[] {buttonKey}});
        }

        private void TableName_RightButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.InitializeTableNameContextMenu();

            this.uxTableName.ContextMenu.IsOpen = true;
        }

        private void EditHeader_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                TableModel tableCopy = this.Table.CopyTo(new TableModel());

                if (ModelView.ShowDialog("Edit Table Properties", tableCopy).IsFalse())
                {
                    return;
                }

                this.Table = tableCopy.CopyTo(this.Table);

                this.TableHeaderChanged?.Invoke(this);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void InitializeTableNameContextMenu()
        {
            #region HEADER CONTEXT SETUP

            this.uxTableName.ContextMenu = new ContextMenu();
            
            MenuItem viewData = new MenuItem {Header = "View Data"};

            if (Connections.Instance.AlternativeModels.Count > 0)
            {
                MenuItem defaultItem = new MenuItem {Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})", Tag = Connections.Instance.DefaultConnectionName};

                defaultItem.Click += this.ViewData_Clicked;

                viewData.Items.Add(defaultItem);

                foreach (KeyValuePair<string, AltDatabaseModel> connection in Connections.Instance.AlternativeModels)
                {
                    MenuItem item = new MenuItem {Header = $"{connection.Key} ({connection.Value.DatabaseName})", Tag = connection.Key};

                    item.Click += this.ViewData_Clicked;

                    viewData.Items.Add(item);
                }
            }
            else // if (Connections.Instance.AlternativeModels.Count == 0)
            {
                viewData.Click += this.ViewData_Clicked;
            }
            
            MenuItem editHeader = new MenuItem { Header = "Edit Table Header" };

            MenuItem viewDbScript = new MenuItem {Header = "View Database Script"};

            MenuItem viewCSharp = new MenuItem {Header = "View Generated Script"};

            MenuItem undoChanges = new MenuItem {Header = "Undo Changes"};

            Separator separator1 = new Separator();

            MenuItem remove = new MenuItem {Header = "Remove from Canvas"};

            MenuItem dropTable = new MenuItem {Header = "Drop Table"};

            editHeader.Click += this.EditHeader_Cliked;

            viewDbScript.Click += this.ViewDataBaseScript_Clicked;

            viewCSharp.Click += this.ViewCSharpScript_Cliked;

            undoChanges.Click += this.UndoChanges_Clicked;

            remove.Click += this.Remove_Clicked;

            dropTable.Click += this.DropTable_Clicked;

            this.uxTableName.ContextMenu.Items.Add(editHeader);

            this.uxTableName.ContextMenu.Items.Add(viewData);

            this.uxTableName.ContextMenu.Items.Add(viewDbScript);

            this.uxTableName.ContextMenu.Items.Add(viewCSharp);

            this.uxTableName.ContextMenu.Items.Add(undoChanges);

            this.uxTableName.ContextMenu.Items.Add(separator1);

            this.uxTableName.ContextMenu.Items.Add(remove);

            this.uxTableName.ContextMenu.Items.Add(dropTable);

            #endregion
        }
    
        private void InitializeColumnsContextMenu()
        {
            #region DATA GRID SETUP

            this.uxTableDataGrid.ContextMenu = new ContextMenu();

            MenuItem columnUssage = new MenuItem {Header = "Column Usage"};

            MenuItem addColumn = new MenuItem {Header = "Add Column"};

            MenuItem editColumn = new MenuItem {Header = "Edit Column"};

            Separator separator2 = new Separator();

            MenuItem deleteColumn = new MenuItem {Header = "Delete Column"};

            columnUssage.Click += this.MenuColumnUsage_Clicked;

            addColumn.Click += this.MenuAddColumn_Clicked;

            editColumn.Click += this.MenuEditColumn_Clicked;

            deleteColumn.Click += this.MenuEditColumnDelete_Clicked;

            this.uxTableDataGrid.ContextMenu.Items.Add(columnUssage);

            this.uxTableDataGrid.ContextMenu.Items.Add(addColumn);

            this.uxTableDataGrid.ContextMenu.Items.Add(editColumn);

            this.uxTableDataGrid.ContextMenu.Items.Add(separator2);

            this.uxTableDataGrid.ContextMenu.Items.Add(deleteColumn);

            #endregion
        }

        private void ShowAddColumn()
        {
            try
            {
                ColumnObjectModel column = new ColumnObjectModel {Column_Id = int.MaxValue};

                ColumnsEdit columnEdit = new ColumnsEdit(column, this.Table.TableName);

                bool? result = columnEdit.ShowDialog();

                if (result == null || !result.Value)
                {
                    return;
                }

                column.HasModelChanged = false; // This is to clear the model changes values. Especialy for new column creation

                this.Table.Columns = this.Table.Columns.Add(columnEdit.Column);

                Integrity.MapColumn(column, this.Table.TableName);

                if (column.IsForeignkey)
                {
                    this.ForeignKeyColumnAdded?.Invoke(this, column);
                }

                this.TableColumnChanged?.Invoke(this, column);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void ShowEditColumn()
        {
            try
            {
                if (this.SelectedColumn == null)
                {
                    MessageBox.Show("Column not selected");

                    return;
                }

                ColumnsEdit columnEdit = new ColumnsEdit(this.SelectedColumn, this.Table.TableName);

                bool? result = columnEdit.ShowDialog();

                if (result.IsFalse())
                {
                    return;
                }

                this.TableColumnChanged?.Invoke(this, this.SelectedColumn);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }
    }
}
