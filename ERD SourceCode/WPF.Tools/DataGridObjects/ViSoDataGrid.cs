using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ViSo.Common;
using WPF.Tools.Dictionaries;
using WPF.Tools.Exstention;

namespace WPF.Tools.DataGridObjects
{
  public class ViSoDataGrid : DataGrid
  {
    public ViSoDataGrid()
    {
      this.ContextMenu = new ContextMenu();

      MenuItem exportCsv = new MenuItem {Header = "Export CSV"};

      exportCsv.Click += this.ExportCsv_Clicked;

      base.BorderThickness = new Thickness(0);

      this.ContextMenu.Items.Add(exportCsv);

      this.HorizontalGridLinesBrush = Brushes.WhiteSmoke;

      this.VerticalGridLinesBrush = Brushes.WhiteSmoke;

      this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

      this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

      this.IsReadOnly = true;
    }

    new public ContextMenu ContextMenu
    {
      get
      {
        if (base.ContextMenu == null)
        {
          return new ContextMenu();
        }

        return base.ContextMenu;
      }

      set
      {
        if (base.ContextMenu == null)
        {
          base.ContextMenu = value;

          return;
        }

        Separator sep = new Separator();

        base.ContextMenu.Items.Add(sep);

        foreach (MenuItem item in value.Items)
        {
          base.ContextMenu.Items.Add(item);
        }
      }
    }

    protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
    {
      DependencyObject dependency = (DependencyObject) e.OriginalSource;

      if (dependency.GetType() == typeof(Border) ||
          dependency.GetType() ==  typeof(System.Windows.Shapes.Rectangle) ||
          dependency.GetType() == typeof(System.Windows.Shapes.Path) ||
          dependency.GetType() == typeof(ScrollViewer))
      {
        e.Handled = true;

        return;
      }

      base.OnMouseDoubleClick(e);
    }

    private void ExportCsv_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        DataTable table = this.GetDataTable();

        StringBuilder result = new StringBuilder();

        IEnumerable<string> columnNames = table.Columns.TryCast<DataColumn>().Select(c => c.ColumnName);

        result.AppendLine(string.Join("|", columnNames));

        foreach (DataRow row in table.Rows)
        {
          IEnumerable<string> rowValues = row.ItemArray.Select(f => f.ParseToString());

          result.AppendLine(string.Join("|", rowValues));
        }

        string path = Paths.KnownFolder(KnownFolders.KnownFolder.Downloads);

        string filePath = Path.Combine(path, "DataGridExport.csv");

        int pathNumber = 1;

        while (File.Exists(filePath))
        {
          filePath = Path.Combine(path, $"DataGridExport ({pathNumber}).csv");

          ++pathNumber;
        }

        File.WriteAllText(filePath, result.ToString());

        Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }
  }
}
