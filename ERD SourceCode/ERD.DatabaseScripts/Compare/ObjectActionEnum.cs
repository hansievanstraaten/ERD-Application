using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.DatabaseScripts.Compare
{
  public enum ObjectActionEnum
  {
    [Description("Drop from Database")]
    DropFromDatabase,

    [Description("Drop from ERD Model")]
    DropFromERD,

    [Description("Create in Database")]
    CreateInDatabase,

    [Description("Correct on Model")]
    CorrectOnModel,

    [Description("Correct in Database")]
    CorrectInDatabase,

    [Description("Ignore")]
    Ignore
  }
}
