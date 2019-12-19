using ERD.Base;
using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Exstention;
using WPF.Tools.ToolModels;

namespace ERD.Viewer.Tools.Relations
{
  /// <summary>
  /// Interaction logic for RelationEditor.xaml
  /// </summary>
  public partial class RelationEditor : WindowBase
  {
    private const string parentComboName = "ParentColumn_";

    private const string childComboName = "ChildColumn_";

    private bool wasLoaded = false;

    private DataItemModel selectedParentTable;

    private DataItemModel selectedChildTable;

    private DataItemModel[] tableList;

    public RelationEditor(DatabaseRelation relation)
    {
      this.InitializeComponent();

      this.DataContext = this;

      this.DatabaseRelation = relation;

      this.TablesList = Integrity.GetSystemTables().ToArray();

      if (!relation.ParentTable.IsNullEmptyOrWhiteSpace() &&
          !relation.ChildTable.IsNullEmptyOrWhiteSpace())
      {
        this.Loaded += this.RelationEditor_Loaded;
      }
    }

    public DatabaseRelation DatabaseRelation {get; private set;}

    public DataItemModel SelectedParentTable
    {
      get
      {
        return this.selectedParentTable;
      }

      set
      {
        this.selectedParentTable = value;

        base.OnPropertyChanged(() => this.SelectedParentTable);

        this.uxParentColumns.Children.Clear();

        this.uxChildColumns.Children.Clear();

        this.AddColumnSelection();
      }
    }

    public DataItemModel SelectedChildTable
    {
      get
      {
        return this.selectedChildTable;
      }

      set
      {
        this.selectedChildTable = value;

        base.OnPropertyChanged(() => this.SelectedChildTable);

        this.uxParentColumns.Children.Clear();

        this.uxChildColumns.Children.Clear();

        this.AddColumnSelection();
      }
    }

    public DataItemModel[] TablesList
    {
      get
      {
        return this.tableList;
      }

      set
      {
        this.tableList = value;

        base.OnPropertyChanged(() => this.TablesList);
      }
    }

    private void RelationEditor_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.SelectedParentTable = this.TablesList.FirstOrDefault(pt => pt.ItemKey.ToString() == this.DatabaseRelation.ParentTable);

        this.SelectedChildTable = this.TablesList.FirstOrDefault(pt => pt.ItemKey.ToString() == this.DatabaseRelation.ChildTable);

        UIElement[] childBoxes = this.uxChildColumns.FindVisualControls(typeof(ComboBoxTool));

        List<ColumnRelationMapModel> actualRelations = new List<ColumnRelationMapModel>();

        foreach (ComboBoxTool parentBox in this.uxParentColumns.FindVisualControls(typeof(ComboBoxTool)))
        {
          if (parentBox.SelectedValue == null)
          { // Vertual Relation Empty Box
            continue;
          }

          ColumnRelationMapModel relation = this.DatabaseRelation.Columns.FirstOrDefault(r => r.ParentColumn == parentBox.SelectedValue.ToString());

          if (relation == null)
          {
            continue;
          }

          string parentIndex = parentBox.Name.Split(new char[] {'_'})[1];

          string childStartName = $"{childComboName}{parentIndex}";

          ComboBoxTool childBox = (ComboBoxTool) childBoxes.FirstOrDefault(c => ((ComboBoxTool) c).Name.StartsWith(childStartName));

          actualRelations.Add(relation);

          childBox.SelectedValue = relation.ChildColumn;
        }

        this.DatabaseRelation.Columns.Clear();

        this.DatabaseRelation.Columns.AddRange(actualRelations);

        this.wasLoaded = true;
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }

