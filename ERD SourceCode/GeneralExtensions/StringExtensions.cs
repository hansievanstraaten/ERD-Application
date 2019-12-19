using System;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace GeneralExtensions
{
  public static class StringExtensions
  {
    public static bool IsNullEmptyOrWhiteSpace(this string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return true;
      }

      if (string.IsNullOrWhiteSpace(value))
      {
        return true;
      }

      return false;
    }

    public static bool IsNullEmptyOrWhiteSpace(this StringBuilder value)
    {
      if (string.IsNullOrEmpty(value.ToString()))
      {
        return true;
      }

      if (string.IsNullOrWhiteSpace(value.ToString()))
      {
        return true;
      }

      return false;
    }

    public static bool ToBool(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return false;
      }

      return Convert.ToBoolean(value);
    }

    public static int ToInt32(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return 0;
      }

      return Convert.ToInt32(value);
    }
    
    public static long ToInt64(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return 0;
      }

      return Convert.ToInt64(value);
    }
    
    public static int TryToInt32(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return 0;
      }

      try
      {
        return Convert.ToInt32(value);
      }
      catch
      {
        return 0;
      }
    }

    public static long TryToInt64(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return 0;
      }

      try
      {
        return Convert.ToInt64(value);
      }
      catch
      {
        return 0;
      }
    }
    
    public static string MakeAlphaNumeric(this string value)
    {
      Regex rgx = new Regex("[^a-zA-Z0-9-_]");

      return rgx.Replace(value, string.Empty);
    }

    public static bool IsNumberic(this string value)
    {
      decimal resultValue = 0;

      return decimal.TryParse(value, out resultValue);
    }

    public static string[] Split(this string value, char character)
    {
      return value.Split(new char[] {character});
    }

    public static string[] Split(this string value, char character, StringSplitOptions options)
    {
      return value.Split(new char[] { character }, options);
    }
    
    public static string SplitByCammelCase(this string e)
    {
      if (e.IsNullEmptyOrWhiteSpace())
      {
        return string.Empty;
      }

      return new string(EnumberableExtensions.InsertSpace(e).ToArray());
    }

    public static string CharacterAt(this string value, int index, bool toUpper = false)
    {
      if (value.IsNullEmptyOrWhiteSpace() || value.Length < index)
      {
        return string.Empty;
      }

      char result = value[index];

      if (!toUpper)
      {
        return result.ToString();
      }

      return result.ToString().ToUpper();
    }

    public static string FirstToUpper(this string e)
    {
      e = e.ToLower();

      if (string.IsNullOrEmpty(e))
      {
        return string.Empty;
      }

      char[] textArray = e.ToCharArray();

      textArray[0] = char.ToUpper(textArray[0]);

      return new string(textArray);
    }

    public static string TrimToLength(this string e, int length)
    {
      if (e == null)
      {
        return string.Empty;
      }

      if (e.Length < length)
      {
        return e;
      }

      return e.Substring(0, length);
    }

    public static string TrimToLength(this string e, int length, int trailLength, char trailChar)
    {
      string result = e.TrimToLength(length);

      if (result.Length < length)
      {
        return result;
      }

      int trimLength = length - trailLength;

      result = result.TrimToLength(trimLength);

      while (result.Length < length)
      {
        result += trailChar;
      }

      return result;
    }

    public static SecureString SecureString(this string value, bool asReadOnly = false)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return null;
      }

      if (asReadOnly)
      {
        return value.Aggregate(new SecureString(), AppendChar, MakeReadOnly);  
      }
      
      return value.Aggregate(new SecureString(), AppendChar);
    }

    private static SecureString MakeReadOnly(SecureString resultString)
    {
      resultString.MakeReadOnly();

      return resultString;
    }

    private static SecureString AppendChar(SecureString resultString, char appenChar)
    {
      resultString.AppendChar(appenChar);

      return resultString;
    }
    
    public static byte[] ConvertStringToBytes(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return new byte[] { };
      }

      return Convert.FromBase64String(value);
    }
  }
}
