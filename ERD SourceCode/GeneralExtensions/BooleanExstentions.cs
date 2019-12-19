using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class BooleanExstentions
  {
    public static bool IsTrue(this bool? value)
    {
      if (value.HasValue && value.Value)
      {
        return true;
      }

      return false;
    }

    public static bool IsFalse(this bool? value)
    {
      if (value.HasValue && value.Value)
      {
        return false;
      }

      return true;
    }
  }
}
