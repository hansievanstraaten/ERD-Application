using ERD.Models;
using GeneralExtensions;
using System;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Tables
{
  /// <summary>
  /// Interaction logic for TableEdit.xaml
  /// </summary>
  public partial class TableEdit : WindowBase
  {
    public TableEdit(TableModel table)
    {
      this.InitializeComponent();

      this.Table = table;

      this.uxTable.Items.Add(this.Table);
    }

    public TableModel Table {get; set;}

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.uxTable.HasValidationError)
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
