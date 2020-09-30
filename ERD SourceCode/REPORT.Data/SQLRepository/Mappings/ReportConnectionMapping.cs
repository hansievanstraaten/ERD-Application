using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportConnectionMapping : EntityTypeConfiguration<ReportConnection>
	{
		public ReportConnectionMapping()
		{
			ToTable("ReportConnection");

			HasKey(k => new {  k.MasterReport_Id , k.ReportConnectionName   });

			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colReportConnectionName =>  colReportConnectionName.ReportConnectionName).HasColumnName("ReportConnectionName");
			Property(colDatabaseTypeEnum =>  colDatabaseTypeEnum.DatabaseTypeEnum).HasColumnName("DatabaseTypeEnum");
			Property(colServerName =>  colServerName.ServerName).HasColumnName("ServerName");
			Property(colDatabaseName =>  colDatabaseName.DatabaseName).HasColumnName("DatabaseName");
			Property(colUserName =>  colUserName.UserName).HasColumnName("UserName");
			Property(colPassword =>  colPassword.Password).HasColumnName("Password");
			Property(colTrustedConnection =>  colTrustedConnection.TrustedConnection).HasColumnName("TrustedConnection");
			Property(colIsProductionConnection =>  colIsProductionConnection.IsProductionConnection).HasColumnName("IsProductionConnection");
			Property(colIsActive =>  colIsActive.IsActive).HasColumnName("IsActive");

		}
	}
}