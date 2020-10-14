using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public abstract class ReportCategoryBase : ModelsBase
    {
	private Int64 _CategoryId;
	private string _CategoryName;
	private bool _IsActive;
	private Int64? _ParentCategoryId;

	/// <summary>
	/// <para>Category ID</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Category Name</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>IsActive</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>ParentCategoryId</para>
	/// <para></para>
	/// </summary>
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