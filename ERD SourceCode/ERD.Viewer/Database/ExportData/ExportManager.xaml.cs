using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DataExport;
using ERD.DataExport.Models;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;

namespace ERD.Viewer.Database.ExportData
{
    /// <summary>
    /// Interaction logic for ExportManager.xaml
    /// </summary>
    public partial class ExportManager : WindowBase
    {
        private TableModel selectedTableModel;

        public ExportManager(TableModel tableModel, MenuItem connectionMenue)
        {
            this.InitializeComponent();

            this.selectedTableModel = tableModel;

            this.Options = new ExportOptions();

            this.Options.Source = connectionMenue.Tag.IsNullEmptyOrWhiteSpace() ? Connections.Instance.DefaultConnectionName : connectionMenue.Tag.ToString();

            this.Options.Destination = "Default";

            this.Options.OutputDirectory = Path.GetTempPath();

            this.uxModelView.Items.Add(this.Options);

            this.LoadColumns();

            this.Options.PropertyChanged += this.OptionsChanged;
        }

        private void OptionsChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Options.PropertyChanged -= this.OptionsChanged;

            try
            {
                if (this.Options.Destination == "Default")
                {
                    this.uxModelView["Place Data In SQL"].IsReadOnly = false;
                }
                else
                {
                    this.Options.PlaceDataInSQL = true;
                    this.uxModelView["Place Data In SQL"].IsReadOnly = true;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
            finally
            {
                this.Options.PropertyChanged += this.OptionsChanged;
            }
        }

        public ExportOptions Options
        {
            get;

            set;
        }

        private void OkButton_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            string mergeSqlFile = string.Empty;

            try            
            {
                if (this.uxModelView.HasValidationError)
                {
                    return;
                }

                DataExporter exporter = new DataExporter();

                string csvFile = exporter.Export(this.Options.Source, this.selectedTableModel, this.Options.DataDelimiter, this.Options.OutputDirectory, this.Options.TopX);

                mergeSqlFile = Scripting.ScriptMerge(this.selectedTableModel, 
                    this.GetSelectedColumns(), 
                    csvFile, 
                    this.Options.DataDelimiter, 
                    this.Options.MergeIdentityValues, 
                    this.Options.PlaceDataInSQL, 
                    this.Options.MergeInsert,
                    this.Options.MergeUpdate,
                    this.Options.MergeDelete);

                if (!this.Options.PlaceDataInSQL)
                {
                    MessageBox.Show("Export completed");

                    string argument = $"/select, \"{mergeSqlFile}\"";

                    Process.Start("explorer.exe", argument);

                    return;
                }

                string sqlMergeScript = File.ReadAllText(mergeSqlFile);

                DataAccess access = new DataAccess(Connections.Instance.GetConnection(this.Options.Destination));

                access.ExecuteNonQuery(sqlMergeScript, 0);

                MessageBox.Show("Merge completed");

                this.Close();

            }
            catch(System.Data.SqlClient.SqlException err)
            {
                MessageBox.Show(err.Message);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }

        private void Cancel_Cliked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }

        private void Options_Browse(object sender, string buttonKey)
        {
            try
            {
                switch (buttonKey)
                {
                    case "OutputDirectoryKey":

                        System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

                        if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }

                        this.Options.OutputDirectory = folder.SelectedPath;

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.GetFullExceptionMessage());
            }
        }
    
        private string[] GetSelectedColumns()
        {
            List<string> result = new List<string>();

            foreach(TreeViewItemTool item in this.uxColumns.Items)
            {
                if(item.IsChecked)
                {
                    result.Add(item.Header.ToString());
                }
            }

            return result.ToArray();
        }

        private void LoadColumns()
        {
            foreach(ColumnObjectModel column in this.selectedTableModel.Columns)
            {
                TreeViewItemTool item = new TreeViewItemTool { IsCheckBox = true, Header = column.ColumnName };

                item.IsChecked = column.InPrimaryKey || column.IsForeignkey;

                this.uxColumns.Items.Add(item);
            }
        }
    }
}
