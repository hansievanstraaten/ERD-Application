using System.ComponentModel;

namespace REPORT.Builder.ReportEnums
{
  public enum ReportTypeEnum
  {
    [Description("Cover Page")]
    CoverPage = 1,

    [Description("Page Header and Footer")]
    PageHeaderAndFooter = 2,

    [Description("Report Content")]
    ReportContent = 3,

    [Description("Final Page")]
    FinalPage = 4
  }
}
