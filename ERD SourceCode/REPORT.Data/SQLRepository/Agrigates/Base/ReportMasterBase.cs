using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
		private Int64? _CategoryId;


		// Primary Keys
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)] 	
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


		// Foreign Keys
		public Int64? CategoryId
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


		// Columns
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