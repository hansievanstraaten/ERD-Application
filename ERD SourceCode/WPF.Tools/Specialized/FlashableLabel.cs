using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IconSet;
using WPF.Tools.CommonControls;

namespace WPF.Tools.Specialized
{
  public class FlashableLabel : LableItem
  {
    public delegate void OnMouseClickedEvent(object sender, System.Windows.Input.MouseButtonEventArgs e);

    public event OnMouseClickedEvent OnMouseClicked;

    private Color endBackground = ColourConverters.GetFromHex("#FFFD5E03");

    private SolidColorBrush animationBrush;

    private Brush originalBrush;

    private ColorAnimation animation;

    private long duration = 2;

    private bool isVertical = false;

    public Color EndColor
    {
      get
      {
        return this.endBackground;
      }

      set
      {
        this.endBackground = value;
      }
    }

    public long DurationSeconds
    {
      get
      {
        return this.duration;
      }

      set
      {
        this.duration = value;
      }
    }

    public bool IsVertical
    {
      get
      {
        return this.isVertical;
      }

      set
      {
        this.PerformRotation(value);

        this.isVertical = value;
      }
    }

    public void StartAnimation()
    {
      if (this.animationBrush != null)
      {
        return;
      }

      this.originalBrush = this.Background;

      Color start = ((SolidColorBrush) this.Background).Color;

      if (this.Background.IsFrozen)
      {
        this.Background = this.Background.CloneCurrentValue();
      }

      this.animationBrush = new SolidColorBrush(start);

      this.Background = this.animationBrush;

      this.animation = new ColorAnimation(start, this.EndColor, new Duration(TimeSpan.FromSeconds(this.duration)));

      this.animation.AutoReverse = true;

      this.animation.RepeatBehavior = RepeatBehavior.Forever;

      this.animationBrush.BeginAnimation(SolidColorBrush.ColorProperty, this.animation);
    }

    public void EndAnimation()
    {
      if (this.originalBrush == null)
      {
        return;
      }

      this.Background = this.originalBrush;

      this.animationBrush = null;

      this.animation = null;

      this.originalBrush = null;
    }

    protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
    {
      this.EndAnimation();

      base.OnMouseLeftButtonUp(e);

      if (this.OnMouseClicked != null)
      {
        this.OnMouseClicked(this, e);
      }
    }

    private void PerformRotation(bool newValue)
    {
      if (this.IsVertical == newValue)
      {
        // Nothing to do
        return;
      }

      if (newValue)
      {
        // Rotate to vertical posistion
        RotateTransform rotateV = new RotateTransform(90);

        this.LayoutTransform = rotateV;

        return;
      }

      // Rotate Horizontal
      RotateTransform rotateH = new RotateTransform(0);

      this.LayoutTransform = rotateH;
    }
  }
}
