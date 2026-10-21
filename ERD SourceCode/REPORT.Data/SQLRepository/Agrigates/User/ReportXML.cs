using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportXML : ReportXMLBase
	{
        // Inheritance was only done as a POC and there is not real practical reason for doing so

        [ForeignKey(nameof(MasterReport_Id))]
        public ReportMaster ReportMaster { get; set; }

        public List<ReportXMLPrintParameter> Parameters { get; set; } = new();
    }
}