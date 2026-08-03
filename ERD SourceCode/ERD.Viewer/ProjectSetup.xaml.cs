using ERD.Base;
using ERD.Common;
using ERD.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Forms;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using MessageBox = System.Windows.MessageBox;

namespace ERD.Viewer
{
    /// <summary>
    /// Interaction logic for ProjectSetup.xaml
    /// </summary>
    public partial class ProjectSetup : WindowBase
    {
        private List<AltDatabaseModel> newAlternativeOptions = new List<AltDatabaseModel>();

        public ProjectSetup(ProjectModel projectModel, DatabaseModel databaseModel)
        {
            InitializeComponent();

            this.SelectedProjectModel = projectModel;

            this.SelectedDatabaseModel = databaseModel;

            this.uxProjectSetup.Items.Add(projectModel);

            this.uxProjectSetup.Items.Add(databaseModel);

            this.SetDBOptions((ModelViewObject)this.uxProjectSetup[1], databaseModel.DatabaseType);

            foreach (KeyValuePair<string, AltDatabaseModel> item in Connections.Instance.AlternativeModels)
            {
                this.uxAlternativeConnections.Items.Add(item.Value);

                int itemIndex = this.uxAlternativeConnections.Items.Count - 1;

                this.uxAlternativeConnections[itemIndex].Header = $"Database Setup: {item.Value.ConnectionName}";

                this.uxAlternativeConnections[itemIndex].ToggelCollaps(true);

                this.SetDBOptions((ModelViewObject)this.uxAlternativeConnections[itemIndex], databaseModel.DatabaseType);
            }

            this.uxProjectSetup.AllignAllCaptions();

            this.uxProjectSetup.ModelViewItemBrowse += this.ModelViewItem_Browse;
            
            this.uxProjectSetup.ModelViewItemSelectedValueChanged += this.SelectedValue_Changed;

            this.uxAlternativeConnections.ModelViewItemSelectedValueChanged += this.SelectedValue_Changed;
        }

        public ProjectModel SelectedProjectModel { get; private set; }

        public DatabaseModel SelectedDatabaseModel { get; private set; }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.uxProjectSetup.HasValidationError || this.uxAlternativeConnections.HasValidationError)
                {
                    return;
                }

                foreach (AltDatabaseModel altModel in this.newAlternativeOptions)
                {
                    if (Connections.Instance.AlternativeModels.ContainsKey(altModel.ConnectionName) || altModel.ConnectionName.StartsWith("Default"))
                    {
                        throw new ApplicationException($"Duplicate Connection Name {altModel.ConnectionName} not allowed.");
                    }

                    Connections.Instance.AlternativeModels.Add(altModel.ConnectionName, altModel);
                }

                Integrity.KeepColumnsUnique = this.SelectedProjectModel.KeepColumnsUnique;

                Integrity.AllowDatabaseRelations = this.SelectedProjectModel.AllowRelations;

                Integrity.AllowVertualRelations = this.SelectedProjectModel.AllowVertualRelations;

                this.DialogResult = true;

                this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ModelViewItem_Browse(object sender, string buttonkey)
        {
            try
            {
                switch (buttonkey)
                {
                    case "DirectoryBrowse":

                        FolderBrowserDialog folder = new FolderBrowserDialog();

                        if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        {
                            return;
                        }

                        this.SelectedProjectModel.FileDirectory = folder.SelectedPath;

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void AddConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AltDatabaseModel altOption = new AltDatabaseModel();

                this.newAlternativeOptions.Add(altOption);

                this.uxAlternativeConnections.Items.Add(altOption);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void SelectedValue_Changed(object sender, object newValue)
        {
            DatabaseTypeEnum dbType = (DatabaseTypeEnum)newValue;

            this.SetDBOptions((ModelViewObject)sender, dbType);
        }

        private void SetDBOptions(ModelViewObject dbSetupOption, DatabaseTypeEnum dbType)
        {
            int offset = dbSetupOption.Items.Count == 9 ? 1 : 0;

            dbSetupOption[2 + offset].Visibility = dbType == DatabaseTypeEnum.POSTGRES ? Visibility.Visible : Visibility.Collapsed;
            dbSetupOption[7 + offset].Visibility = dbType == DatabaseTypeEnum.POSTGRES ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
