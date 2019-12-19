using ERD.DatabaseScripts.Compare;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Windows;
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

        if (ModelView.ShowDialog("Edit Discrepancy", copyModel).IsFalse())
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

    private void Script_Cliked(object sender, System.Windows.RoutedEventArgs e)
    {
      try
      {

      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }
  }
}
