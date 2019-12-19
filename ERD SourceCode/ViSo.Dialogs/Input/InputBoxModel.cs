using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.Attributes;

namespace ViSo.Dialogs.Input
{
  [ModelNameAttribute("Input Box")]
  internal class InputBoxModel
  {
    [FieldInformationAttribute("Value")]
    public string Value { get; set;}
  }
}
