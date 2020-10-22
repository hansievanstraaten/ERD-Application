using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportWhereHeaderMapping : EntityTypeConfiguration<ReportWhereHeader>
	{
		public ReportWhereHeaderMapping()
		{
			ToTable("ReportWhereHeader");

			HasKey(k => new {  k.ReplaceColumn , k.ReplaceTable , k.MasterReport_Id   });

			Property(colReplaceColumn =>  colReplaceColumn.ReplaceColumn).HasColumnName("ReplaceColumn");
			Property(colReplaceTable =>  colReplaceTable.ReplaceTable).HasColumnName("ReplaceTable");
			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colUseColumn =>  colUseColumn.UseColumn).HasColumnName("UseColumn");
			Property(colUseTable =>  colUseTable.UseTable).HasColumnName("UseTable");
			Property(colIsActive =>  colIsActive.IsActive).HasColumnName("IsActive");

		}
	}
}