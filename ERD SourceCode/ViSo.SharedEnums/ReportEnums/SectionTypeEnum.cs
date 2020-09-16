using System.ComponentModel;

namespace ViSo.SharedEnums.ReportEnums
{
    public enum SectionTypeEnum
    {
        [Description("Header")]
        Header = 1,

        [Description("Content")]
        Content = 2,

        [Description("Footer")]
        Footer = 3,

        [Description("Page")]
        Page = 4
    }
}
