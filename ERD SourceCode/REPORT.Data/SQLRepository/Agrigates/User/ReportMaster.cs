using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportMaster : ReportMasterBase
	{
        // Inheritance was only done as a POC and there is not real practical reason for doing so

        [ForeignKey(nameof(CategoryId))]
        public ReportCategory ReportCategory { get; set; }
                
        public List<ReportConnection> ReportConnections { get; set; } = new();

        public List<ReportXML> ReportXML { get; set; } = new();
    }
}