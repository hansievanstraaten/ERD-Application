using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace REPORT.Data.SQLRepository.Repositories
{
	public class DataSourceRepository : DataSourceRepository_Base
	{
		public DataSourceRepository()
		{
		}

		// Add your code here

		public void SetIsAvailable(long masterReportId, bool isAvailable)
        {
			foreach(DataSourceTable table in base.dataContext.DataSourceTables.Where(mr => mr.MasterReport_Id == masterReportId))
            {
				table.IsAvailable = isAvailable;
            }

			base.dataContext.SaveChanges();
        }
	}
}