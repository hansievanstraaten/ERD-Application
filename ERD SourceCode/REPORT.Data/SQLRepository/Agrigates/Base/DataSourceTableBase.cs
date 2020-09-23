using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public abstract class DataSourceTableBase : ModelsBase
    {
	private Int64 _MasterReport_Id;
	private string _TableName;
	private bool _IsAvailable;

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
	/// <para>Is Available</para>
	/// <para></para>
	/// </summary>
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