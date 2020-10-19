using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportXMLPrintParameterMapping : EntityTypeConfiguration<ReportXMLPrintParameter>
	{
		public ReportXMLPrintParameterMapping()
		{
			ToTable("ReportXMLPrintParameter");

			HasKey(k => new {  k.TableName , k.ColumnName , k.ReportXMLVersion , k.MasterReport_Id   });

			Property(colTableName =>  colTableName.TableName).HasColumnName("TableName");
			Property(colColumnName =>  colColumnName.ColumnName).HasColumnName("ColumnName");
			Property(colReportXMLVersion =>  colReportXMLVersion.ReportXMLVersion).HasColumnName("ReportXMLVersion");
			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colIsActive =>  colIsActive.IsActive).HasColumnName("IsActive");
			Property(colFilterCaption =>  colFilterCaption.FilterCaption).HasColumnName("FilterCaption");
			Property(colDefaultValue =>  colDefaultValue.DefaultValue).HasColumnName("DefaultValue");

		}
	}
}