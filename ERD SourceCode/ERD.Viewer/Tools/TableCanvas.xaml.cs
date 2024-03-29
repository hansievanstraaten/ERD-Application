﻿using ERD.Base;
using ERD.Common;
using ERD.FileManagement;
using ERD.Models;
using ERD.Models.ModelExstentions;
using ERD.Viewer.Tables;
using GeneralExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ViSo.Common;
using ViSo.Dialogs.ModelViewer;
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

        private string headingText = "Tables Prefix: ";

        public TableCanvas(ErdCanvasModel erdSegment, DatabaseModel databaseModel)
        {
            this.InitializeComponent();

            this.ErdSegment = erdSegment;

            this.ErdSegment.ErdCanvasModelLockChanged += this.ErdSegmentLock_Changed;

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

            this.uxTabMetadata.Content = $"{this.headingText} {this.ErdSegment.TablePrefix}";

        }

        private void ErdSegmentLock_Changed(object sender, bool lockStatus)
        {
            this.SetLock();
        }

        private void Canvas_Changed(object sender, object changedItem)
        {
            this.SetLock();
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
                return this.ErdSegment.SegmentTables.FirstOrDefault(tn => tn.TableName == tableName || tn.FullName() == tableName);
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

        public async void RefreshModel(ErdCanvasModel changedModel)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    this.uxTableCanvas.CanvasChanged -= this.Canvas_Changed;

                    List<TableModel> removedTables = new List<TableModel>();

                    TableModel[] existingTables = this.ErdSegment.SegmentTables.ToArray();

                    // REMOVE DROPED TABLES
                    foreach (TableModel existing in existingTables)
                    {
                        TableModel table = changedModel.SegmentTables.FirstOrDefault(t => t.TableName.ToLower() == existing.TableName.ToLower());

                        if (table == null)
                        {
                            // The table was removed
                            this.Dispatcher.Invoke(() => 
                            { 
                                this.uxTableCanvas.RemoveTableFromCanvas(existing);
                            });

                            removedTables.Add(existing);
                        }
                    }

                    foreach(TableModel remove in removedTables)
                    {
                        this.ErdSegment.SegmentTables.Remove(remove);
                    }

                    foreach (TableModel table in changedModel.SegmentTables)
                    {
                        lock (SingletonLock.Instance.LockObject)
                        {
                            TableModel existing = this.ErdSegment.SegmentTables.FirstOrDefault(t => t.TableName.ToLower() == table.TableName.ToLower());

                            if (existing == null)
                            {
                                this.Dispatcher.Invoke(() =>
                                {
                                    this.uxTableCanvas.CreateTableObject(table);
                                });

                                this.ErdSegment.SegmentTables.Add(table);
                            }
                            else
                            {
                                existing = table.CopyTo(existing);

                                this.Dispatcher.Invoke(() =>
                                {
                                    this.uxTableCanvas.RefreshTableRelations(existing);
                                });
                            }
                        }
                    }
                });
            }
            catch
            {

            }
            finally
            {
                this.uxTableCanvas.CanvasChanged += this.Canvas_Changed;
            }
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
                string[] exludeTables = this.ErdSegment.SegmentTables == null || this.ErdSegment.SegmentTables.Count == 0 ?
                new string[] { } :    
                this.ErdSegment.SegmentTables.Select(t => t.TableName).ToArray();

                SelectedTables selector = new SelectedTables(this.ErdSegment.IncludeInContextBuild.ToArray(), exludeTables);

                bool? result = selector.ShowDialog();

                if (!result.IsTrue())
                {
                    return;
                }

                this.ErdSegment.IncludeInContextBuild.Clear();

                this.ErdSegment.IncludeInContextBuild.AddRange(selector.SelectedModels());

                this.Canvas_Changed(this, this.ErdSegment.IncludeInContextBuild);
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

		private void EditCanvasData_Clicked(object sender, RoutedEventArgs e)
		{
            try
			{
                ErdCanvasEditModel editModel = this.ErdSegment.CopyTo(new ErdCanvasEditModel()).To<ErdCanvasEditModel>();

                if (ModelView.ShowDialog("Edit Canvas", editModel).IsFalse())
				{
                    return;
				}

                if (editModel.TablePrefix == this.ErdSegment.TablePrefix)
				{
                    return;
				}

                this.ErdSegment.SetLock(true, true);

                editModel.CopyTo(this.ErdSegment);

                this.uxTabMetadata.Content = $"{this.headingText} {this.ErdSegment.TablePrefix}";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void PrintCanvas_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog print = new PrintDialog();

                if (print.ShowDialog().IsFalse())
                {
                    return;
                }

                Size pageSize = new Size(print.PrintableAreaWidth, print.PrintableAreaHeight);

                this.uxTableCanvas.Measure(pageSize);

                this.uxTableCanvas.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                
                print.PrintVisual(this.uxTableCanvas, "Printing Canvas");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    
        private void SetLock()
        {
            EventParser.ParseMessage(this, "SetForwardEngineerOption", string.Empty);

            this.ErdSegment.SetLock(true, false); // Alwasy set this to ensure that the canvas will be saved

            if (!General.ProjectModel.LockCanvasOnEditing)
            {
                return;
            }

            CanvasLocks.Instance.LockFile(this.ErdSegment.ModelSegmentControlName);

            this.uxTabLock.Visibility = Visibility.Visible;

            this.uxTabLock.Content = "This Canvas is locked by You";
        }

	}
}
