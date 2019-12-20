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
    
    [Description("Create in Database")]
    CreateInDatabase,

    [Description("Alter Database")]
    AlterDatabase,
    
    [Description("Ignore")]
    Ignore
  }
}
