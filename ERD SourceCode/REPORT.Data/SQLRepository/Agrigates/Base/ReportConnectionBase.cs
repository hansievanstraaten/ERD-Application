using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class ReportConnectionBase : ModelsBase
	{
		private Int64 _MasterReport_Id;
		private string _ReportConnectionName;
		private int _DatabaseTypeEnum;
		private string _ServerName;
		private string _DatabaseName;
		private string _UserName;
		private string _Password;
		private bool _TrustedConnection;
		private bool _IsProductionConnection;
		private bool _IsActive;


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
		 	
		public string ReportConnectionName
		{
			get
			{
				return this._ReportConnectionName;
			}

			set
			{
				base.OnPropertyChanged("ReportConnectionName", ref this._ReportConnectionName, value);
			}
		}


		// Foreign Keys


		// Columns
		public int DatabaseTypeEnum
		{
			get
			{
				return this._DatabaseTypeEnum;
			}

			set
			{
				base.OnPropertyChanged("DatabaseTypeEnum", ref this._DatabaseTypeEnum, value);
			}
		}
		public string ServerName
		{
			get
			{
				return this._ServerName;
			}

			set
			{
				base.OnPropertyChanged("ServerName", ref this._ServerName, value);
			}
		}
		public string DatabaseName
		{
			get
			{
				return this._DatabaseName;
			}

			set
			{
				base.OnPropertyChanged("DatabaseName", ref this._DatabaseName, value);
			}
		}
		public string UserName
		{
			get
			{
				return this._UserName;
			}

			set
			{
				base.OnPropertyChanged("UserName", ref this._UserName, value);
			}
		}
		public string Password
		{
			get
			{
				return this._Password;
			}

			set
			{
				base.OnPropertyChanged("Password", ref this._Password, value);
			}
		}
		public bool TrustedConnection
		{
			get
			{
				return this._TrustedConnection;
			}

			set
			{
				base.OnPropertyChanged("TrustedConnection", ref this._TrustedConnection, value);
			}
		}
		public bool IsProductionConnection
		{
			get
			{
				return this._IsProductionConnection;
			}

			set
			{
				base.OnPropertyChanged("IsProductionConnection", ref this._IsProductionConnection, value);
			}
		}
		public bool IsActive
		{
			get
			{
				return this._IsActive;
			}

			set
			{
				base.OnPropertyChanged("IsActive", ref this._IsActive, value);
			}
		}


	}
}