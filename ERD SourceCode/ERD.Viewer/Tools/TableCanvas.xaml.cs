using ERD.Models;
using ERD.Viewer.Tables;
using GeneralExtensions;
using System;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Tools
{
  /// <summary>
  /// Interaction logic for TableCanvas.xaml
  /// </summary>
  public partial class TableCanvas : UserControlBase
  {
    public delegate void TableAddedEvent(object sender, TableModel table);

    public delegate void TableRemovedEvent(object sender, TableModel tableModel);

    public event TableAddedEvent TableAdded;

    public event TableRemovedEvent TableRemoved;

    public TableCanvas(ErdCanvasModel erdSegment, DatabaseModel databaseModel)
    {
      this.InitializeComponent();

      this.ErdSegment = erdSegment;

      this.uxTableCanvas.NewTablePrefix = this.ErdSegment.TablePrefix;

      foreach (TableModel table in this.ErdSegment.SegmentTables)
      {
        this.uxTableCanvas.CreateTableObject(table);
      }

      this.uxTableCanvas.DatabaseModel = databaseModel;

      this.uxTableCanvas.TableAdded += this.Table_Added;

      this.uxTableCanvas.RemoveTable += this.Table_Removed;

      this.SizeChanged += this.TableCanvas_SizeChanged;
      
      this.uxTabMetadata.Content = $"Tables Prefix: {this.ErdSegment.TablePrefix}";
    }
    
    private void Table_Removed(object sender, TableModel tableModel)
    {
      if (this.TableRemoved != null)
      {
        this.TableRemoved(this, tableModel);
      }
    }

    public TableModel this[string tableName]
    {
      get
      {
        return this.ErdSegment.SegmentTables.FirstOrDefault(tn => tn.TableName == tableName);
      }
    }

    public ErdCanvasModel ErdSegment {get; set;}

    public void ZoomTo(Point location)
    {
      this.uxCanvasScroll.ZoomTo(location);
    }
    
    private void TableObjet_Move(object sender, bool isDrag)
    {
      this.uxCanvasScroll.IsPannable = !isDrag;
    }
    
    private void Table_Added(object sender, TableModel table)
    {
      if (!this.ErdSegment.SegmentTables.Any(t => t.TableName == table.TableName))
      {
        table.ErdSegmentModelName = this.ErdSegment.ModelSegmentName;

        this.ErdSegment.SegmentTables.Add(table);

        if (this.TableAdded != null)
        {
          this.TableAdded(this, table);
        }
      }
    }

    private void AdditionalTables_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        SelectedTables selector = new SelectedTables(this.ErdSegment.IncludeInContextBuild.ToArray());

        bool? result = selector.ShowDialog();

        if (!result.IsTrue())
        {
          return;
        }

        this.ErdSegment.IncludeInContextBuild.Clear();

        this.ErdSegment.IncludeInContextBuild.AddRange(selector.SelectedModels());

      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }

    private void TableCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.uxTableCanvas.MinWidth = this.uxCanvasScroll.ActualWidth + 1000;

      this.uxTableCanvas.MinHeight = this.uxCanvasScroll.ActualHeight + 1000;
    }
  }
}
