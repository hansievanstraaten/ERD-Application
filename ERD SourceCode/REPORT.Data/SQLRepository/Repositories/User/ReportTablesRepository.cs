using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using System;
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
			ReportXML model = this.dataContext
				.ReportsXML
				.FirstOrDefault(r => r.MasterReport_Id == MasterReport_Id
						 && r.IsActiveVersion == true);

			if (model != null)
			{
				return model.ReportXMLVersion;
			}

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
	
		public List<int> GetReportXmlVersions(long MasterReport_Id)
		{
			List<ReportXML> result = this.dataContext
				.ReportsXML
				.Where(r => r.MasterReport_Id == MasterReport_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<int>();
			}

			return result.OrderBy(o => o.ReportXMLVersion).Select(r => r.ReportXMLVersion).ToList();
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

		public List<ReportMasterModel> GetReportMasterByCategoryId( long categoryId)
		{
			List<ReportMaster> agrigates = this.dataContext
				.ReportsMaster
				.Where(fk => fk.CategoryId == categoryId)
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

		public List<ReportXMLPrintParameterModel> GetPrintparameters(long masterReport_Id)
		{
			return this.GetPrintparameters(masterReport_Id, this.GetReportXMLVersion(masterReport_Id));
		}

		public List<ReportXMLPrintParameterModel> GetPrintparameters(long masterReport_Id, int reportXMLVersion)
		{
			List<ReportXMLPrintParameter> result = base.dataContext
				.ReportXMLPrintParameters
				.Where(r => r.MasterReport_Id == masterReport_Id
						 && r.ReportXMLVersion == reportXMLVersion)
				.ToList();

			List<object> objectList = result.CopyToObject(typeof(ReportXMLPrintParameterModel));

			return objectList.TryCast<ReportXMLPrintParameterModel>().ToList();
		}

		public bool IsActiveVersion(long masterReport_Id, int reportXMLVersion)
		{
			ReportXML xml = base.dataContext
				.ReportsXML
				.FirstOrDefault(rx => rx.MasterReport_Id == masterReport_Id
								   && rx.ReportXMLVersion == reportXMLVersion);

			return xml == null ? false : xml.IsActiveVersion;
		}

		public bool CategoryHaveReports(long categoryId)
		{
			List<ReportCategory> categories = base.dataContext.ReportCategories.ToList();

			return this.CheckCategoryHaveReports(categories, categoryId);			
		}

		private bool CheckCategoryHaveReports(List<ReportCategory> categories, long categoryId)
		{
			if (this.CategoryReportCount(categoryId) > 0)
			{
				return true;
			}

			foreach (ReportCategory category in categories.Where(pc => pc.ParentCategoryId == categoryId))
			{
				if (this.CheckCategoryHaveReports(categories, category.CategoryId))
				{
					return true;
				}
			}

			return false;
		}

		public int CategoryReportCount(long categoryId)
		{
			return base.dataContext
				.ReportsMaster
				.Where(ci => ci.CategoryId == categoryId)
				.Count();
		}

		public void DeleteCategory(long categoryId)
		{
			int reportCount = base.dataContext
				.ReportsMaster
				.Where(ci => ci.CategoryId == categoryId)
				.Count();

			if (reportCount > 0)
			{
				throw new ApplicationException("Cannot delete Category while reports are attached.");
			}

			ReportCategory category = base.dataContext
				.ReportCategories
				.First(rc => rc.CategoryId == categoryId);

			category.IsActive = false;

			base.dataContext.SaveChanges();
		}

		public void DisableReportXMLPrintFilters(long masterReportId, int reportXMLVersion)
		{
			List<ReportXMLPrintParameter> parametersList = base.dataContext
				.ReportXMLPrintParameters
				.Where(p => p.MasterReport_Id == masterReportId
						 && p.ReportXMLVersion == reportXMLVersion)
				.ToList();

			foreach(ReportXMLPrintParameter item in parametersList)
			{
				item.IsActive = false;
			}

			base.dataContext.SaveChanges();
		}

		new public void UpdateReportXML(ReportXMLModel model)
		{
			if (model.IsActiveVersion)
			{
				foreach (ReportXML xml in this.dataContext.ReportsXML.Where(rm => rm.MasterReport_Id == model.MasterReport_Id))
				{
					xml.IsActiveVersion = false;
				}
			}

			ReportXML existing = this.dataContext
				.ReportsXML
				.Where(rx => rx.ReportXMLVersion == model.ReportXMLVersion && rx.MasterReport_Id == model.MasterReport_Id)
				.FirstOrDefault();

			if (existing == null)
			{
				existing = model.CopyToObject(new ReportXML()) as ReportXML;

				this.dataContext.ReportsXML.Add(existing);
			}
			else
			{
				existing = model.CopyToObject(existing) as ReportXML;
			}

			this.dataContext.SaveChanges();

			model = existing.CopyToObject(model) as ReportXMLModel;
		}

	}
}