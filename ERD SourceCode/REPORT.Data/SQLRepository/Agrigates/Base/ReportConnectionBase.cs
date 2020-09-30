using System;
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
	/// <para></para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Database Type Enum</para>
	/// <para>The Database type for the connection</para>
	/// </summary>
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

	/// <summary>
	/// <para>Server Name</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Database Name</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>User Name</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Password</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Is Trusted Connection</para>
	/// <para></para>
	/// </summary>
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

	/// <summary>
	/// <para>Is Production Connection</para>
	/// <para></para>
	/// </summary>
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