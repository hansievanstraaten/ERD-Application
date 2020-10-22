using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public abstract class ReportWhereDetailBase : ModelsBase
    {
	private string _WhereOption;
	private string _WhereValue;
	private Int64 _MasterReport_Id;
	private string _ReplaceColumn;
	private string _ReplaceTable;
	private bool _IsActive;
	private bool _IsColumn;

	/// <summary>
	/// <para>WhereOption</para>
	/// <para></para>
	/// </summary>
	public string WhereOption
	{ 
		get
		{
			return this._WhereOption;
		}

		set
		{
			base.OnPropertyChanged("WhereOption", ref this._WhereOption, value);
		}
	}

	/// <summary>
	/// <para>Where Value</para>
	/// <para></para>
	/// </summary>
	public string WhereValue
	{ 
		get
		{
			return this._WhereValue;
		}

		set
		{
			base.OnPropertyChanged("WhereValue", ref this._WhereValue, value);
		}
	}

	/// <summary>
	/// <para>Master Report ID</para>
	/// <para>Master Report ID</para>
	/// </summary>
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

	/// <summary>
	/// <para>ReplaceColumn</para>
	/// <para></para>
	/// </summary>
	public string ReplaceColumn
	{ 
		get
		{
			return this._ReplaceColumn;
		}

		set
		{
			base.OnPropertyChanged("ReplaceColumn", ref this._ReplaceColumn, value);
		}
	}

	/// <summary>
	/// <para>ReplaceTable</para>
	/// <para></para>
	/// </summary>
	public string ReplaceTable
	{ 
		get
		{
			return this._ReplaceTable;
		}

		set
		{
			base.OnPropertyChanged("ReplaceTable", ref this._ReplaceTable, value);
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
	/// <para>Is Derived From Column</para>
	/// <para></para>
	/// </summary>
	public bool IsColumn
	{ 
		get
		{
			return this._IsColumn;
		}

		set
		{
			base.OnPropertyChanged("IsColumn", ref this._IsColumn, value);
		}
	}


    }
}