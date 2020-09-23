using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public abstract class DataSourceMasterBase : ModelsBase
    {
	private Int64 _MasterReport_Id;
	private string _MainTableName;

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
	/// <para>Main Table Name</para>
	/// <para></para>
	/// </summary>
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