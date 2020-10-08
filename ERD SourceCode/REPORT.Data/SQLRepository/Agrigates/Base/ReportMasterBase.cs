using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public abstract class ReportMasterBase : ModelsBase
    {
	private Int64 _MasterReport_Id;
	private string _ReportName;
	private byte[] _Description;
	private int _ReportTypeEnum;
	private int _PaperKindEnum;
	private int _PageOrientationEnum;
	private Int64? _CoverPage_Id;
	private Int64? _HeaderAndFooterPage_Id;
	private Int64? _FinalPage_Id;
	private int _PageMarginLeft;
	private int _PageMarginRight;
	private int _PageMarginTop;
	private int _PageMarginBottom;
	private string _ProjectName;

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

	/// <summary>
	/// <para>PageOrientation</para>
	/// <para></para>
	/// </summary>
	public int PageOrientationEnum
	{ 
		get
		{
			return this._PageOrientationEnum;
		}

		set
		{
			base.OnPropertyChanged("PageOrientationEnum", ref this._PageOrientationEnum, value);
		}
	}

	/// <summary>
	/// <para>Cover Page ID</para>
	/// <para></para>
	/// </summary>
	public Int64? CoverPage_Id
	{ 
		get
		{
			return this._CoverPage_Id;
		}

		set
		{
			base.OnPropertyChanged("CoverPage_Id", ref this._CoverPage_Id, value);
		}
	}

	/// <summary>
	/// <para>Headers and Footers Page ID</para>
	/// <para></para>
	/// </summary>
	public Int64? HeaderAndFooterPage_Id
	{ 
		get
		{
			return this._HeaderAndFooterPage_Id;
		}

		set
		{
			base.OnPropertyChanged("HeaderAndFooterPage_Id", ref this._HeaderAndFooterPage_Id, value);
		}
	}

	/// <summary>
	/// <para>Final Page ID</para>
	/// <para></para>
	/// </summary>
	public Int64? FinalPage_Id
	{ 
		get
		{
			return this._FinalPage_Id;
		}

		set
		{
			base.OnPropertyChanged("FinalPage_Id", ref this._FinalPage_Id, value);
		}
	}

	/// <summary>
	/// <para>Page Margin Left</para>
	/// <para></para>
	/// </summary>
	public int PageMarginLeft
	{ 
		get
		{
			return this._PageMarginLeft;
		}

		set
		{
			base.OnPropertyChanged("PageMarginLeft", ref this._PageMarginLeft, value);
		}
	}

	/// <summary>
	/// <para>Page Margin Right</para>
	/// <para></para>
	/// </summary>
	public int PageMarginRight
	{ 
		get
		{
			return this._PageMarginRight;
		}

		set
		{
			base.OnPropertyChanged("PageMarginRight", ref this._PageMarginRight, value);
		}
	}

	/// <summary>
	/// <para>Page Margin Top</para>
	/// <para></para>
	/// </summary>
	public int PageMarginTop
	{ 
		get
		{
			return this._PageMarginTop;
		}

		set
		{
			base.OnPropertyChanged("PageMarginTop", ref this._PageMarginTop, value);
		}
	}

	/// <summary>
	/// <para>Page Margin Bottom</para>
	/// <para></para>
	/// </summary>
	public int PageMarginBottom
	{ 
		get
		{
			return this._PageMarginBottom;
		}

		set
		{
			base.OnPropertyChanged("PageMarginBottom", ref this._PageMarginBottom, value);
		}
	}

	/// <summary>
	/// <para>Project Name</para>
	/// <para></para>
	/// </summary>
	public string ProjectName
	{ 
		get
		{
			return this._ProjectName;
		}

		set
		{
			base.OnPropertyChanged("ProjectName", ref this._ProjectName, value);
		}
	}


    }
}