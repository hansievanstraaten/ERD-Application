using GeneralExtensions;
using System;
using System.Text;

namespace ViSo.Common
{
  public static class Formatting
  {
    public static string FullnameFormat(string firstName, string secondName, string lastName, string preferedName)
    {
      string secondInitial = secondName.CharacterAt(0, true);

      secondName = secondInitial.IsNullEmptyOrWhiteSpace() ? string.Empty : $"{secondInitial}. ";

      if (preferedName.IsNullEmptyOrWhiteSpace())
      {
        return $"{firstName} {secondName}{lastName}";
      }

      string firstInitial = firstName.CharacterAt(0, true);

      firstName = firstInitial.IsNullEmptyOrWhiteSpace() ? string.Empty : $"{firstInitial}.{(secondInitial.IsNullEmptyOrWhiteSpace() ? " " : string.Empty)}";
      
      return $"{preferedName} {firstName}{secondName}{lastName}";
    }

    public static string PhoneNumberFormats(string phoneNumber)
    {
      if (phoneNumber.IsNullEmptyOrWhiteSpace())
      {
        return string.Empty;
      }

      phoneNumber = phoneNumber.Replace(" ", string.Empty);

      StringBuilder result = new StringBuilder();

      if (phoneNumber.StartsWith("+"))
      {
        for (int x = 0; x < phoneNumber.Length; ++x)
        { // +27 82 225 3357
          // 01234567890
          if (x == 3 || x == 5 || x == 8)
          {
            result.Append(' ');
          }

          result.Append(phoneNumber[x]);
        }
      }
      else
      {
        for (int x = 0; x < phoneNumber.Length; ++x)
        { // 082 225 3357
          // 01234567890
          if (x == 3 || x == 6)
          {
            result.Append(' ');
          }

          result.Append(phoneNumber[x]);
        }
      }


      return result.ToString();
    }

    public static string ForeignKeyID(object[] arguments)
    {
      StringBuilder result = new StringBuilder();

      foreach (object item in arguments)
      {
        result.Append($"{item}|");
      }

      return result.ToString();
    }
  }
}
