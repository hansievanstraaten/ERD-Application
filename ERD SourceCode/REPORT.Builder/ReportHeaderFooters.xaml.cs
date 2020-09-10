using REPORT.Data.SQLRepository.Agrigates;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportHeaderFooters.xaml
    /// </summary>
    public partial class ReportHeaderFooters : UserControlBase
    {
        public ReportHeaderFooters()
        {
            this.InitializeComponent();
        }

        public ReportMasterModel HeadersAndFooters
        {
            get;

            set;
        }
    }
}
