using System.ComponentModel;

namespace REPORT.Builder.ReportEnums
{
    public enum SectionTypeEnum
    {
        [Description("Header")]
        Header = 1,

        [Description("Content")]
        Content = 2,

        [Description("Footer")]
        Footer = 3
    }
}
