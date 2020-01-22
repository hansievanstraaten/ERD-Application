using System.ComponentModel;

namespace ERD.Build.BuildEnums
{
  public enum RepeatOptionEnum
  {
    [Description("Foreach Canvas in Project")]
    ForeachCanvas,

    [Description("Foreach Table in Project")]
    ForeachTableProject,

    [Description("Single File for Project")]
    SingleFile
  }
}
