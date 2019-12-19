using IconSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using WPF.Tools.CommonControls;

namespace WPF.Tools.Specialized
{
  public enum DirectionsEnum
  {
    Left = 1,
    Right = 2,
    Down = 3,
    Up = 4
  }

  public class DirectionButton : ActionButton
  {
    private double scale = 12;

    private DirectionsEnum pointDirection;

    public DirectionButton()
      : base()
    {
      this.BorderBrush = Brushes.White;

      this.BorderThickness = new System.Windows.Thickness(0);

      this.Loaded += this.DirectionButton_Loaded;
    }

    public double Scale
    {
      get
      {
        return this.scale;
      }

      set
      {
        this.scale = value;
      }
    }

    public DirectionsEnum Direction 
    {
      get
      {
        return this.pointDirection;
      }

      set
      {
        this.pointDirection = value;

        DirectionButton_Loaded(this, null);
      }
    }

    private void DirectionButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      switch (this.Direction)
      {
        case DirectionsEnum.Left:

          this.Content = new Image() { Source = IconSets.ResourceImageSource("ArrowLeft", this.Scale) };

          break;
        case DirectionsEnum.Right:

          this.Content = new Image() { Source = IconSets.ResourceImageSource("ArrowRight", this.Scale) };

          break;
        case DirectionsEnum.Up:

          this.Content = new Image() { Source = IconSets.ResourceImageSource("ArrowUp", this.Scale) };

          break;
        case DirectionsEnum.Down:

          this.Content = new Image() { Source = IconSets.ResourceImageSource("ArrowDown", this.Scale) };

          break;
        default:
          break;
      }
    }
  }
}
