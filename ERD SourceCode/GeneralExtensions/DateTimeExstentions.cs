using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class DateTimeExstentions
  {
    public static string ToStringCulture(this DateTime value, bool showTime)
    {
      CultureInfo inf = CultureInfo.CurrentCulture;

      string format = "dd/MM/yyyy";

      if (showTime)
      {
        if (inf.DateTimeFormat != null && inf.DateTimeFormat.ShortDatePattern != null)
        {
          format = string.Format("{0} {1}", inf.DateTimeFormat.ShortDatePattern, inf.DateTimeFormat.ShortTimePattern);
        }
        else
        {
          format = string.Format("{0} {1}", format, "h:mm tt");
        }

        format = format.Replace('h', 'H');

        return value.ToString(format, CultureInfo.CurrentCulture);
      }

      if (inf.DateTimeFormat != null && inf.DateTimeFormat.ShortDatePattern != null)
      {
        format = inf.DateTimeFormat.ShortDatePattern;
      }

      return value.ToString(format, CultureInfo.CurrentCulture);
    }

    public static TimeSpan DateDifference(this DateTime start, DateTime endDate)
    {
      TimeSpan result = endDate - start;

      return result;
    }

    public static DateTime DateAt00Time(this DateTime date)
    {
      return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }

    public static DateTime DateAt24Time(this DateTime date)
    {
      return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
    }
  }
}
