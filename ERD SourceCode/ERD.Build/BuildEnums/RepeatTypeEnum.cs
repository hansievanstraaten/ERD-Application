using System.ComponentModel;

namespace ERD.Build.BuildEnums
{
    public enum RepeatTypeEnum
    {
        [Description("Once")] Once,

        [Description("Foreach Table in Canvas")]
        ForeachTableInCanvas,

        [Description("Foreach Column in Table")]
        ForeachColumnInTable,

        [Description("Foreach Primary Key in Table")]
        ForeachPrimaryKeyInTable,

        [Description("Foreach Foreign Key in Table")]
        ForeachForeignKeyInTable,

        [Description("Foreach Non-Key Column in Table")]
        ForeachNonColumnInTable,

        [Description("Foreach Referenced Table")]
        ForeachReferencedTable

  }
}
