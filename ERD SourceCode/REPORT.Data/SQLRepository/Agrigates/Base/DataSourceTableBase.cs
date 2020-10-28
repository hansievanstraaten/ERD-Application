using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class DataSourceTableBase : ModelsBase
	{
		private Int64 _MasterReport_Id;
		private string _TableName;
		private bool _IsAvailable;


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
		[Key]
		 	
		public string TableName
		{
			get
			{
				return this._TableName;
			}

			set
			{
				base.OnPropertyChanged("TableName", ref this._TableName, value);
			}
		}


		// Foreign Keys


		// Columns
		public bool IsAvailable
		{
			get
			{
				return this._IsAvailable;
			}

			set
			{
				base.OnPropertyChanged("IsAvailable", ref this._IsAvailable, value);
			}
		}


	}
}