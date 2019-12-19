using ERD.Models;
using GeneralExtensions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Tools
{
  /// <summary>
  /// Interaction logic for TableMenuItem.xaml
  /// </summary>
  public partial class TableMenuItem : UserControlBase
  {
    public delegate void TableMenuItemDoubleClickEvent(object sender, MouseButtonEventArgs e);

    public event TableMenuItemDoubleClickEvent TableMenuItemDoubleClick;

    private bool allowDragDrop;

    public TableMenuItem(TableModel tableModel)
    {
      this.InitializeComponent();

      this.TableModelObject = tableModel;

      this.uxTableName.Content = this.TableModelObject.TableName;

      this.uxTabLocation.Content = this.TableModelObject.ErdSegmentModelName;

      this.allowDragDrop = this.TableModelObject.ErdSegmentModelName.IsNullEmptyOrWhiteSpace();
    }
    
    public double DesiredNameWidth
    {
      get
      {
        return this.uxTableName.TextLenght;
      }

      set
      {
        this.uxTableName.Width = value;
      }
    }

    public string TableName
    {
      get
      {
        if (this.uxTableName.Content == null)
        {
          return string.Empty;
        }

        return this.uxTableName.Original.ToString();
      }
    }

    public string ErdCanvas
    {
      get
      {
        if (this.uxTabLocation.Content == null)
        {
          return string.Empty;
        }

        return this.uxTabLocation.Content.ToString();
      }
    }
    
    public TableModel TableModelObject {get; private set;}
    
    //public void SetErdCanvas(string erdSegmentModelName)
    //{
    //  this.allowDragDrop = erdSegmentModelName.IsNullEmptyOrWhiteSpace();

    //  this.uxTabLocation.Content = erdSegmentModelName;
    //}

    public void SetErdCanvas(string erdSegmentModelName, bool mayDrop)
    {
      this.allowDragDrop = mayDrop;

      this.uxTabLocation.Content = erdSegmentModelName;
    }
    
    private void OnMouse_DoubleClick(object sender, MouseButtonEventArgs e)
    {
      this.TableMenuItemDoubleClick?.Invoke(this, e);
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      this.Background = Brushes.LightSteelBlue;

      this.uxTabLocation.Foreground = Brushes.DarkSlateGray;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

      this.Background = null;

      this.uxTabLocation.Foreground = Brushes.LightGray;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (!this.allowDragDrop)
      {
        return;
      }

      if (e.LeftButton == MouseButtonState.Pressed)
      {
        DataObject data = new DataObject();

        data.SetData(DataFormats.StringFormat, this.TableModelObject.TableName);
        data.SetData(this.TableModelObject);

        DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
      }
    }

    protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
      base.OnGiveFeedback(e);

      if (!this.allowDragDrop)
      {
        Mouse.SetCursor(Cursors.None);

        return;
      }

      if (e.Effects.HasFlag(DragDropEffects.Copy))
      {
        Mouse.SetCursor(Cursors.Cross);
      }
      else if (e.Effects.HasFlag(DragDropEffects.Move))
      {
        Mouse.SetCursor(Cursors.Pen);
      }
      else
      {
        Mouse.SetCursor(Cursors.No);
      }
      e.Handled = true;
    }
  }
}
