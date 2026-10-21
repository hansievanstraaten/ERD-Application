using System.Collections.Generic;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportCategory : ReportCategoryBase
	{
        // Inheritance was only done as a POC and there is not real practical reason for doing so

        public List<ReportMaster> Reports { get; set; } = new List<ReportMaster>();
    }
}