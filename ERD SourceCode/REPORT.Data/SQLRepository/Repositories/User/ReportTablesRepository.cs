using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
	
		public void SetReportConnectionsSttus(long masterReportId, bool isProduction, bool isActive)
        {
			List<ReportConnection> connections = base.dataContext
				.ReportConnections
				.Where(cn => cn.MasterReport_Id == masterReportId)
				.ToList();

			foreach (ReportConnection connection in connections)
            {
				connection.IsProductionConnection = isProduction;

				connection.IsActive = isActive;
            }

			base.dataContext.SaveChanges();
        }
	
		public ReportConnectionModel GetProductionOrConnectionModel(long masterReport_Id)
        {
			ReportConnection result = base.dataContext
				.ReportConnections
				.FirstOrDefault(d => d.IsProductionConnection == true
							&& d.IsActive == true);

			if (result == null)
            {
				return null;
            }

			return result.CopyToObject(new ReportConnectionModel()) as ReportConnectionModel;
		}

		public List<ReportMasterModel> GetReportMasterByReportTypeEnum(int ReportTypeEnum, string projectName)
		{
			List<ReportMaster> agrigates = this.dataContext
				.ReportsMaster
				.Where(fk => fk.ReportTypeEnum == ReportTypeEnum
						  && fk.ProjectName == projectName)
				.ToList();

			if (agrigates.Count == 0)
			{
				return new List<ReportMasterModel>();
			}


			List<object> objectList = agrigates.CopyToObject(typeof(ReportMasterModel));

			List<ReportMasterModel> result = objectList.TryCast<ReportMasterModel>().OrderBy(n => n.ReportName).ToList();

			foreach (ReportMasterModel item in result)
			{
				item.ReportXMLVersion = this.GetReportXMLVersion(item.MasterReport_Id);
			}

			return result;
		}

		public List<ReportMasterModel> GetReportMasterByReportTypeEnum(int ReportTypeEnum, string projectName, long categoryId)
		{
			List<ReportMaster> agrigates = this.dataContext
				.ReportsMaster
				.Where(fk => fk.ReportTypeEnum == ReportTypeEnum
						  && fk.ProjectName == projectName
						  && fk.CategoryId == categoryId)
				.ToList();

			if (agrigates.Count == 0)
			{
				return new List<ReportMasterModel>();
			}


			List<object> objectList = agrigates.CopyToObject(typeof(ReportMasterModel));

			List<ReportMasterModel> result = objectList.TryCast<ReportMasterModel>().OrderBy(n => n.ReportName).ToList();

			foreach (ReportMasterModel item in result)
			{
				item.ReportXMLVersion = this.GetReportXMLVersion(item.MasterReport_Id);
			}

			return result;
		}


		public List<ReportCategoryModel> GetActiveCategories()
        {
			List<ReportCategory> result = base.dataContext
				.ReportCategories
				.Where(a => a.IsActive == true)
				.ToList();

			List<object> objectList = result.CopyToObject(typeof(ReportCategoryModel));

			return objectList.TryCast<ReportCategoryModel>().ToList();
		}
	}
}