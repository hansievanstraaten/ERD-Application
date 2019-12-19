using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeneralExtensions
{
  public static class EnumberableExtensions
  {
    public static object GetByIndex(this IEnumerable enumerable, int i)
    {
      int m = 0;

      foreach (object obj in enumerable)
      {
        if (m == i)
        {
          return obj;
        }

        m++;
      }

      return null;
    }
    
    public static IEnumerable<T> TryCast<T>(this IEnumerable items)
    {
      if (items == null)
      {
        return Enumerable.Empty<T>();
      }

      try
      {
        return items.Cast<T>();
      }
      catch
      {
        return Enumerable.Empty<T>();
      }
    }

    public static IEnumerable<char> InsertSpace(IEnumerable<char> input)
    {
      for (int x = 0; x < input.Count(); x++)
      {
        char item = input.ToArray()[x];

        if (char.IsUpper(item) && x > 0)
        {
          yield return ' ';
        }

        yield return item;
      }
    }
  }
}
