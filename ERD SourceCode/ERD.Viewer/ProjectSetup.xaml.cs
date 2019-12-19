using ERD.Common;
using ERD.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using WPF.Tools.BaseClasses;
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
      this.InitializeComponent();

      this.SelectedProjectModel = projectModel;

      this.SelectedDatabaseModel = databaseModel;
      
      this.uxProjectSetup.Items.Add(projectModel);

      this.uxProjectSetup.Items.Add(databaseModel);

      foreach (KeyValuePair<string, AltDatabaseModel> item in Connections.AlternativeModels)
      {
        this.uxAlternativeConnections.Items.Add(item.Value);
      }

      this.uxProjectSetup.AllignAllCaptions();

      this.uxProjectSetup.ModelViewItemBrowse += this.ModelViewItem_Browse;
    }
    
    public ProjectModel SelectedProjectModel {get; private set;}

    public DatabaseModel SelectedDatabaseModel {get; private set;}
    
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
          if (Connections.AlternativeModels.ContainsKey(altModel.ConnectionName) || altModel.ConnectionName.StartsWith("Default"))
          {
            throw new ApplicationException($"Duplicate Connection Name {altModel.ConnectionName} not allowed.");
          }

          Connections.AlternativeModels.Add(altModel.ConnectionName, altModel);
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
  }
}
