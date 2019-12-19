using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class DecimalExtensions
  {
    private static string DefaultCurrencyDecimalFormat = "{0:C}";

    public static decimal? TryToDecimal(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return null;
      }

      bool haveSeperator = false;

      StringBuilder resultString = new StringBuilder();

      char[] valueCahracters = value.ToCharArray();

      for (int x = (valueCahracters.Length - 1); x >= 0; x--)
      {
        if (char.IsNumber(valueCahracters[x]))
        {
          resultString.Insert(0, valueCahracters[x]);
        }
        else if (!haveSeperator && resultString.Length > 0)
        {
          haveSeperator = true;

          resultString.Insert(0, '.');
        }
      }

      if (resultString.IsNullEmptyOrWhiteSpace())
      {
        return null;
      }

      return Convert.ToDecimal(resultString.ToString(), CultureInfo.InvariantCulture);
    }

    public static decimal ToDecimal(this float value)
    {
      return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
    }

    public static float ToFloat(this decimal value)
    {
      return Convert.ToSingle(value);
    }

    public static double ToDouble(this float value)
    {
      return Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    public static int ToInt32(this decimal value)
    {
      return Convert.ToInt32(value);
    }

    public static decimal? TryToDecimal(this string value, CultureInfo culture)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return null;
      }

      bool haveSeperator = false;

      string cultureDecimalSeperator = culture.NumberFormat.NumberDecimalSeparator;

      StringBuilder resultString = new StringBuilder();

      char[] valueCahracters = value.ToCharArray();

      for (int x = (valueCahracters.Length - 1); x >= 0; x--)
      {
        if (char.IsNumber(valueCahracters[x]))
        {
          resultString.Insert(0, valueCahracters[x]);
        }
        else if (!haveSeperator && resultString.Length > 0)
        {
          haveSeperator = true;

          resultString.Insert(0, cultureDecimalSeperator);
        }
      }

      if (resultString.IsNullEmptyOrWhiteSpace())
      {
        return null;
      }

      return Convert.ToDecimal(resultString.ToString(), culture);
    }

    public static string ToStringCulture(this decimal? value, bool asCurrentcy = true)
    {
      if (!value.HasValue)
      {
        return string.Empty;
      }

      if (asCurrentcy)
      {
        return string.Format(CultureInfo.CurrentCulture, DefaultCurrencyDecimalFormat, value);
      }

      return Convert.ToString(value, CultureInfo.CurrentCulture);
    }

    public static string ToStringCulture(this decimal? value, CultureInfo culture, bool asCurrentcy = true)
    {
      if (!value.HasValue)
      {
        return string.Empty;
      }

      if (asCurrentcy)
      {
        return string.Format(culture, DefaultCurrencyDecimalFormat, value);
      }

      return Convert.ToString(value, culture);
    }

    public static string ToStringCulture(this decimal value, bool asCurrentcy = true)
    {
      if (asCurrentcy)
      {
        return string.Format(CultureInfo.CurrentCulture, DefaultCurrencyDecimalFormat, value);
      }

      return Convert.ToString(value, CultureInfo.CurrentCulture);
    }

    public static string ToStringCulture(this decimal value, CultureInfo culture, bool asCurrentcy = true)
    {
      if (asCurrentcy)
      {
        return string.Format(culture, DefaultCurrencyDecimalFormat, value);
      }

      return Convert.ToString(value, culture);
    }

  }
}
