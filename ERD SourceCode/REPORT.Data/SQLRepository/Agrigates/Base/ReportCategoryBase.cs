using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public abstract class ReportCategoryBase : ModelsBase
	{
		private Int64 _CategoryId;
		private string _CategoryName;
		private bool _IsActive;
		private Int64? _ParentCategoryId;


		// Primary Keys
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] 	
		public Int64 CategoryId
		{
			get
			{
				return this._CategoryId;
			}

			set
			{
				base.OnPropertyChanged("CategoryId", ref this._CategoryId, value);
			}
		}


		// Foreign Keys


		// Columns
		public string CategoryName
		{
			get
			{
				return this._CategoryName;
			}

			set
			{
				base.OnPropertyChanged("CategoryName", ref this._CategoryName, value);
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
		public Int64? ParentCategoryId
		{
			get
			{
				return this._ParentCategoryId;
			}

			set
			{
				base.OnPropertyChanged("ParentCategoryId", ref this._ParentCategoryId, value);
			}
		}


	}
}