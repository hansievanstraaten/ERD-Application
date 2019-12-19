using System;
using System.Text;
using GeneralExtensions;

namespace ViSo.Common
{
  public static class ViSoCollections
  {
    public static char[] Alpha
    {
      get
      {
        return new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', ' ' };
      }
    }

    public static int GetAlphaAsNumber(char character)
    {
      int result = Array.IndexOf(ViSoCollections.Alpha, character);

      return ++result;
    }

    public static string GetNextAlpha(string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return ViSoCollections.Alpha[0].ToString();
      }

      int currentIndex = 0;

      for (int x = 0; x < value.Length; ++x)
      {
        currentIndex *= 26;

        currentIndex += (value[x] - 'A' + 1);
      }
      
      return ViSoCollections.GetNextExcelColumn(++currentIndex);
    }

    private static string GetNextExcelColumn(int annextureIndex)
    {
      if (annextureIndex <= 26)
      {
        return Convert.ToChar(annextureIndex + 64).ToString();
      }

      int div = annextureIndex / 26;

      int mod = annextureIndex % 26;

      if (mod == 0)
      {
        mod = 26; div--;
      }

      return GetExcelColumnName(div) + GetExcelColumnName(mod);
    }

    private static string GetExcelColumnName(int columnNumber)
    {
      if (columnNumber <= 26)
      {
        return Convert.ToChar(columnNumber + 64).ToString();
      }

      int div = columnNumber / 26;

      int mod = columnNumber % 26;

      if (mod == 0)
      {
        mod = 26; div--;
      }

      return GetExcelColumnName(div) + GetExcelColumnName(mod);
    }
  }
}
