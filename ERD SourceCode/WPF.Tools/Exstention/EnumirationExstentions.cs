using System;
using System.Collections.Generic;
using WPF.Tools.ToolModels;
using GeneralExtensions;

namespace WPF.Tools.Exstention
{
  public class Enum<T> where T : struct, IConvertible
  {
    public static List<DataItemModel> ToDataItemsEnumKey()
    {
      List<DataItemModel> result = new List<DataItemModel>();

      foreach (var item in Enum.GetValues(typeof(T)))
      {
        result.Add(new DataItemModel
        {
          DisplayValue = item.GetDescriptionAttribute(),
          ItemKey = item
        });
      }

      return result;
    }

    public static List<DataItemModel> ToDataItemsIntKey()
    {
      List<DataItemModel> result = new List<DataItemModel>();

      foreach (var item in Enum.GetValues(typeof(T)))
      {
        result.Add(new DataItemModel
        {
          DisplayValue = item.GetDescriptionAttribute(),
          ItemKey = item.ToInt32()
        });
      }

      return result;
    }

    public static int Count
    {
      get
      {
        if (!typeof(T).IsEnum)
          throw new ArgumentException("T must be an enumerated type");

        return Enum.GetNames(typeof(T)).Length;
      }
    }
  }
}
