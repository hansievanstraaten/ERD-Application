using IconSet;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.Tools.Converters
{
  public class ColorConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        string color = System.Convert.ToString(values[0]);

        return ColourConverters.GetBrushfromHex(color);
      }
      catch
      {
        return System.Windows.DependencyProperty.UnsetValue;
      }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      return new object[] { };
    }
  }
}
