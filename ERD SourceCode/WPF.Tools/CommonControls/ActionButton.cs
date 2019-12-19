using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GeneralExtensions;

namespace WPF.Tools.CommonControls
{
  public class ActionButton : Button
  {
    private string resourceImageName;

    private double defaultImageSize = 26;

    private Image buttonImage;
    
    public ActionButton()
    {
      object itemStyle = this.FindResource("ButtonStyle"); 

      if (itemStyle != null && itemStyle.GetType() == typeof(Style))
      {
        base.Style = (Style)itemStyle;
      }
      
      this.BorderBrush = Brushes.Transparent;

      this.Loaded += this.ActionButton_Loaded;
    }

    public double DefaultSize
    {
      get
      {
        return this.defaultImageSize;
      }

      set
      {
        this.defaultImageSize = value;

        this.Width = value;

        this.Height = value;
      }
    }

    public string ResourceImageName
    {
      get
      {
        return this.resourceImageName;
      }

      set
      {
        this.resourceImageName = value;

        if (this.IsLoaded && !value.IsNullEmptyOrWhiteSpace())
        {
          this.SetImageSource(this.DefaultSize);
        }
      }
    }
    
    public ImageSource ImageSource
    {
      get
      {
        if (this.buttonImage == null)
        {
          return null;
        }

        return this.buttonImage.Source;
      }

      set
      {
        if (this.buttonImage == null)
        {
          this.buttonImage = new Image { Margin = new Thickness(0), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

          base.HorizontalContentAlignment = HorizontalAlignment.Stretch;

          base.VerticalContentAlignment = VerticalAlignment.Stretch;

          base.Content = this.buttonImage;
        }

        this.buttonImage.Source = value;
      }
    }
    
    private void ActionButton_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (!this.ResourceImageName.IsNullEmptyOrWhiteSpace())
        {
          this.SetImageSource(this.DefaultSize);
        }
      }
      catch
      {
        // DO NOTHING
      }
    }

    private void SetImageSource(double defaultSize)
    {
      this.DefaultSize = defaultSize.IsNan() || defaultSize < 16 ? 16 : defaultSize;
      
      double desiredSize = this.DesiredSize.Height.IsNan() || this.DesiredSize.Height < 16 ? this.DefaultSize : this.DesiredSize.Height;

      this.ImageSource = IconSet.IconSets.ResourceImageSource(this.ResourceImageName, desiredSize);
    }

  }
}
