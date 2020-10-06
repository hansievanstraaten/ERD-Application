using System.ComponentModel;

namespace ViSo.SharedEnums.ReportEnums
{
    public enum SectionTypeEnum
    {
        [Description("None")]
        None = 0,

        [Description("Header")]
        Header = 1,

        [Description("Footer")]
        Footer = 2,

        [Description("Page")]
        Page = 3,

        [Description("Header")]
        TableHeader = 4,

        [Description("Data")]
        TableData = 5,

        [Description("Footer")]
        TableFooter = 6
    }
}
