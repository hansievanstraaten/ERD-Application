using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

    public void ZoomTo(Point location)
    {
      try
      {
        this.ScrollToHorizontalOffset(location.X - 50);

        this.ScrollToVerticalOffset(location.Y - 50);
      }
      catch
      {
        // DO NOTHING
      }
    }
    
    private void ScrollViewerBase_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (this.CanZoom)
      {
        e.Handled = true;

        FrameworkElement element = this.Content as FrameworkElement;

        if (element != null)
        {
          ScaleTransform scaleTransform = null;

          if (element.LayoutTransform != null && element.LayoutTransform.GetType() == typeof(TransformGroup) && ((TransformGroup) element.LayoutTransform).Children.FirstOrDefault(x => x.GetType() == typeof(ScaleTransform)) != null)
          {
            scaleTransform = ((TransformGroup) element.LayoutTransform).Children.FirstOrDefault(x => x.GetType() == typeof(ScaleTransform)) as ScaleTransform;
          }
          else
          {
            TransformGroup transGroup = new TransformGroup();

            scaleTransform = new ScaleTransform();

            transGroup.Children.Add(scaleTransform);

            element.LayoutTransform = transGroup;
          }

          if (scaleTransform.ScaleX <= 0.4 && e.Delta < 0)
          {
            return;
          }

          double zoom = e.Delta > 0 ? 0.1 : -0.1;

          if (!(e.Delta > 0) && (scaleTransform.ScaleX < 0.4 || scaleTransform.ScaleY < 0.4))
          {
            return;
          }

          scaleTransform.ScaleX += zoom;

          scaleTransform.ScaleY += zoom;
        }

        return;
      }

      if (this.stopScroll)
      {
        return;
      }

      double newOffset = this.VerticalOffset - e.Delta / 3;

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
