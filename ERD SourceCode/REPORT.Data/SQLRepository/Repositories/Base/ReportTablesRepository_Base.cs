using GeneralExtensions;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace REPORT.Data.SQLRepository.Repositories
{
	public abstract class ReportTablesRepository_Base
	{
		public ReportTablesContext dataContext;

		public ReportTablesRepository_Base()
		{
			this.dataContext = new ReportTablesContext();
		}

		public ReportMasterModel GetReportMasterByPrimaryKey (Int64 MasterReport_Id )
		{
			ReportMaster result =this.dataContext
				.ReportsMaster
				.FirstOrDefault(pk => pk.MasterReport_Id == MasterReport_Id  );

			if (result == null)
			{
				return null;
			}

			return result.CopyToObject(new ReportMasterModel()) as ReportMasterModel;
		}

		public ReportXMLModel GetReportXMLByPrimaryKey (int ReportXMLVersion, Int64 MasterReport_Id )
		{
			ReportXML result =this.dataContext
				.ReportsXML
				.FirstOrDefault(pk => pk.ReportXMLVersion == ReportXMLVersion && pk.MasterReport_Id == MasterReport_Id  );

			if (result == null)
			{
				return null;
			}

			return result.CopyToObject(new ReportXMLModel()) as ReportXMLModel;
		}


		public List<ReportXMLModel> GetReportXMLByMasterReport_Id (Int64 MasterReport_Id)
		{
			List<ReportXML> result = this.dataContext
				.ReportsXML
				.Where(fk => fk.MasterReport_Id == MasterReport_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportXMLModel>();
			}

			return result.TryCast<ReportXMLModel>().ToList();
		}
	}
}