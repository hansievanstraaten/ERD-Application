using ERD.Models.ReportModels.StructureModels;
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

		}
	}
}