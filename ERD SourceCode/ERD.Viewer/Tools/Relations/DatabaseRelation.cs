using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ERD.Base;
using ERD.Viewer.Common;
using ERD.Viewer.Enumiration;
using ERD.Viewer.Models;
using WPF.Tools.ModelViewer;

namespace ERD.Viewer.Tools.Relations
{
  public class DatabaseRelation
  {
    private Path[] linePath;

    public DatabaseRelation()
    {
      this.Columns = new List<ColumnRelationMapModel>();
    }
    
    internal string ParentTable { get; set; }

    internal string ChildTable { get; set; }
    
    internal string RelationshipName {get; set;}
    
    internal List<ColumnRelationMapModel> Columns { get; set; }

    internal Point ParentTablePoint { get; set; }

    internal Point ChildTablePoint { get; set; }

    internal RelationTypesEnum RelationType {get; set;}

    internal Brush LineColourBrush
    {
      get
      {
        return this.RelationType == RelationTypesEnum.DatabaseRelation ? Brushes.LightSteelBlue : Brushes.LightGreen;
      }
    }

    internal Point[] IntersectionPoints { get; set; }

    internal Path[] LinePath
    {
      get
      {
        if (this.linePath != null)
        {
          return this.linePath;
        }

        List<Point> linePoints = this.LinePoints;

        PathGeometry pathLine = new PathGeometry();

        PathFigure figure = new PathFigure();

        figure.StartPoint = linePoints[0];
        
        for (int i = 1; i < linePoints.Count; i++)
        {
          figure.Segments.Add(new LineSegment(linePoints[i], true));
        }

        

        pathLine.Figures.Add(figure);

        this.linePath = new Path[2];

        this.linePath[0] = new Path();

        this.linePath[0].StrokeThickness = 3;

        this.linePath[0].Stroke = this.LineColourBrush;

        this.linePath[0].Data = pathLine;

        this.linePath[0].Name = $"A_{this.RelationshipName}";

        this.linePath[1] = new Path();
        
        this.linePath[1].StrokeThickness = 3;

        this.linePath[1].StrokeDashArray = new DoubleCollection() {4, 2};

        this.linePath[1].Stroke = this.LineColourBrush;

        this.linePath[1].Data = pathLine;

        this.linePath[1].Name = $"B_{this.RelationshipName}";

        ModelViewerToolTip toolTip = new ModelViewerToolTip();

        toolTip.ClassObjects.AddRange(this.Columns.ToArray());

        this.linePath[0].ToolTip = toolTip;

        this.linePath[1].ToolTip = toolTip;

        ContextMenu path0Context = new ContextMenu();

        ContextMenu path1Context = new ContextMenu();

        MenuItem edit0 = new MenuItem { Header = "Edit" };

        MenuItem edit1 = new MenuItem { Header = "Edit" };

        edit0.Click += this.LineEdit_Cliked;

        edit1.Click += this.LineEdit_Cliked;

        path0Context.Items.Add(edit0);

        path1Context.Items.Add(edit1);



        MenuItem delete0 = new MenuItem { Header = "Delete" };

        MenuItem delete1 = new MenuItem { Header = "Delete" };

        delete0.Click += this.LineDelete_Cliked;

        delete1.Click += this.LineDelete_Cliked;

        path0Context.Items.Add(delete0);

        path1Context.Items.Add(delete1);



        this.linePath[0].ContextMenu = path0Context;

        this.linePath[1].ContextMenu = path1Context;

        return this.linePath;
      }
    }
    
    internal void ClearPath()
    {
      this.linePath = null;
    }

    private void LineEdit_Cliked(object sender, RoutedEventArgs e)
    {
      RelationEditor editor = new RelationEditor(this);

      editor.ShowDialog();
    }
    
    private void LineDelete_Cliked(object sender, RoutedEventArgs e)
    {
      string message = $"Are you sure you want to delete the relation between {this.ParentTable} and {this.ChildTable}?";

      if (MessageBox.Show(message, "Warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
      {
        return;
      }

      Integrity.RemoveForeighKey(this);

      foreach (Path item in this.linePath)
      {
        item.Visibility = Visibility.Collapsed;
      }
    }

    private List<Point> LinePoints
    {
      get
      {
        List<Point> results = new List<Point>();

        results.Add(this.ParentTablePoint);

        if (this.IntersectionPoints != null)
        {
          results.AddRange(this.IntersectionPoints);
        }

        results.Add(this.ChildTablePoint);

        return results;
      }
    }
  }
}
