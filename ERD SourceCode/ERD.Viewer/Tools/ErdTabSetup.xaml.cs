using ERD.Models;
using GeneralExtensions;
using System;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Tools
{
  /// <summary>
  /// Interaction logic for ErdTabSetup.xaml
  /// </summary>
  public partial class ErdTabSetup : WindowBase
  {
    public ErdTabSetup(ErdCanvasModel erdSegment)
    {
      this.InitializeComponent();

      this.ErdSegment = erdSegment;

      this.uxModelViewer.Items.Add(erdSegment);
    }

    public ErdCanvasModel ErdSegment {get; private set;}

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.uxModelViewer.HasValidationError)
        {
          return;
        }

        this.DialogResult = true;

        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }
  }
}
