using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class DataSourceMasterBase : ModelsBase
	{
		private Int64 _MasterReport_Id;
		private string _MainTableName;


		// Primary Keys
		[Key]
		 	
		public Int64 MasterReport_Id
		{
			get
			{
				return this._MasterReport_Id;
			}

			set
			{
				base.OnPropertyChanged("MasterReport_Id", ref this._MasterReport_Id, value);
			}
		}


		// Foreign Keys


		// Columns
		public string MainTableName
		{
			get
			{
				return this._MainTableName;
			}

			set
			{
				base.OnPropertyChanged("MainTableName", ref this._MainTableName, value);
			}
		}


	}
}