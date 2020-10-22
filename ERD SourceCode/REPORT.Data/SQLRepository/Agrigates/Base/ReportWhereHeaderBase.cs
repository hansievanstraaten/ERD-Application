using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public abstract class ReportWhereHeaderBase : ModelsBase
    {
	private string _ReplaceColumn;
	private string _ReplaceTable;
	private Int64 _MasterReport_Id;
	private string _UseColumn;
	private string _UseTable;
	private bool _IsActive;

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
	/// <para>Use Column</para>
	/// <para></para>
	/// </summary>
	public string UseColumn
	{ 
		get
		{
			return this._UseColumn;
		}

		set
		{
			base.OnPropertyChanged("UseColumn", ref this._UseColumn, value);
		}
	}

	/// <summary>
	/// <para>Use Table</para>
	/// <para></para>
	/// </summary>
	public string UseTable
	{ 
		get
		{
			return this._UseTable;
		}

		set
		{
			base.OnPropertyChanged("UseTable", ref this._UseTable, value);
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


    }
}