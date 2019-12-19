using System.ComponentModel;

namespace ERD.DatabaseScripts.Compare
{
  public enum ObjectTypeEnum
  {
    [Description("Table")]
    Table,

    [Description("Column")]
    Column
  }
}
