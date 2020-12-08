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

        [Description("Foreach Primary Key in Table (Include Foreign Keys)")]
        ForeachPrimaryKeyInTable,

        [Description("Foreach Primary Key in Table (Exclude Foreign Keys)")]
        ForeachPrimaryOnlyKeyInTable,

        [Description("Foreach Foreign Key in Table (Exclude Primary Keys)")]
        ForeachForeignKeyInTable,

		[Description("Foreach Foreign Key in Table (Include Primary Keys)")]
		ForeachPrimaryForeignKeyInTable,

		[Description("Foreach Non-Key Column in Table")]
        ForeachNonColumnInTable,

        [Description("Foreach Referenced Table")]
        ForeachReferencedTable,

        [Description("Foreach Table in Project")]
        ForeachTableInProject

    }
}
