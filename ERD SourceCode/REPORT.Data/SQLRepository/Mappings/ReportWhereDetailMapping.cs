using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportWhereDetailMapping : EntityTypeConfiguration<ReportWhereDetail>
	{
		public ReportWhereDetailMapping()
		{
			ToTable("ReportWhereDetail");

			HasKey(k => new {  k.WhereOption , k.WhereValue , k.MasterReport_Id , k.ReplaceColumn , k.ReplaceTable   });

			Property(colWhereOption =>  colWhereOption.WhereOption).HasColumnName("WhereOption");
			Property(colWhereValue =>  colWhereValue.WhereValue).HasColumnName("WhereValue");
			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colReplaceColumn =>  colReplaceColumn.ReplaceColumn).HasColumnName("ReplaceColumn");
			Property(colReplaceTable =>  colReplaceTable.ReplaceTable).HasColumnName("ReplaceTable");
			Property(colIsActive =>  colIsActive.IsActive).HasColumnName("IsActive");
			Property(colIsColumn =>  colIsColumn.IsColumn).HasColumnName("IsColumn");

		}
	}
}