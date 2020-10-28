using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using System;
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

		public List<DataSourceMasterModel> GetDataSourceMasterByForeignKeyMasterReport_Id(Int64 MasterReport_Id)
		{
			List<DataSourceMaster> result = this.dataContext
				.DataSourcesMaster
				.Where(fk => fk.MasterReport_Id == MasterReport_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<DataSourceMasterModel>();
			}


			List<object> objectList = result.CopyToObject(typeof(DataSourceMasterModel));

			return objectList.TryCast<DataSourceMasterModel>().ToList();
		}

		public List<DataSourceTableModel> GetDataSourceTableByForeignKeyMasterReport_Id(Int64 MasterReport_Id)
		{
			List<DataSourceTable> result = this.dataContext
				.DataSourceTables
				.Where(fk => fk.MasterReport_Id == MasterReport_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<DataSourceTableModel>();
			}


			List<object> objectList = result.CopyToObject(typeof(DataSourceTableModel));

			return objectList.TryCast<DataSourceTableModel>().ToList();
		}
	}
}