using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace WPF.Tools.CommonControls
{
    public class ScrollViewerPanable : ScrollViewer
    {
        private bool isMouseCaptured = false;

        private bool stopScroll = false;

        private bool isPannable;

        private Point scrollStartPoint;

        private Point scrollStartOffset;

        public ScrollViewerPanable()
        {
            this.Initialize();

            this.IsPannable = false;

            this.UseLayoutRounding = true;

            this.VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor;

            this.SnapsToDevicePixels = true;

            this.VisualClearTypeHint = ClearTypeHint.Enabled;
        }

        public bool IsPannable
        {
            get
            {
                return this.isPannable;
            }

            set
            {
                this.isPannable = value;

                this.HorizontalScrollBarVisibility = value ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;

                this.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
            }
        }

        public bool CanZoom
        {
            get;

            set;
        }

        public void BringToView(Point location)
        {
            try
            {
                double currentZoom = 1.0;

                FrameworkElement element = this.Content as FrameworkElement;

                if (element?.LayoutTransform is TransformGroup transGroup)
                {
                    ScaleTransform scaleTransform = transGroup.Children
                        .OfType<ScaleTransform>()
                        .FirstOrDefault();

                    if (scaleTransform != null)
                    {
                        currentZoom = scaleTransform.ScaleX;
                    }
                }

                while (currentZoom < 0.9)
                {   // The canvas is zommed out to far to read, so zoom it in for the user to see the table.
                    currentZoom = this.ZoomTo(location, 1);
                }

                double targetX = location.X * currentZoom;
                double targetY = location.Y * currentZoom;

                this.ScrollToHorizontalOffset(targetX - 50);
                this.ScrollToVerticalOffset(targetY - 50);
            }
            catch
            {
                // DO NOTHING
            }
        }

        public double ZoomTo(Point centerPoint, int delta)
        {
            FrameworkElement element = this.Content as FrameworkElement;

            if (element != null)
            {
                ScaleTransform scaleTransform = null;

                if (element.LayoutTransform is TransformGroup transGroup)
                {
                    scaleTransform = transGroup.Children
                        .OfType<ScaleTransform>()
                        .FirstOrDefault();
                }
                else
                {
                    transGroup = new TransformGroup();
                    scaleTransform = new ScaleTransform();

                    transGroup.Children.Add(scaleTransform);
                    element.LayoutTransform = transGroup;
                }

                double zoom = delta > 0 ? 0.1 : -0.1;

                double oldScale = scaleTransform.ScaleX;
                double newScale = oldScale + zoom;

                if (newScale < 0.4)
                    return newScale;

                double contentX = (this.HorizontalOffset + centerPoint.X) / oldScale;
                double contentY = (this.VerticalOffset + centerPoint.Y) / oldScale;

                scaleTransform.ScaleX = newScale;
                scaleTransform.ScaleY = newScale;

                this.ScrollToHorizontalOffset(contentX * newScale - centerPoint.X);

                this.ScrollToVerticalOffset(contentY * newScale - centerPoint.Y);

                return newScale;
            }

            return 1.0;
        }

        private void ScrollViewerBase_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.CanZoom)
            {
                e.Handled = true;

                Point mousePosition = e.GetPosition(this);
                this.ZoomTo(mousePosition, e.Delta);

                return;
            }

            if (this.stopScroll)
                return;

            this.ScrollToVerticalOffset(this.VerticalOffset - e.Delta / 3);
        }

        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            if (!this.IsPannable)
            {
                return;
            }

            if (this.IsMouseOver)
            {
                this.scrollStartPoint = e.GetPosition(this);

                this.scrollStartOffset.X = base.HorizontalOffset;

                this.scrollStartOffset.Y = base.VerticalOffset;

                this.Cursor = (this.ExtentWidth > this.ViewportWidth) || (this.ExtentHeight > this.ViewportHeight) ? Cursors.ScrollAll : Cursors.Arrow;

                this.isMouseCaptured = true;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.isMouseCaptured && this.IsPannable)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    this.Cursor = Cursors.Arrow;

                    this.isMouseCaptured = false;

                    return;
                }

                Point point = e.GetPosition(this);

                Point delta = new Point((point.X > this.scrollStartPoint.X) ? -(point.X - this.scrollStartPoint.X) : (this.scrollStartPoint.X - point.X), (point.Y > this.scrollStartPoint.Y) ? -(point.Y - this.scrollStartPoint.Y) : (this.scrollStartPoint.Y - point.Y));

                ////Scroll to new posistion
                this.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);

                this.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            this.isMouseCaptured = false;
        }

        protected override void OnPreviewMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (!this.IsPannable)
            {
                return;
            }

            if (this.isMouseCaptured)
            {
                this.Cursor = Cursors.Arrow;

                this.isMouseCaptured = false;
            }
        }

        private void Initialize()
        {
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.PreviewMouseWheel += this.ScrollViewerBase_PreviewMouseWheel;
        }
    }
}
