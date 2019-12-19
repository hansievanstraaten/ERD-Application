using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class ArrayExstentions
  {
    public static bool HasElements<T>(this T[] value)
    {
      if (value == null || value.Length == 0)
      {
        return false;
      }

      return true;
    }
    
    public static object[] CloneArrayTo<T>(this T[] source, Type resultT)
    {
      if (source == null || source.Length == 0)
      {
        return new object[] { };
      }

      Queue<object> result = new Queue<object>();

      Type sourceT = source[0].GetType();
      
      foreach (T sourceItem in source)
      {
        ConstructorInfo constructorInfo = resultT.GetConstructor(Type.EmptyTypes);
       
        object instanceResult = constructorInfo.Invoke(null);
        
        foreach (PropertyInfo item in sourceT.GetProperties())
        {
          try
          {
            PropertyInfo resultP = sourceT.GetProperty(item.Name);

            if (resultP == null)
            {
              continue;
            }

            resultP.SetValue(instanceResult, item.GetValue(sourceItem, null), null);
          }
          catch
          {
            // Do Nothing
          }
        }

        result.Enqueue(instanceResult);
      }

      return result.ToArray();
    }
    
    public static T[] CloneArray<T>(this T[] source)
    {
      if (source == null || source.Length == 0)
      {
        return new T[] { };
      }

      Queue<T> result = new Queue<T>();

      Type sourceT = source[0].GetType();

      Type resultT = source[0].GetType();

      foreach (T sourceItem in source)
      {
        ConstructorInfo constructorInfo = resultT.GetConstructor(Type.EmptyTypes);
       
        T instanceResult = (T)constructorInfo.Invoke(null);
        
        foreach (PropertyInfo item in sourceT.GetProperties())
        {
          try
          {
            PropertyInfo resultP = sourceT.GetProperty(item.Name);

            if (resultP == null)
            {
              continue;
            }

            resultP.SetValue(instanceResult, item.GetValue(sourceItem, null), null);
          }
          catch
          {
            // Do Nothing
          }
        }

        result.Enqueue((T)instanceResult);
      }

      return result.ToArray();
    }
    
    public static T[] Add<T>(this T[] array, T item)
    {
      if (!array.HasElements())
      {
        array = new T[] { };
      }

      Array.Resize(ref array, array.Length + 1);

      array[array.Length - 1] = item;
      
      return array;
    }

    public static T[] AddRange<T>(this T[] array, T[] items)
    {
      foreach (T item in items)
      {
        array = array.Add(item);
      }

      return array;
    }

    public static T[] TryCast<T>(this object[] items)
    {
      if (items == null || items.Length == 0)
      {
        return new T[] { };
      }

      try
      {
        return items.Cast<T>().ToArray();
      }
      catch
      {
        return new T[] { };
      }
    }

    public static T[] Remove<T>(this T[] array, T item)
    {
      List<T> result = new List<T>(array);

      result.Remove(item);

      return result.ToArray();
    }
    
    public static string Concatenate(this string[] array, string delimiter)
    {
      StringBuilder result = new StringBuilder();

      foreach (string item in array)
      {
        result.Append($"{item}{delimiter}");
      }

      return result.ToString();
    }

    public static void SortLogically<T>(this T[] array, string propertyName, bool preferenceToAlpha = false)
    {
      Type objectType = typeof(T);

      PropertyInfo info = objectType.GetProperty(propertyName);

      string[] propValues = array.Select(p => info.GetValue(p)).TryCast<string>().ToArray();

      propValues.SortLogically(preferenceToAlpha);

      Array.Sort(array, (a, b) =>
      {
        object aValue = a.GetPropertyValue(propertyName);

        object bValue = b.GetPropertyValue(propertyName);

        int aIndex = Array.IndexOf(propValues, aValue.ParseToString());

        int bIndex = Array.IndexOf(propValues, bValue.ParseToString());

        if (aIndex == bIndex)
        {
          return 0;
        }

        return aIndex > bIndex ? 1 : -1;
      });
    }

    public static void SortLogically(this string[] array, bool preferenceToAlpha = false)
    {
      char[] sortByArray = null;

      if (preferenceToAlpha)
      {
        sortByArray = new char[]
        {
          'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g', 'H', 'h',
          'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o', 'P', 'p', 'Q', 'q', 'R', 'r', 'S', 's',
          'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y', 'Z', 'z',
          '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
      }
      else
      {
        sortByArray = new char[]
        {
          '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
          'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g', 'H', 'h',
          'I', 'i', 'J', 'j', 'K', 'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o', 'P', 'p', 'Q', 'q', 'R', 'r', 'S', 's',
          'T', 't', 'U', 'u', 'V', 'v', 'W', 'w', 'X', 'x', 'Y', 'y', 'Z', 'z'
        };
      }

      Array.Sort(array, (a, b) =>
      {
        #region PRE-VALIDATION

        if (a.IsNullEmptyOrWhiteSpace() && b.IsNullEmptyOrWhiteSpace())
        {
          return 0; // They are the same
        }

        if (a.IsNullEmptyOrWhiteSpace())
        {
          return -1; // Set A on top
        }

        if (b.IsNullEmptyOrWhiteSpace())
        {
          return 1; // Set B on Top
        }

        #endregion

        #region WORKING CHAR ARRAY SETUP

        int maxCharLength = a.Length > b.Length ? a.Length : b.Length; // Get the max char length to ease the checks and balances down stream

        char[] aRawChars = a.ToCharArray();

        char[] bRawChars = b.ToCharArray();

        char[] aWorkingChars = new char[maxCharLength]; // a.ToCharArray();

        char[] bWorkingChars = new char[maxCharLength]; // b.ToCharArray();

        Array.Copy(aRawChars, aWorkingChars, aRawChars.Length);

        Array.Copy(bRawChars, bWorkingChars, bRawChars.Length);

        // Fill the empty chars
        if (a.Length > b.Length)
        {
          for (int x = b.Length; x < bWorkingChars.Length; ++x)
          {
            bWorkingChars[x] = ' ';
          }
        }
        else
        {
          for (int x = a.Length; x < aWorkingChars.Length; ++x)
          {
            aWorkingChars[x] = ' ';
          }
        }

        #endregion
        
        int readIndex = 0;

        StringBuilder aResult = new StringBuilder();

        StringBuilder bResult = new StringBuilder();

        while (readIndex < maxCharLength)
        {
          #region CALCULATION

          int aIndex = Array.IndexOf(sortByArray, aWorkingChars[readIndex]);

          int bIndex = Array.IndexOf(sortByArray, bWorkingChars[readIndex]);

          ++readIndex;

          if (aIndex >= 0)
          {
            aResult.Append(aIndex);
          }

          if (bIndex >= 0)
          {
            bResult.Append(bIndex);
          }

          #endregion
        }

        if (aResult.ParseToString() == bResult.ParseToString())
        {
          return 0;
        }

        int result = aResult.ParseToString().ToDouble() > bResult.ParseToString().ToDouble() ? 1 : -1; // For debugging

        return result;
      });
    }
  }
}
