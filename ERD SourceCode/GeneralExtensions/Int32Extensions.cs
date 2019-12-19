using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class Int32Extensions
  {
    public static decimal ToDecimal(this int value)
    {
      return Convert.ToDecimal(value);
    }
  }
}