    private void Accept_Cliked(object sender, RoutedEventArgs e)
    {
      try
      {
        Dictionary<string, string> childToParentRelation = new Dictionary<string, string>();
        
        string parentTable = this.SelectedParentTable.ItemKey.ToString();

        string childTable = this.SelectedChildTable.ItemKey.ToString();

        UIElement[] parentBoxes = this.uxParentColumns.FindVisualControls(typeof(ComboBoxTool));

        foreach (ComboBoxTool childBox in this.uxChildColumns.Children)
        {
          string parentBoxName = childBox.Name.Replace(childComboName, parentComboName);

          ComboBoxTool parentBox = (ComboBoxTool)parentBoxes.First(b => ((ComboBoxTool) b).Name == parentBoxName);

          if (childBox .SelectedItem == null || parentBox.SelectedItem == null)
          {
            continue;
          }

          string childColumnName = ((DataItemModel)childBox.SelectedItem).ItemKey.ToString();

          string parentColumnName = ((DataItemModel)parentBox.SelectedItem).ItemKey.ToString();

          if (childToParentRelation.ContainsKey(childColumnName))
          {
            throw new ApplicationException($"Cannot have duplicate selections for child {childBox.SelectedItem}.");
          }

          if (Integrity.GetGlobalColumnDataType(childColumnName) != Integrity.GetGlobalColumnDataType(parentColumnName))
          {
            throw new ApplicationException($"Inconsistent Data Types {parentColumnName} – {childColumnName}.");
          }

          childToParentRelation.Add(childColumnName, parentColumnName);
        }

        this.DatabaseRelation.ParentTable = parentTable;

        this.DatabaseRelation.ChildTable = childTable;

        this.DatabaseRelation.RelationshipName = this.uxRelationName.Content.ToString();

        this.DatabaseRelation.Columns.Clear();

        foreach (ColumnObjectModel column in Integrity.GetObjectModel(childTable))
        {
          if (!childToParentRelation.ContainsKey(column.ColumnName))
          {
            continue;
          }

          column.ForeignConstraintName = this.DatabaseRelation.RelationshipName;

          column.ForeignKeyColumn = childToParentRelation[column.ColumnName];

          column.ForeignKeyTable = this.DatabaseRelation.ParentTable;

          column.IsVertualRelation = (this.DatabaseRelation.RelationType == RelationTypesEnum.VirtualRelation);

          column.IsForeignkey = true;

          this.DatabaseRelation.Columns.Add(new ColumnRelationMapModel
          {
            ChildColumn = column.ColumnName,
            ChildTable = this.DatabaseRelation.ChildTable,
            ForeignConstraintName = this.DatabaseRelation.RelationshipName,
            ParentColumn = childToParentRelation[column.ColumnName],
            ParentTable = this.DatabaseRelation.ParentTable
          });
        }

        this.DialogResult = true;

        //this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }
    
    private void ChildBox_Closed(object sender, EventArgs e)
    {
      try
      {
        ComboBoxTool box = (ComboBoxTool) sender;

        if (box.SelectedItem == null)
        {
          return;
        }

        box.DropDownClosed -= this.ChildBox_Closed;

        this.AddColumnSelection();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.GetFullExceptionMessage());
      }
    }

    private void AddColumnSelection()
    {
      if (this.SelectedParentTable == null || this.SelectedChildTable == null)
      {
        return;
      }

      if (this.DatabaseRelation.RelationType == RelationTypesEnum.VirtualRelation)
      {
        this.VirtualTypeLink();
      }
      else
      {
        this.DataBaseTypeLink();
      }
    }

    private void DataBaseTypeLink()
    {
      this.uxRelationName.Content = Integrity.BuildForeighKeyName(this.SelectedParentTable.ItemKey.ToString(), this.SelectedChildTable.ItemKey.ToString());

      foreach (ColumnObjectModel keyColumn in Integrity.GetObjectModel(this.SelectedParentTable.ItemKey.ToString()).Where(pk => pk.InPrimaryKey))
      {
        ComboBoxTool parentBox = new ComboBoxTool();

        ComboBoxTool childBox = new ComboBoxTool();

        parentBox.Name = $"{parentComboName}{this.uxParentColumns.Children.Count}";

        childBox.Name = $"{childComboName}{this.uxChildColumns.Children.Count}";

        parentBox.Items.Add(new DataItemModel { DisplayValue =  keyColumn.ColumnName, ItemKey = keyColumn.ColumnName});

        foreach (DataItemModel item in Integrity.GetColumnsForTable(this.SelectedChildTable.ItemKey.ToString()))
        {
          childBox.Items.Add(item);
        }

        parentBox.SelectedValue = keyColumn.ColumnName;
      
        this.uxParentColumns.Children.Add(parentBox);

        this.uxChildColumns.Children.Add(childBox);
      }
    }

    private void VirtualTypeLink()
    {
      this.uxRelationName.Content = Integrity.BuildForeighKeyName(this.SelectedParentTable.ItemKey.ToString(), this.SelectedChildTable.ItemKey.ToString());

      if (!this.wasLoaded)
      {
        foreach (ColumnRelationMapModel column in this.DatabaseRelation.Columns)
        {
          ComboBoxTool parentBox = new ComboBoxTool();

          ComboBoxTool childBox = new ComboBoxTool();

          parentBox.Name = $"{parentComboName}{this.uxParentColumns.Children.Count}";

          childBox.Name = $"{childComboName}{this.uxChildColumns.Children.Count}";

          foreach (DataItemModel item in Integrity.GetColumnsForTable(this.SelectedParentTable.ItemKey.ToString()))
          {
            parentBox.Items.Add(item);
          }

          foreach (DataItemModel item in Integrity.GetColumnsForTable(this.SelectedChildTable.ItemKey.ToString()))
          {
            childBox.Items.Add(item);
          }

          childBox.DropDownClosed += this.ChildBox_Closed;

          parentBox.SelectedValue = column.ParentColumn;

          this.uxParentColumns.Children.Add(parentBox);

          this.uxChildColumns.Children.Add(childBox);
        }
      }

      ComboBoxTool emptyParentBox = new ComboBoxTool();

      ComboBoxTool emptyChildBox = new ComboBoxTool();

      emptyParentBox.Name = $"{parentComboName}{this.uxParentColumns.Children.Count}";

      emptyChildBox.Name = $"{childComboName}{this.uxChildColumns.Children.Count}";
      
      foreach (DataItemModel item in Integrity.GetColumnsForTable(this.SelectedParentTable.ItemKey.ToString()))
      {
        emptyParentBox.Items.Add(item);
      }

      foreach (DataItemModel item in Integrity.GetColumnsForTable(this.SelectedChildTable.ItemKey.ToString()))
      {
        emptyChildBox.Items.Add(item);
      }

      emptyChildBox.DropDownClosed += this.ChildBox_Closed;

      //parentBox.SelectedValue = keyColumn.ColumnName;

      this.uxParentColumns.Children.Add(emptyParentBox);

      this.uxChildColumns.Children.Add(emptyChildBox);
    }
  }
}
