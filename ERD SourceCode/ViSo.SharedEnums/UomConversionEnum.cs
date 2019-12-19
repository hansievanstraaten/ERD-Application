using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViSo.SharedEnums
{
  public enum UomConversionEnum
  {
    [Description("<None>")]
    None = 0,

    [Description("Multiply")]
    Multiply = 1,

    [Description("Divide")]
    Divide = 2
  }
}
