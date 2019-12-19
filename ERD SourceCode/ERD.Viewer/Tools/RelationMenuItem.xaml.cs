using ERD.Base;
using ERD.Models;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer.Tools
{
  /// <summary>
  /// Interaction logic for RelationMenuItem.xaml
  /// </summary>
  public partial class RelationMenuItem : UserControlBase
  {
    private RelationTypesEnum relationTypeObject;

    public RelationMenuItem(RelationTypesEnum relationType)
    {
      this.InitializeComponent();

      this.relationTypeObject = relationType;

      this.uxRelationTypeName.Content = relationType == RelationTypesEnum.DatabaseRelation ? "Database Relation" : "Virtual Relation";
    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);

      this.Background = Brushes.LightSteelBlue;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);

      this.Background = null;
    }


    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        try
        {
          DataObject data = new DataObject();

          DatabaseRelation dragData = new DatabaseRelation
          {
            RelationType = this.relationTypeObject
          };
          
          data.SetData(DataFormats.StringFormat, "Relation");

          data.SetData(dragData);

          DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }
        catch (Exception err)
        {
          // DO NOHTING: This will break if the relationsships table is not on the same canvas
        }
      }
    }

    protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
    {
      base.OnGiveFeedback(e);
      
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
