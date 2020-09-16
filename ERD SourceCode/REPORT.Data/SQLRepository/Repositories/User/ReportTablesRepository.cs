using GeneralExtensions;
using REPORT.Data.SQLRepository.Agrigates;
using System.Linq;

namespace REPORT.Data.SQLRepository.Repositories
{
	public class ReportTablesRepository : ReportTablesRepository_Base
	{
		public ReportTablesRepository()
		{
		}

        public int GetReportXMLVersion(long MasterReport_Id)
        {
			int result = this.dataContext
				.ReportsXML
				.Where(r => r.MasterReport_Id == MasterReport_Id)
				.Max(m => m.ReportXMLVersion);

			if (result == 0)
            {
				return 1;
            }

			return result;
		}
	}
}