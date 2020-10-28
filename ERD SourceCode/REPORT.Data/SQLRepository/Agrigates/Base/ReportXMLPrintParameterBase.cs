using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class ReportXMLPrintParameterBase : ModelsBase
	{
		private string _TableName;
		private string _ColumnName;
		private int _ReportXMLVersion;
		private Int64 _MasterReport_Id;
		private bool _IsActive;
		private string _FilterCaption;
		private string _DefaultValue;


		// Primary Keys
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
		[Key]
		 	
		public string ColumnName
		{
			get
			{
				return this._ColumnName;
			}

			set
			{
				base.OnPropertyChanged("ColumnName", ref this._ColumnName, value);
			}
		}
		[Key]
		 	
		public int ReportXMLVersion
		{
			get
			{
				return this._ReportXMLVersion;
			}

			set
			{
				base.OnPropertyChanged("ReportXMLVersion", ref this._ReportXMLVersion, value);
			}
		}
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
		public string FilterCaption
		{
			get
			{
				return this._FilterCaption;
			}

			set
			{
				base.OnPropertyChanged("FilterCaption", ref this._FilterCaption, value);
			}
		}
		public string DefaultValue
		{
			get
			{
				return this._DefaultValue;
			}

			set
			{
				base.OnPropertyChanged("DefaultValue", ref this._DefaultValue, value);
			}
		}


	}
}