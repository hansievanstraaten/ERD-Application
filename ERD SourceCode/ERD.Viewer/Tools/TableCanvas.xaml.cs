using ERD.Common;
using ERD.FileManagement;
using ERD.Models;
using ERD.Viewer.Tables;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.Functions;

namespace ERD.Viewer.Tools
{
    /// <summary>
    /// Interaction logic for TableCanvas.xaml
    /// </summary>
    public partial class TableCanvas : UserControlBase
    {
        public delegate void TableAddedEvent(object sender, TableModel table);

        public delegate void TableRemovedEvent(object sender, TableModel tableModel);

        public event TableAddedEvent TableAdded;

        public event TableRemovedEvent TableRemoved;

        public TableCanvas(ErdCanvasModel erdSegment, DatabaseModel databaseModel)
        {
            this.InitializeComponent();

            this.ErdSegment = erdSegment;

            this.uxTableCanvas.NewTablePrefix = this.ErdSegment.TablePrefix;

            foreach (TableModel table in this.ErdSegment.SegmentTables)
            {
                this.uxTableCanvas.CreateTableObject(table);
            }

            this.uxTableCanvas.DatabaseModel = databaseModel;

            this.uxTableCanvas.TableAdded += this.Table_Added;

            this.uxTableCanvas.RemoveTable += this.Table_Removed;

            this.uxTableCanvas.CanvasChanged += this.Canvas_Changed;

            this.SizeChanged += this.TableCanvas_SizeChanged;

            this.uxTabMetadata.Content = $"Tables Prefix: {this.ErdSegment.TablePrefix}";

        }

        private void Canvas_Changed(object sender, object changedItem)
        {
            EventParser.ParseMessage(this, "SetForwardEngineerOption", string.Empty);
            
            if (!General.ProjectModel.LockCanvasOnEditing)
            {
                return;
            }

            CanvasLocks.Instance.LockFile(this.ErdSegment.ModelSegmentControlName);

            this.uxTabLock.Visibility = Visibility.Visible;

            this.uxTabLock.Content = "This Canvas is locked by You";
        }

        private void Table_Removed(object sender, TableModel tableModel)
        {
            if (this.TableRemoved != null)
            {
                this.TableRemoved(this, tableModel);
            }
        }

        public TableModel this[string tableName]
        {
            get
            {
                return this.ErdSegment.SegmentTables.FirstOrDefault(tn => tn.TableName == tableName);
            }
        }

        public ErdCanvasModel ErdSegment
        {
            get;
            set;
        }

        public void ZoomTo(Point location)
        {
            this.uxCanvasScroll.ZoomTo(location);
        }

        public async void CheckLockStatus(List<string> lockedFiles)
        {
            bool isLocked = false;

            bool haveLock = false;

            string lockedByUser = string.Empty;

            string thisName = $"{this.ErdSegment.ModelSegmentControlName}:";

            await Task.Factory.StartNew(() =>
            {
                foreach (string lockedItem in lockedFiles)
                {
                    if (!lockedItem.StartsWith(thisName))
                    {
                        continue;
                    }

                    string[] lockInformation = lockedItem.Split(':');

                    lockedByUser = lockInformation[1];

                    isLocked = Environment.UserName != lockedByUser;

                    if (!isLocked)
                    {
                        lockedByUser = "You";
                    }

                    haveLock = true;
                }
            });

            this.Dispatcher.Invoke(() =>
            {
                this.uxTabLock.Visibility = haveLock ? Visibility.Visible : Visibility.Collapsed;

                this.uxTabLock.Content = $"This Canvas is locked by {lockedByUser}";

                this.uxTableCanvas.IsEnabled = !isLocked;

                if (lockedByUser == "You")
                {
                    EventParser.ParseMessage(this, "SetForwardEngineerOption", string.Empty);
                }
            });
        }

        private void TableObjet_Move(object sender, bool isDrag)
        {
            this.uxCanvasScroll.IsPannable = !isDrag;
        }

        private void Table_Added(object sender, TableModel table)
        {
            if (!this.ErdSegment.SegmentTables.Any(t => t.TableName == table.TableName))
            {
                table.ErdSegmentModelName = this.ErdSegment.ModelSegmentName;

                this.ErdSegment.SegmentTables.Add(table);

                this.TableAdded?.Invoke(this, table);
            }
        }

        private void AdditionalTables_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedTables selector = new SelectedTables(this.ErdSegment.IncludeInContextBuild.ToArray());

                bool? result = selector.ShowDialog();

                if (!result.IsTrue())
                {
                    return;
                }

                this.ErdSegment.IncludeInContextBuild.Clear();

                this.ErdSegment.IncludeInContextBuild.AddRange(selector.SelectedModels());

            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void TableCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.uxTableCanvas.MinWidth = this.uxCanvasScroll.ActualWidth + 1000;

            this.uxTableCanvas.MinHeight = this.uxCanvasScroll.ActualHeight + 1000;
        }
    }
}
