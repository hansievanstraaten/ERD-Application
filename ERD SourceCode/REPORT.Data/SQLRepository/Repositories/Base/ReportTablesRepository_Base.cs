using GeneralExtensions;
using REPORT.Data.Models;
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
		
		public ReportConnectionModel GetReportConnectionByPrimaryKey (Int64 MasterReport_Id, string ReportConnectionName  )
		{
			ReportConnection result =this.dataContext
				.ReportConnections
				.FirstOrDefault(pk => pk.MasterReport_Id == MasterReport_Id && pk.ReportConnectionName == ReportConnectionName  );

			if (result == null)
			{
				return null;
			}

			return result.CopyToObject(new ReportConnectionModel()) as ReportConnectionModel;
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
		
		public List<ReportConnectionModel> GetReportConnectionByForeignKeyMasterReport_Id (Int64 MasterReport_Id)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.MasterReport_Id == MasterReport_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
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
		
		public List<ReportMasterModel> GetReportMasterByPaperKindEnum (int? PaperKindEnum)
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
		
		public List<ReportMasterModel> GetReportMasterByPageOrientationEnum (int PageOrientationEnum)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.PageOrientationEnum == PageOrientationEnum)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByCoverPage_Id (Int64? CoverPage_Id)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.CoverPage_Id == CoverPage_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByHeaderAndFooterPage_Id (Int64? HeaderAndFooterPage_Id)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.HeaderAndFooterPage_Id == HeaderAndFooterPage_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByFinalPage_Id (Int64? FinalPage_Id)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.FinalPage_Id == FinalPage_Id)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByPageMarginLeft (int? PageMarginLeft)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.PageMarginLeft == PageMarginLeft)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByPageMarginRight (int? PageMarginRight)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.PageMarginRight == PageMarginRight)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByPageMarginTop (int? PageMarginTop)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.PageMarginTop == PageMarginTop)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportMasterModel));

			return objectList.TryCast<ReportMasterModel>().ToList();
		}
		
		public List<ReportMasterModel> GetReportMasterByPageMarginBottom (int? PageMarginBottom)
		{
			List<ReportMaster> result = this.dataContext
				.ReportsMaster
				.Where(fk => fk.PageMarginBottom == PageMarginBottom)
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
		
		public List<ReportConnectionModel> GetReportConnectionByDatabaseTypeEnum (int DatabaseTypeEnum)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.DatabaseTypeEnum == DatabaseTypeEnum)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByServerName (string ServerName)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.ServerName == ServerName)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByDatabaseName (string DatabaseName)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.DatabaseName == DatabaseName)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByUserName (string UserName)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.UserName == UserName)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByPassword (string Password)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.Password == Password)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByTrustedConnection (bool TrustedConnection)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.TrustedConnection == TrustedConnection)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByIsProductionConnection (bool IsProductionConnection)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.IsProductionConnection == IsProductionConnection)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
		}
		
		public List<ReportConnectionModel> GetReportConnectionByIsActive (bool IsActive)
		{
			List<ReportConnection> result = this.dataContext
				.ReportConnections
				.Where(fk => fk.IsActive == IsActive)
				.ToList();

			if (result.Count == 0)
			{
				return new List<ReportConnectionModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(ReportConnectionModel));

			return objectList.TryCast<ReportConnectionModel>().ToList();
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

		public void UpdateReportConnection(ReportConnectionModel model)
		{
			ReportConnection existing = this.dataContext
				.ReportConnections
				.Where(rx => rx.MasterReport_Id == model.MasterReport_Id && rx.ReportConnectionName == model.ReportConnectionName  )
				.FirstOrDefault();

			if (existing == null)
			{
				existing = model.CopyToObject(new ReportConnection()) as ReportConnection;

				this.dataContext.ReportConnections.Add(existing);
			}
			else
			{
				existing = model.CopyToObject(existing) as ReportConnection;
			}

			this.dataContext.SaveChanges();

			model = existing.CopyToObject(model) as ReportConnectionModel;
		}


	}
}