using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportMasterMapping : EntityTypeConfiguration<ReportMaster>
	{
		public ReportMasterMapping()
		{
			ToTable("ReportMaster");

			HasKey(k => new {  k.MasterReport_Id   });

			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colReportName =>  colReportName.ReportName).HasColumnName("ReportName");
			Property(colDescription =>  colDescription.Description).HasColumnName("Description");
			Property(colReportTypeEnum =>  colReportTypeEnum.ReportTypeEnum).HasColumnName("ReportTypeEnum");
			Property(colPaperKindEnum =>  colPaperKindEnum.PaperKindEnum).HasColumnName("PaperKindEnum");
			Property(colPageOrientationEnum =>  colPageOrientationEnum.PageOrientationEnum).HasColumnName("PageOrientationEnum");
			Property(colCoverPage_Id =>  colCoverPage_Id.CoverPage_Id).HasColumnName("CoverPage_Id");
			Property(colHeaderAndFooterPage_Id =>  colHeaderAndFooterPage_Id.HeaderAndFooterPage_Id).HasColumnName("HeaderAndFooterPage_Id");
			Property(colFinalPage_Id =>  colFinalPage_Id.FinalPage_Id).HasColumnName("FinalPage_Id");

		}
	}
}