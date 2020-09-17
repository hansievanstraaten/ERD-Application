using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF.Tools.Mesurements
{
    internal class Marker : Shape, IDisposable
    {
        private bool isCaptured = false;

        private PathGeometry path;

        private Line trace = new Line();

        private Brush markerColor = Brushes.DarkBlue;

        private Brush traceColor = Brushes.Green;

        public Marker()
        {
            this.Loaded += Marker_Loaded;

            this.InitializeMarker();
        }

        public Marker(bool isStatic)
        {
            //NOTE: DO NOT INHERIT from this() as we require the new color before initialization
            this.IsStatic = isStatic;

            this.Loaded += Marker_Loaded;

            this.InitializeMarker();
        }

        public RulerOrientationEnum Orientation
        {
            get;

            set;
        }

        public double X
        {
            get
            {
                return Canvas.GetLeft(this);
            }

            set
            {
                if (value < 0 || value > this.ParentOffset)
                {
                    return;
                }

                Canvas.SetLeft(this, value);

                Canvas.SetLeft(this.trace, value);
            }
        }

        public double Y
        {
            get
            {
                return Canvas.GetTop(this);
            }

            set
            {
                if (value < 0 || value > this.ParentOffset)
                {
                    return;
                }

                Canvas.SetTop(this, value);

                Canvas.SetTop(this.trace, value);
            }
        }

        public double CanvasX
        {
            get;

            set;
        }

        public double CanvasY
        {
            get;

            set;
        }

        public double TraceLength
        {
            get
            {
                return this.ParentTrace;
            }

            set
            {
                //if (value.IsNaN())
                //{
                //    value = 0;
                //}

                if (this.Orientation == RulerOrientationEnum.Vertical)
                {
                    this.trace.X1 = value;
                }
                else
                {
                    this.trace.Y1 = value;
                }
            }
        }

        public bool IsStatic
        {
            get
            {
                return this.isStaticMarker;
            }

            set
            {
                this.isStaticMarker = value;

                if (value)
                {
                    this.markerColor = Brushes.DarkRed;

                    this.traceColor = Brushes.Red;
                }
            }
        }

        public void Dispose()
        {
            FrameworkElement parent = (FrameworkElement)this.Parent;

            if (parent.GetType() == typeof(RulerControl))
            {
                ((Canvas)parent).Children.Remove(this.trace);
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (this.Orientation == RulerOrientationEnum.Vertical)
                {
                    this.VerticalSetup();
                }
                else
                {
                    this.HorizontalSetup();
                }

                return path;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            Mouse.Capture(this);

            this.isCaptured = true;

            this.CanvasX = e.GetPosition((System.Windows.UIElement)this.Parent).X;

            this.CanvasY = e.GetPosition((System.Windows.UIElement)this.Parent).Y;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Mouse.Capture(null);

            this.isCaptured = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.isCaptured)
            {
                double currentX = e.GetPosition((System.Windows.UIElement)this.Parent).X;

                double currentY = e.GetPosition((System.Windows.UIElement)this.Parent).Y;

                if (this.Orientation == RulerOrientationEnum.Horizontal)
                {
                    this.X += currentX - this.CanvasX;

                    this.CanvasX = currentX;
                }

                if (this.Orientation == RulerOrientationEnum.Vertical)
                {
                    this.Y += currentY - this.CanvasY;

                    this.CanvasY = currentY;
                }
            }
        }

        private void Marker_Loaded(object sender, RoutedEventArgs e)
        {
            this.AddTraceToparent();
        }

        private double ParentOffset
        {
            get
            {
                FrameworkElement parent = (FrameworkElement)this.Parent;

                if (this.Orientation == RulerOrientationEnum.Horizontal)
                {
                    return parent.Width;
                }

                return parent.Height - this.Height;
            }
        }

        private double ParentTrace
        {
            get
            {
                RulerControl parent = (RulerControl)this.Parent;

                return parent.TraceLength;
            }
        }

        private void VerticalSetup()
        {
            PathFigure fig = new PathFigure { StartPoint = new System.Windows.Point(0, 0) };

            LineSegment seg1 = new LineSegment { Point = new System.Windows.Point(0, this.Height) };

            LineSegment seg2 = new LineSegment { Point = new System.Windows.Point(this.Height, this.Height / 2) };

            PathSegmentCollection collect = new PathSegmentCollection();

            collect.Add(seg1);

            collect.Add(seg2);

            fig.Segments = collect;

            PathFigureCollection pathFig = new PathFigureCollection();

            pathFig.Add(fig);

            this.path.Figures = pathFig;

            this.trace.Y1 = this.Height / 2;

            this.trace.Y2 = this.Height / 2;

            this.trace.X1 = this.TraceLength;

            this.trace.X2 = this.Height;
        }

        private void HorizontalSetup()
        {
            PathFigure arrowStart = new PathFigure { StartPoint = new System.Windows.Point(0, 0) };

            LineSegment arrowRight = new LineSegment { Point = new System.Windows.Point(this.Height, 0) };

            LineSegment arrowBottom = new LineSegment { Point = new System.Windows.Point(this.Height / 2, this.Height) };

            PathSegmentCollection arrow = new PathSegmentCollection();

            arrow.Add(arrowRight);

            arrow.Add(arrowBottom);

            arrowStart.Segments = arrow;

            PathFigureCollection pathFig = new PathFigureCollection();

            pathFig.Add(arrowStart);

            this.path.Figures = pathFig;

            this.trace.X1 = this.Height / 2;

            this.trace.X2 = this.Height / 2;

            this.trace.Y1 = this.TraceLength;

            this.trace.Y2 = this.Height;
        }

        private void InitializeMarker()
        {
            this.path = new PathGeometry();

            this.trace = new Line();

            this.Orientation = RulerOrientationEnum.Vertical;

            this.Fill = this.markerColor;

            this.Stroke = this.markerColor;

            this.StrokeThickness = 0.5;

            this.Height = 10;

            this.trace.StrokeThickness = 0.8;

            this.trace.Stroke = this.traceColor;
        }

        private void AddTraceToparent()
        {
            if (this.Parent == null)
            {
                return;
            }

            FrameworkElement parent = (FrameworkElement)this.Parent;

            if (parent.GetType() == typeof(RulerControl))
            {
                try
                {
                    Canvas.SetLeft(this.trace, this.X);

                    Canvas.SetTop(this.trace, this.Y);

                    ((Canvas)parent).Children.Add(this.trace);
                }
                catch
                {
                    // DO NOTHING
                }
            }
        }

        public bool isStaticMarker { get; set; }
    }
}
