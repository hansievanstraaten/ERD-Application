using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportHeaderFooters.xaml
    /// </summary>
    public partial class ReportHeaderFooters : UserControlBase
    {
        private ReportTypeEnum selectedreportType;

        public ReportHeaderFooters(ReportTypeEnum reportType)
        {
            this.InitializeComponent();

            this.selectedreportType = reportType;

            ReportTablesRepository repo = new ReportTablesRepository();

            this.HeadersAndFooters = repo.GetReportMasterByPrimaryKey
        }

        public ReportMasterModel HeadersAndFooters
        {
            get;

            set;
        }
    }
}
