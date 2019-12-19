using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WPF.Tools.ToolModels;

namespace WPF.Tools.Functions
{
  public static class ViSoGlobalization
  {
    public static DataItemModel[] MonthsOfYearLookup
    {
      get
      {
        List<string> monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames.Take(12).ToList();

        List<DataItemModel> result = monthNames
          .Select(m => new DataItemModel { ItemKey = monthNames.IndexOf(m) + 1, DisplayValue = m })
          .ToList();

        return result.ToArray();
      }
    }
  }
}
