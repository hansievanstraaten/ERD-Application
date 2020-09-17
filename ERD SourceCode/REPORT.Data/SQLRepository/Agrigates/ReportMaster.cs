using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportMaster : ModelsBase
    {
	private Int64 _MasterReport_Id;
	private string _ReportName;
	private byte[] _Description;
	private int _ReportTypeEnum;
	private int _PaperKindEnum;

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
	/// <para>Report Name</para>
	/// <para></para>
	/// </summary>
	public string ReportName
	{ 
		get
		{
			return this._ReportName;
		}

		set
		{
			base.OnPropertyChanged("ReportName", ref this._ReportName, value);
		}
	}

	/// <summary>
	/// <para>Description</para>
	/// <para></para>
	/// </summary>
	public byte[] Description
	{ 
		get
		{
			return this._Description;
		}

		set
		{
			base.OnPropertyChanged("Description", ref this._Description, value);
		}
	}

	/// <summary>
	/// <para>Report Type Enumerator</para>
	/// <para></para>
	/// </summary>
	public int ReportTypeEnum
	{ 
		get
		{
			return this._ReportTypeEnum;
		}

		set
		{
			base.OnPropertyChanged("ReportTypeEnum", ref this._ReportTypeEnum, value);
		}
	}

	/// <summary>
	/// <para>PaperKind</para>
	/// <para></para>
	/// </summary>
	public int PaperKindEnum
	{ 
		get
		{
			return this._PaperKindEnum;
		}

		set
		{
			base.OnPropertyChanged("PaperKindEnum", ref this._PaperKindEnum, value);
		}
	}


    }
}