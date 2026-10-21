using System.ComponentModel.DataAnnotations.Schema;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportXMLPrintParameter : ReportXMLPrintParameterBase
	{
        // Inheritance was only done as a POC and there is not real practical reason for doing so

        [ForeignKey($"{nameof(ReportXMLVersion)}, {nameof(MasterReport_Id)}")]
        public ReportXML ReportXML { get; set; }
    }
}