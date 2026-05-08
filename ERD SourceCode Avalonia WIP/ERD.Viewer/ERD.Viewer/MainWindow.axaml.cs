using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using ERD.Viewer.FileManagement;
using ERD.Viewer.Models.CanvasModels;
using ERD.Viewer.Models.ProjectModels;
using ERD.Viewer.Shared.Extensions;
using ERD.Viewer.Tools.BaseControls;
using ERD.Viewer.Tools.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ERD.Viewer
{
    public partial class MainWindow : ViewerWindowBase
    {
        private Dictionary<string, ErdCanvasModel> canvasDictionary = new Dictionary<string, ErdCanvasModel>();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OnMainWindow_Closed(object? sender, EventArgs e)
        {
            try
            {
                foreach (ErdCanvasModel segment in this.canvasDictionary.Values)
                {
                    string lockedByFileUser = CanvasLocks.Instance.LockedByUser(segment.ModelSegmentControlName);

                    if (lockedByFileUser.IsNullEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    if (segment.IsLocked)
                    {
                        CanvasLocks.Instance.UnlockFile(segment.ModelSegmentControlName);
                    }
                }

                CanvasLocks.Instance.RemoveUserFileLockObject();

                base.Dispatcher.Shutdown();
            }
            catch (Exception err)
            {
                // DO NOTHING. Deconstructing
            }
        }

        private void NewProject_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            //TaskUtilities.Run(() => { throw new NotImplementedException("Method not implemented"); });
            try
            {
                ProjectSetup setup = new ProjectSetup(new ProjectModel(), new DatabaseModel());
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }

        }

        private void OpenProject_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void ExitProject_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception err)
            {
                WindowNotificationManager manager = new WindowNotificationManager(this)
                {
                    Position = NotificationPosition.TopRight
                };
                manager.Show(new Notification("Error", err.Message));
                //Avalonia.Controls.Notifications.MessageBox.Show(err.Message);
                //MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void EditProject_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void AddCanvas_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void ForwardEngineer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void RefreshFromDB_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void CompareToDB_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void GetDbTables_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void ScriptDbChanges_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void ScriptBuildSetup_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void ScriptBuildRun_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void ReportSystemSetup_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void About_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void SelectReport_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void InstallUpdates_Cliked(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        private void TableSearch_Changed(object? sender, TextChangedEventArgs e)
        {
            try
            {
                throw new NotImplementedException("Method not implemented");
            }
            catch (Exception err)
            {
                this.ShowError(err);
            }
        }

        
    }
}