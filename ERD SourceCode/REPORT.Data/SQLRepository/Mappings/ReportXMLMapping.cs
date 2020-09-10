using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportXMLMapping : EntityTypeConfiguration<ReportXML>
	{
		public ReportXMLMapping()
		{
			ToTable("ReportXML");

			HasKey(k => new {  k.ReportXMLVersion , k.MasterReport_Id   });

			Property(colReportXMLVersion =>  colReportXMLVersion.ReportXMLVersion).HasColumnName("ReportXMLVersion");
			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colBinaryXML =>  colBinaryXML.BinaryXML).HasColumnName("BinaryXML");
			Property(colPrintCount =>  colPrintCount.PrintCount).HasColumnName("PrintCount");

		}
	}
}