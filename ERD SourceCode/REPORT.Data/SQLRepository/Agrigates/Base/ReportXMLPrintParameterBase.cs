using System;
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

	/// <summary>
	/// <para>Table Name</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para></para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Report XML Version</para>
	/// <para>Report XML Version Number</para>
	/// </summary>
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
	/// <para>Filter Caption</para>
	/// <para></para>
	/// </summary>
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


    }
}