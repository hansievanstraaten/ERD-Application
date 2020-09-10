using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner : UserControlBase
  {
    private ReportTypeEnum reportDesignType;

    public ReportDesigner(ReportTypeEnum designType)
    {
      this.InitializeComponent();

      this.reportDesignType = designType;
    }
  }
}
