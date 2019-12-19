using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using ERD.Viewer.Enumiration;
using ERD.Viewer.Models;
using WPF.Tools.ModelViewer;

namespace ERD.Viewer.Tools.Relations
{
  public abstract class RelationshipDrawing
  {
    private Path[] linePath;

    internal RelationshipDrawing()
    {
      this.LineColourBrush = Brushes.WhiteSmoke;

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
      get;
      set;
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

        return this.linePath;
      }
    }

    internal void ClearPath()
    {
      this.linePath = null;
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

