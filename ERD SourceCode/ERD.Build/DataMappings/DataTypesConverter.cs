using ERD.Models;

namespace ERD.Build.DataMappings
{
  internal static class DataTypesConverter
  {
    internal static string GetStringDataType(ColumnObjectModel column, LanguageOptionEnum languageOption)
    {
      switch (languageOption)
      {
        case LanguageOptionEnum.CSharp:
        default:

          if (column.IsNullable())
          {
            return $"{MsSqlDataMap.GetMsSqlDataMap(column.SqlDataType.Value)}?";
          }

          return MsSqlDataMap.GetMsSqlDataMap(column.SqlDataType.Value);
      }
    }
  }
}
