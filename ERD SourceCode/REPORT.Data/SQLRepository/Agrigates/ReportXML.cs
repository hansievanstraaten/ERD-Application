using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportXML : ModelsBase
    {
	private int _ReportXMLVersion;
	private Int64 _MasterReport_Id;
	private byte[] _BinaryXML;
	private Int64 _PrintCount;

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
	/// <para>Binary XML</para>
	/// <para></para>
	/// </summary>
	public byte[] BinaryXML
	{ 
		get
		{
			return this._BinaryXML;
		}

		set
		{
			base.OnPropertyChanged("BinaryXML", ref this._BinaryXML, value);
		}
	}

	/// <summary>
	/// <para>Printed Count</para>
	/// <para></para>
	/// </summary>
	public Int64 PrintCount
	{ 
		get
		{
			return this._PrintCount;
		}

		set
		{
			base.OnPropertyChanged("PrintCount", ref this._PrintCount, value);
		}
	}


    }
}