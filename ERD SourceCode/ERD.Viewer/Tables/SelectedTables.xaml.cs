using ERD.Models;
using ERD.Viewer.Tools;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Functions;

namespace ERD.Viewer.Tables
{
  /// <summary>
  /// Interaction logic for SelectedTables.xaml
  /// </summary>
  public partial class SelectedTables : WindowBase
  {
    private IncludeTableModel[] included;

    public SelectedTables(IncludeTableModel[] includedTables)
    {
      this.InitializeComponent();

      this.included = includedTables;

      this.Initialize();
    }

    public IncludeTableModel[] SelectedModels()
    {
      List<IncludeTableModel> result = new List<IncludeTableModel>();

      foreach (TreeViewItemTool item in this.uxTables.Items)
      {
        if (!item.IsChecked)
        {
          continue;
        }

        TableModel table = item.Tag.To<TableModel>();

        result.Add(new IncludeTableModel { FriendlyName = table.FriendlyName, TableName = table.TableName });
      }

      return result.ToArray();
    }
    
    private void Accept_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        this.DialogResult = true;

        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }
    
    private void Initialize()
    {
      var resulItems = (EventParser.ParseQuery(this, new ParseQueryEventArguments {Title = "QueryTablesStack" }));

      foreach (TableMenuItem item in ((List<TableMenuItem>)resulItems).OrderBy(n => n.TableName))
      {
        TableModel table = item.TableModelObject;

        TreeViewItemTool tool = new TreeViewItemTool
        {
          Header = table.TableName,
          Tag = table,
          IsCheckBox = true,
          IsChecked = included.Any(tn => tn.TableName == item.TableName)
        };

        this.uxTables.Items.Add(tool);
      }
    }
  }
}
