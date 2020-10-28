using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace REPORT.Data.SQLRepository.Repositories
{
	public abstract class DataSourceRepository_Base
	{
		public DataSourceContext dataContext;

		public DataSourceRepository_Base()
		{
			this.dataContext = new DataSourceContext();
		}
		
		public DataSourceMasterModel GetDataSourceMasterByPrimaryKey (Int64 MasterReport_Id  )
		{
			DataSourceMaster result =this.dataContext
				.DataSourcesMaster
				.FirstOrDefault(pk => pk.MasterReport_Id == MasterReport_Id  );

			if (result == null)
			{
				return null;
			}

			return result.CopyToObject(new DataSourceMasterModel()) as DataSourceMasterModel;
		}
		
		public DataSourceTableModel GetDataSourceTableByPrimaryKey (Int64 MasterReport_Id, string TableName  )
		{
			DataSourceTable result =this.dataContext
				.DataSourceTables
				.FirstOrDefault(pk => pk.MasterReport_Id == MasterReport_Id && pk.TableName == TableName  );

			if (result == null)
			{
				return null;
			}

			return result.CopyToObject(new DataSourceTableModel()) as DataSourceTableModel;
		}


		
		public List<DataSourceMasterModel> GetDataSourceMasterByMainTableName (string MainTableName)
		{
			List<DataSourceMaster> result = this.dataContext
				.DataSourcesMaster
				.Where(fk => fk.MainTableName == MainTableName)
				.ToList();

			if (result.Count == 0)
			{
				return new List<DataSourceMasterModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(DataSourceMasterModel));

			return objectList.TryCast<DataSourceMasterModel>().ToList();
		}
		
		public List<DataSourceTableModel> GetDataSourceTableByIsAvailable (bool IsAvailable)
		{
			List<DataSourceTable> result = this.dataContext
				.DataSourceTables
				.Where(fk => fk.IsAvailable == IsAvailable)
				.ToList();

			if (result.Count == 0)
			{
				return new List<DataSourceTableModel>();
			}

			
			List<object> objectList = result.CopyToObject(typeof(DataSourceTableModel));

			return objectList.TryCast<DataSourceTableModel>().ToList();
		}

		public void UpdateDataSourceMaster(DataSourceMasterModel model)
		{
			DataSourceMaster existing = this.dataContext
				.DataSourcesMaster
				.Where(rx => rx.MasterReport_Id == model.MasterReport_Id  )
				.FirstOrDefault();

			if (existing == null)
			{
				existing = model.CopyToObject(new DataSourceMaster()) as DataSourceMaster;

				this.dataContext.DataSourcesMaster.Add(existing);
			}
			else
			{
				existing = model.CopyToObject(existing) as DataSourceMaster;
			}

			this.dataContext.SaveChanges();

			model = existing.CopyToObject(model) as DataSourceMasterModel;
		}

		public void UpdateDataSourceTable(DataSourceTableModel model)
		{
			DataSourceTable existing = this.dataContext
				.DataSourceTables
				.Where(rx => rx.MasterReport_Id == model.MasterReport_Id && rx.TableName == model.TableName  )
				.FirstOrDefault();

			if (existing == null)
			{
				existing = model.CopyToObject(new DataSourceTable()) as DataSourceTable;

				this.dataContext.DataSourceTables.Add(existing);
			}
			else
			{
				existing = model.CopyToObject(existing) as DataSourceTable;
			}

			this.dataContext.SaveChanges();

			model = existing.CopyToObject(model) as DataSourceTableModel;
		}


	}
}