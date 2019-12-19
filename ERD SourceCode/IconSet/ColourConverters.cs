using System.Windows.Media;

namespace IconSet
{
  public static class ColourConverters
  {
    public static Color GetFromHex(string hex)
    {
      return (Color)ColorConverter.ConvertFromString(hex);
    }

    public static Brush GetBrushfromHex(string hex)
    {
      return (SolidColorBrush)(new BrushConverter().ConvertFrom(hex));
    }

    public static string GetHexFromBrush(Brush brush)
    {
      return ((SolidColorBrush)brush).Color.ToString();
    }
  }
}
