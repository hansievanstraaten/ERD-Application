using ERD.Common;
using ERD.DatabaseScripts;
using ERD.DatabaseScripts.Compare;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ViSo.Common;
using ViSo.Dialogs.ModelViewer;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Comparer
{
  /// <summary>
  /// Interaction logic for CompreResults.xaml
  /// </summary>
  public partial class CompreResults : WindowBase
  {
    private CompareResultModel selectedResult;

    private CompareResultModel[] comparedResults;
    
    public CompreResults(List<CompareResultModel> results)
    {
      this.InitializeComponent();

      this.DataContext = this;

      this.ComparedResults = results.ToArray();
    }

    public CompareResultModel SelectedResult
    {
      get
      {
        return this.selectedResult;
      }

      set
      {
        this.selectedResult = value;

        base.OnPropertyChanged(() => this.SelectedResult);
      }
    }

    public CompareResultModel[] ComparedResults
    {
      get
      {
        return this.comparedResults;
      }

      set
      {
        this.comparedResults = value;

        base.OnPropertyChanged(() => this.ComparedResults);
      }
    }

    private void Accept_Cliked(object sender, System.Windows.RoutedEventArgs e)
    {
      try
      {
        StringBuilder result = new StringBuilder();

        string filePath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{General.ProjectModel.ModelName}_Comparison.sql");

        int itemIndex = 1;

        while (File.Exists(filePath))
        {
          filePath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{General.ProjectModel.ModelName}_Comparison({itemIndex}).sql");

          ++itemIndex;
        }

        foreach (CompareResultModel model in this.ComparedResults)
        {
          switch (model.ObjectAction)
          {
            case ObjectActionEnum.AlterDatabase:

              result.AppendLine(this.AlterDatabase(model));

              break;

            case ObjectActionEnum.CreateInDatabase:

              result.AppendLine(this.CreateInDatabase(model));

              break;

            case ObjectActionEnum.DropFromDatabase:

              result.AppendLine(this.DropFromDatabase(model));

              break;
              
            case ObjectActionEnum.Ignore:
            default:
              break;
          }
        }

        File.WriteAllText(filePath, result.ToString());

        Process.Start(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads));
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }
    
    private void Script_Cliked(object sender, System.Windows.RoutedEventArgs e)
    {
      try
      {
        StringBuilder result = new StringBuilder();

        string filePath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{General.ProjectModel.ModelName}_Comparison.sql");

        int itemIndex = 1;

        while(File.Exists(filePath))
        {
          filePath = Path.Combine(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads), $"{General.ProjectModel.ModelName}_Comparison({itemIndex}).sql");

          ++itemIndex;
        }

        foreach(CompareResultModel model in this.ComparedResults)
        {
          switch(model.ObjectAction)
          {
            case ObjectActionEnum.AlterDatabase:

              result.AppendLine(this.AlterDatabase(model));

              break;

            case ObjectActionEnum.CreateInDatabase:

              result.AppendLine(this.CreateInDatabase(model));

              break;

            case ObjectActionEnum.DropFromDatabase:

              result.AppendLine(this.DropFromDatabase(model));

              break;

            case ObjectActionEnum.Ignore:
            default:
              break;
          }
        }

        File.WriteAllText(filePath, result.ToString());

        Process.Start(Paths.KnownFolder(KnownFolders.KnownFolder.Downloads));
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }

    private void Edit_Cliked(object sender, System.Windows.RoutedEventArgs e)
    {
      if (this.SelectedResult == null)
      {
        MessageBox.Show("Please select a discrepancy.");

        return;
      }

      try
      {
        CompareResultModel copyModel = this.SelectedResult.CopyTo(new CompareResultModel());

        if (ModelView.ShowDialog(this, true, "Edit Discrepancy", copyModel).IsFalse())
        {
          return;
        }

        this.SelectedResult = copyModel.CopyTo(this.SelectedResult);
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }
  
    private string AlterDatabase(CompareResultModel model)
    {
      switch (model.ObjectType)
      {
        case ObjectTypeEnum.Column:

          return Scripting.BuildColumnAlter(model.TableName, model.TableObject.Columns.First(col => col.ColumnName == model.ObjectName));

        case ObjectTypeEnum.ForeignKeyConstraint:

          StringBuilder foreingResult = new StringBuilder();

          string constraintKey = $"{model.TableName}||{model.ObjectName}";

          foreingResult.AppendLine(Scripting.DropForeignKey(constraintKey));

          foreingResult.AppendLine(Scripting.BuildForeignKey(model.TableObject));

          return foreingResult.ToString();

        case ObjectTypeEnum.Table:

          StringBuilder tableResult = new StringBuilder();

          tableResult.AppendLine(Scripting.ScriptTableCreate(model.TableObject));

          foreach(ColumnObjectModel column in model.TableObject.Columns)
          {
            tableResult.AppendLine(Scripting.BuildeColumnCreate(Integrity.GetTableSchema(model.TableName), model.TableName, column));

            tableResult.AppendLine(Scripting.BuildColumnAlter(model.TableName, column));
          }

          return tableResult.ToString();
      }

      return string.Empty;
    }

    private string CreateInDatabase(CompareResultModel model)
    {
      switch(model.ObjectType)
      {
        case ObjectTypeEnum.Column:

          return Scripting.BuildeColumnCreate(Integrity.GetTableSchema(model.TableName), model.TableName, model.TableObject.Columns.First(col => col.ColumnName == model.ObjectName));

        case ObjectTypeEnum.ForeignKeyConstraint:

          return Scripting.BuildForeignKey(model.TableObject);

        case ObjectTypeEnum.Table:

          return Scripting.ScriptTableCreate(model.TableObject);
      }

      return string.Empty;
    }

    private string DropFromDatabase(CompareResultModel model)
    {
      switch (model.ObjectType)
      {
        case ObjectTypeEnum.Column:

          return Scripting.DropColumn(Integrity.GetTableSchema(model.TableName), model.TableName, model.ObjectName);

        case ObjectTypeEnum.ForeignKeyConstraint:

          string constraintKey = $"{model.TableName}||{model.ObjectName}";

          return Scripting.DropForeignKey(constraintKey);

        case ObjectTypeEnum.Table:

          return Scripting.DropTable(model.TableObject);
      }

      return string.Empty;
    }
  }
}
