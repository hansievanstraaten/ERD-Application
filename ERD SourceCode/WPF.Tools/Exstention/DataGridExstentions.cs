using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF.Tools.Exstention
{
  public static class DataGridExstentions
  {
    public static void SetGridCellForeground(this DataGrid grid, int rowIndex, int cellIndex, Brush color)
    {
      try
      {
        grid.EnableColumnVirtualization = false;

        grid.EnableRowVirtualization = false;

        grid.UpdateLayout();

        grid.SelectedIndex = rowIndex;

        DataGridRow gridRow = grid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

        if (gridRow == null)
        {
          return;
        }

        if (cellIndex < 0)
        {
          foreach (DataGridCellInfo cellItem in grid.SelectedCells)
          {
            FrameworkElement itemElement = cellItem.Column.GetCellContent(gridRow);

            itemElement.SetPropertyValue("Foreground", color);
          }

          return;
        }

        if (grid.SelectedCells.Count < cellIndex)
        {
          return;
        }

        DataGridCellInfo gridCell = grid.SelectedCells[cellIndex];

        if (gridCell == null)
        {
          return;
        }

        FrameworkElement cellObject = gridCell.Column.GetCellContent(gridRow);

        cellObject.SetPropertyValue("Foreground", color);
      }
      catch
      {
        throw;
      }
      finally
      {
        grid.EnableColumnVirtualization = true;

        grid.EnableRowVirtualization = true;
      }
    }

    public static void SetGridCellFontWeight(this DataGrid grid, int rowIndex, int cellIndex, FontWeight weight)
    {
      try
      {
        grid.EnableColumnVirtualization = false;

        grid.EnableRowVirtualization = false;

        grid.UpdateLayout();

        grid.SelectedIndex = rowIndex;

        DataGridRow gridRow = grid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;

        if (gridRow == null)
        {
          return;
        }

        if (cellIndex < 0)
        {
          foreach (DataGridCellInfo cellItem in grid.SelectedCells)
          {
            FrameworkElement itemElement = cellItem.Column.GetCellContent(gridRow);

            itemElement.SetPropertyValue("FontWeight", weight);
          }

          return;
        }

        if (grid.SelectedCells.Count < cellIndex)
        {
          return;
        }

        DataGridCellInfo gridCell = grid.SelectedCells[cellIndex];

        if (gridCell == null)
        {
          return;
        }

        FrameworkElement cellObject = gridCell.Column.GetCellContent(gridRow);

        cellObject.SetPropertyValue("FontWeight", weight);
      }
      catch
      {
        throw;
      }
      finally
      {
        grid.EnableColumnVirtualization = true;

        grid.EnableRowVirtualization = true;
      }
    }

    public static DataTable GetDataTableStructureCaptions(this DataGrid dg)
    {
      DataTable table = new DataTable();

      DataGridColumn[] dataGridColumns = dg.Columns.Where(x => x.Visibility == Visibility.Visible).OrderBy(x => x.DisplayIndex).OfType<DataGridColumn>().ToArray();

      List<string> memberPath = new List<string>();

      foreach (DataGridColumn item in dataGridColumns)
      {
        string[] splitPath = item.SortMemberPath.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (splitPath.Length == 0 || memberPath.Contains(splitPath[(splitPath.Length - 1)]) || splitPath[(splitPath.Length - 1)].IsNullEmptyOrWhiteSpace())
        {
          continue;
        }

        table.Columns.Add(new DataColumn(splitPath[(splitPath.Length - 1)], typeof(string)));

        memberPath.Add(splitPath[(splitPath.Length - 1)]);
      }

      foreach (object row in dg.ItemsSource)
      {
        List<object> list = new List<object>();

        object actualRow = row.GetPropertyValue("ActualDataRow");

        foreach (string mmPath in memberPath)
        {
          object theValue = actualRow == null ? row.GetPropertyValue(mmPath) : actualRow.GetPropertyValue(mmPath);

          list.Add(theValue);
        }

        table.Rows.Add(list.ToArray());
      }

      return table;
    }
    
    public static DataTable GetDataTable(this DataGrid grid)
    {
      grid.EnableColumnVirtualization = false;

      grid.EnableRowVirtualization = false;

      grid.UpdateLayout();

      DataTable table = new DataTable(grid.Name);

      string indentationColumn = string.Empty;
      
      int cellCount = -1;

      Dictionary<string, int> headerMappingDic = new Dictionary<string, int>();

      Dictionary<int, string> bindingPaths = new Dictionary<int, string>();

      List<int> skipCellLoadList = new List<int>();

      foreach (DataGridColumn column in grid.Columns)
      {
        cellCount++;

        if (column.Visibility != Visibility.Visible)
        {
          skipCellLoadList.Add(cellCount);

          continue;
        }

        //string columnHeader = column.GetType() == typeof(DWDataGridTextColumn) ? ((DWDataGridTextColumn)column).Header.TGISToString() : column.Header.TGISToString();
        string columnHeader = column.Header.ParseToString();

        table.Columns.Add(columnHeader, typeof(string));

        headerMappingDic.Add(columnHeader, cellCount);

        if (column.GetType() == typeof(DataGridTemplateColumn))
        {
          string[] splitPath = column.SortMemberPath.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);

          if (splitPath.Length == 0 || splitPath[(splitPath.Length - 1)].IsNullEmptyOrWhiteSpace())
          {
            splitPath = new string[] { columnHeader };
          }

          bindingPaths.Add(cellCount, splitPath[(splitPath.Length - 1)]);
        }
        else if (column.GetType() == typeof(DataGridComboBoxColumn))
        {
          bindingPaths.Add(cellCount, ((DataGridComboBoxColumn)column).SortMemberPath);
        }
        else
        {
          Binding columnBinding = ((DataGridBoundColumn)column).Binding as Binding;

          string[] splitPath = columnBinding.Path.Path.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);

          bindingPaths.Add(cellCount, (columnBinding == null ? column.SortMemberPath : splitPath[(splitPath.Length - 1)]));
        }
      }

      int rowCount = grid.Items.Count;

      int initialIndex = grid.SelectedIndex;

      for (int x = 0; x < rowCount; x++)
      {
        grid.SelectedIndex = x;

        DataGridRow item = grid.ItemContainerGenerator.ContainerFromIndex(x) as DataGridRow;

        if (item == null)
        {
          continue;
        }

        object itemRow = grid.ItemsSource.GetByIndex(x);

        object actualRow = itemRow.GetPropertyValue("ActualDataRow");

        DataRow tableRow = table.NewRow();

        int columnIndex = 0;

        for (int c = 0; c <= cellCount; c++)
        {
          if (skipCellLoadList.Contains(c))
          {
            continue;
          }

          DataGridCellInfo cellInf = grid.SelectedCells[c];

          string cellInfoHeader = ((DataGridColumn)cellInf.Column).Header.ParseToString();

          if (!headerMappingDic.ContainsKey(cellInfoHeader))
          {
            continue;
          }

          int mappingIndex = headerMappingDic[cellInfoHeader];

          object cellContent = cellInf.Column.GetCellContent(item);

          if (!indentationColumn.IsNullEmptyOrWhiteSpace() && c == 0)
          {
            tableRow[columnIndex] = actualRow == null ? itemRow.GetPropertyValue(indentationColumn) : actualRow.GetPropertyValue(indentationColumn);
          }
          else if (cellContent != null && cellContent.GetType() == typeof(ContentPresenter))
          {
            tableRow[columnIndex] = GetContentPresenterValue((ContentPresenter)cellContent);
          }
          else if (cellContent != null && cellContent.GetType() == typeof(TextBlock))
          {
            tableRow[columnIndex] = ((TextBlock)cellContent).Text;
          }
          else
          {
            tableRow[columnIndex] = actualRow == null ? itemRow.GetPropertyValue(bindingPaths[mappingIndex]) : actualRow.GetPropertyValue(bindingPaths[mappingIndex]);
          }

          columnIndex++;
        }

        table.Rows.Add(tableRow);
      }

      grid.SelectedIndex = initialIndex;

      grid.EnableColumnVirtualization = true;

      grid.EnableRowVirtualization = true;

      return table;
    }

    private static object GetContentPresenterValue(ContentPresenter contentPresent)
    {
      foreach (UIElement cellItem in contentPresent.FindVisualControls())
      {
        Type cellItemType = cellItem.GetType();

        if (cellItemType == typeof(TextBox) || cellItemType == typeof(TextBox))
        {
          return ((TextBox)cellItem).Text;
        }
        else if (cellItemType == typeof(TextBlock) || cellItemType == typeof(TextBlock))
        {
          return ((TextBlock)cellItem).Text;
        }
        else if (cellItemType == typeof(CheckBox) || cellItemType == typeof(CheckBox))
        {
          return ((CheckBox)cellItem).IsChecked;
        }
        else if (cellItemType == typeof(ComboBox) || cellItemType == typeof(ComboBox))
        {
          return ((ComboBox)cellItem).Text;
        }
      }

      return null;
    }
  }
}
