using ERD.Base;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;

namespace ERD.Models
{
    public class DatabaseRelation : IDisposable
    {
        public delegate void DatabaseRelationDeleteEvent(DatabaseRelation sender);
        public event DatabaseRelationDeleteEvent DatabaseRelationDelete;
        private Path[] linePath;
        private bool isSelected;

        public DatabaseRelation()
        {
            this.Columns = new List<ColumnRelationMapModel>();
        }

        public string ParentTable { get; set; }

        public string ChildTable { get; set; }

        public string RelationshipName { get; set; }

        public List<ColumnRelationMapModel> Columns { get; set; }

        public Point ParentTablePoint { get; set; }

        public Point ChildTablePoint { get; set; }

        public RelationTypesEnum RelationType { get; set; }

        public bool IsChildOptional { get; set; }

        public Brush LineColourBrush
        {
            get
            {
                return this.RelationType == RelationTypesEnum.DatabaseRelation ? Brushes.LightSteelBlue : Brushes.LightGreen;
            }
        }

        public Point[] IntersectionPoints { get; set; }

        public Path[] LinePath
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

                figure.IsFilled = false;

                for (int i = 1; i < linePoints.Count; i++)
                {
                    figure.Segments.Add(new LineSegment(linePoints[i], true));
                }

                pathLine.Figures.Add(figure);

                PathFigure parentOneMarker = this.CreateOneMarkerFigure(this.ParentTablePoint, linePoints.Count > 1 ? linePoints[1] : this.ChildTablePoint);

                if (parentOneMarker != null)
                {
                    pathLine.Figures.Add(parentOneMarker);
                }

                bool hasCrowFootChildMarker = this.IntersectionPoints != null && this.IntersectionPoints.Length >= 7;

                if (!hasCrowFootChildMarker)
                {
                    PathFigure childOneMarker = this.CreateOneMarkerFigure(this.ChildTablePoint, linePoints.Count > 1 ? linePoints[linePoints.Count - 2] : this.ParentTablePoint);

                    if (childOneMarker != null)
                    {
                        pathLine.Figures.Add(childOneMarker);
                    }
                }

                if (this.IsChildOptional)
                {
                    PathFigure childOptionalMarker = this.CreateOptionalMarkerFigure(this.ChildTablePoint, linePoints.Count > 1 ? linePoints[linePoints.Count - 2] : this.ParentTablePoint);

                    if (childOptionalMarker != null)
                    {
                        pathLine.Figures.Add(childOptionalMarker);
                    }
                }

                this.linePath = new Path[2];

                this.linePath[0] = new Path(); // { Name = $"LINE_{Guid.NewGuid().ToString().Replace("-", "_")}" };

                this.linePath[0].StrokeThickness = 2;

                this.linePath[0].StrokeLineJoin = PenLineJoin.Round;

                this.linePath[0].StrokeStartLineCap = PenLineCap.Round;

                this.linePath[0].StrokeEndLineCap = PenLineCap.Round;

                this.linePath[0].SnapsToDevicePixels = true;

                this.linePath[0].Stroke = this.LineColourBrush;

                this.linePath[0].Data = pathLine;

                this.linePath[0].Name = $"A_{this.RelationshipName}";

                this.linePath[1] = new Path(); // { Name = $"LINE_{Guid.NewGuid().ToString().Replace("-", "_")}" };

                this.linePath[1].StrokeThickness = 5;

                this.linePath[1].Stroke = Brushes.Transparent;

                this.linePath[1].Fill = null;

                this.linePath[1].Data = pathLine;

                this.linePath[1].Name = $"B_{this.RelationshipName}";

                this.linePath[0].MouseEnter += this.Mouse_Enter;
                this.linePath[1].MouseEnter += this.Mouse_Enter;

                this.LinePath[0].MouseLeave += this.Mouse_Leave;
                this.LinePath[1].MouseLeave += this.Mouse_Leave;

                this.linePath[0].PreviewMouseLeftButtonUp += this.Line_Clicked;
                this.linePath[1].PreviewMouseLeftButtonUp += this.Line_Clicked;

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

        public void ClearPath()
        {
            this.linePath = null;
        }

        public void Dispose()
        {
            this.ClearPath();

            this.Columns.Clear();
        }


        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            this.linePath[0].Stroke = Brushes.Orange;
            this.linePath[1].Stroke = Brushes.Transparent;
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            this.linePath[0].Stroke = this.isSelected ? Brushes.Red : this.LineColourBrush;
            this.linePath[1].Stroke = Brushes.Transparent;
        }

        private void Line_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (this.isSelected)
            {
                this.isSelected = false;
                this.LinePath[0].Stroke = this.LineColourBrush;
                this.LinePath[1].Stroke = Brushes.Transparent;
            }
            else
            {
                this.isSelected = true;
                this.LinePath[0].Stroke = Brushes.Red;
                this.LinePath[1].Stroke = Brushes.Transparent;
            }
        }

        private void LineEdit_Cliked(object sender, RoutedEventArgs e)
        {
            Type controlType = Type.GetType("ERD.Viewer.Tools.Relations.RelationEditor,ERD.Viewer");

            WindowBase editor = Activator.CreateInstance(controlType, new object[] { this }) as WindowBase;

            //RelationEditor editor = new RelationEditor(this);

            editor.ShowDialog();
        }

        private void LineDelete_Cliked(object sender, RoutedEventArgs e)
        {
            string message = $"Are you sure you want to delete the relation between {this.ParentTable} and {this.ChildTable}?";

            if (MessageBox.Show(message, "Warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            this.DatabaseRelationDelete?.Invoke(this);
        }

        private PathFigure CreateOneMarkerFigure(Point anchorPoint, Point directionPoint)
        {
            Vector direction = directionPoint - anchorPoint;

            if (direction.Length < 0.001)
            {
                return null;
            }

            direction.Normalize();

            Vector perpendicular = new Vector(-direction.Y, direction.X);

            const double markerOffset = 8.0;
            const double markerHalfLength = 5.0;

            Point markerCenter = anchorPoint + (direction * markerOffset);

            Point markerStart = markerCenter + (perpendicular * markerHalfLength);
            Point markerEnd = markerCenter - (perpendicular * markerHalfLength);

            PathFigure markerFigure = new PathFigure();
            markerFigure.StartPoint = markerStart;
            markerFigure.Segments.Add(new LineSegment(markerEnd, true));

            return markerFigure;
        }

        private PathFigure CreateOptionalMarkerFigure(Point anchorPoint, Point directionPoint)
        {
            Vector direction = directionPoint - anchorPoint;

            if (direction.Length < 0.001)
            {
                return null;
            }

            direction.Normalize();

            const double markerOffset = 10.0;
            const double markerRadius = 3.5;
            const int segments = 12;

            Point center = anchorPoint + (direction * markerOffset);

            PathFigure circleFigure = new PathFigure();
            circleFigure.IsClosed = true;
            circleFigure.IsFilled = false;

            Point startPoint = new Point(center.X + markerRadius, center.Y);
            circleFigure.StartPoint = startPoint;

            for (int i = 1; i <= segments; i++)
            {
                double angle = (Math.PI * 2 * i) / segments;
                Point point = new Point(
                    center.X + (markerRadius * Math.Cos(angle)),
                    center.Y + (markerRadius * Math.Sin(angle)));

                circleFigure.Segments.Add(new LineSegment(point, true));
            }

            return circleFigure;
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
