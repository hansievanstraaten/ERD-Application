using System;
using System.ComponentModel;
using System.Reflection;

namespace GeneralExtensions
{
  public static class EnumExstentions
  {
    public static string GetDescriptionAttribute<T>(this T value)
    {
      Type type = value.GetType();

      string name = Enum.GetName(type, value);

      if (name == null)
      {
        return value.ParseToString().SplitByCammelCase();
      }

      FieldInfo field = type.GetField(name);

      if (field == null)
      {
        return value.ParseToString().SplitByCammelCase();
      }

      DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

      return attr != null ? attr.Description : value.ParseToString().SplitByCammelCase();
    }
  }
}
