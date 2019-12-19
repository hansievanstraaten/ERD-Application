using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.Tools.BaseClasses
{
  public abstract class GridSplitterBase : GridSplitter
  {
    public GridSplitterBase()
    {
      if (DesignerProperties.GetIsInDesignMode(this))
      {
        return;
      }

      this.SetStyes();

      this.IsTabStop = false;

      this.UseLayoutRounding = true;

      this.VisualBitmapScalingMode = System.Windows.Media.BitmapScalingMode.NearestNeighbor;

      this.SnapsToDevicePixels = true;

      this.VisualClearTypeHint = System.Windows.Media.ClearTypeHint.Enabled;
    }

    private void SetStyes()
    {
      this.Background = (LinearGradientBrush)this.FindResource("GridSplitter");
    }
  }

}
