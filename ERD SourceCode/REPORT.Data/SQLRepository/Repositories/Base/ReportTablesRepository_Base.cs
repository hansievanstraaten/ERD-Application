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
		
		public ReportMasterModel GetReportMasterByPrimaryKey (Int64 MasterReport_Id  )
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
		
		public ReportXMLModel GetReportXMLByPrimaryKey (int ReportXMLVersion, Int64 MasterReport_Id  )
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

		
		public List<ReportXMLModel> GetReportXMLByForeignKeyMasterReport_Id (Int64 MasterReport_Id)
		{
			List<ReportXML> result = this.dataContext
				.ReportsXML
				.Where(fk => fk.MasterReport_Id == MasterReport_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportXMLModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportXMLModel));

			return objectList.TryCast<ReportXMLModel>().ToList();
		}

		
		public List<ReportMasterModel> GetReportMasterByReportName (string ReportName)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.ReportName == ReportName)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByDescription (byte[] Description)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.Description == Description)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByReportTypeEnum (int ReportTypeEnum)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.ReportTypeEnum == ReportTypeEnum)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByPaperKindEnum (int PaperKindEnum)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.PaperKindEnum == PaperKindEnum)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportXMLModel> GetReportXMLByBinaryXML (byte[] BinaryXML)
		{
			List<ReportXML> result = this.dataContext
				.ReportsXML
				.Where(fk => fk.BinaryXML == BinaryXML)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportXMLModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportXMLModel));

			return objectList.TryCast<ReportXMLModel>().ToList();
		}
		
		public List<ReportXMLModel> GetReportXMLByPrintCount (Int64 PrintCount)
		{
			List<ReportXML> result = this.dataContext
				.ReportsXML
				.Where(fk => fk.PrintCount == PrintCount)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportXMLModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportXMLModel));

			return objectList.TryCast<ReportXMLModel>().ToList();
		}

		public void UpdateReportMaster(ReportMasterModel model)
		{
			ReportMaster existing = this.dataContext
				.ReportsMaster
				.Where(rx => rx.MasterReport_Id == model.MasterReport_Id  )
				.FirstOrDefault();

			if (existing == null)
			{
				existing = model.CopyToObject(new ReportMaster()) as ReportMaster;

				this.dataContext.ReportsMaster.Add(existing);
			}
			else
			{
				existing = model.CopyToObject(existing) as ReportMaster;
			}

			this.dataContext.SaveChanges();

			model = existing.CopyToObject(model) as ReportMasterModel;
		}

		public void UpdateReportXML(ReportXMLModel model)
		{
			ReportXML existing = this.dataContext
				.ReportsXML
				.Where(rx => rx.ReportXMLVersion == model.ReportXMLVersion && rx.MasterReport_Id == model.MasterReport_Id  )
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