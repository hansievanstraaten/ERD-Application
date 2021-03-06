using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Data.Models
{
	[ModelNameAttribute("Report", allowHeaderCollapse: true)]
	public class ReportMasterModel : ModelsBase
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
        private string productionConnection;
        private string _DescriptionText;
        private int _ReportXMLVersion;
		private Int64? _CategoryId;
		private bool isActiveVersion;


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
		[FieldInformationAttribute("Name", IsRequired = true, Sort = 1)]
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

		[FieldInformation("Description", Sort = 2)]
		[BrowseButtonAttribute("DescriptionText", "Edit", "Edit")]
		public string DescriptionText
		{
			get
			{
				if (this.Description.HasElements())
				{
					return this.Description.UnzipFile().ParseToString();
				}

				return string.Empty;
			}

			set
			{
				if (value.IsNullEmptyOrWhiteSpace())
				{
					this.Description = new byte[] { };
				}
				else
				{
					this.Description = value.ZipFile();
				}

				base.OnPropertyChanged("DescriptionText", ref this._DescriptionText, value);
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

		[FieldInformation("Report Type", IsReadOnly = true, Sort = 3)]
		public string ReportTypeEnumValue
		{
			get
			{
				if (this.ReportTypeEnum == 0)
				{
					return "Not Set";
				}

				ViSo.SharedEnums.ReportEnums.ReportTypeEnum result = (ViSo.SharedEnums.ReportEnums.ReportTypeEnum)this.ReportTypeEnum;

				return result.GetDescriptionAttribute();
			}
		}

		[FieldInformation("Report Version", IsRequired = true, Sort = 4)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: true)]
		[ValuesSource("ReportXMLVersions")]
		public int ReportXMLVersion
		{
			get
			{
				return this._ReportXMLVersion == 0 ? 1 : this._ReportXMLVersion;
			}

			set
			{
				this._ReportXMLVersion = value;

				base.OnPropertyChanged("ReportXMLVersion");
			}
		}

		[FieldInformation("Is Active Version", Sort = 5)]
		public bool IsActiveVesion
		{
			get
			{
				return this.isActiveVersion;
			}

			set
			{
				this.isActiveVersion = value;

				base.OnPropertyChanged("IsActiveVesion");
			}
		}

		/// <summary>
		/// <para>PaperKind</para>
		/// <para></para>
		/// </summary>
		[FieldInformation("Paper Kind", IsRequired = true, Sort = 6)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("PaperKindValues")]
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
		[FieldInformation("Page Orientation", IsRequired = true, Sort = 7)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("PageOrientationValues")]
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
		[FieldInformation("Cover Page", IsVisible = false, Sort = 8)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("CoverPageValues")]
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
		[FieldInformation("Page Headers and Footers", IsVisible = false, Sort = 9)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("PageHeadersAndFootersValues")]
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
		[FieldInformation("Final Page", IsVisible = false, Sort = 10)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("FinalPageValues")]
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

		[FieldInformation("Production Connection", IsVisible = false, Sort = 11)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("ProductionConnectionValues")]
		public string ProductionConnection
        {
			get
            {
				return this.productionConnection;
            }

			set
            {
				this.productionConnection = value;

				base.OnPropertyChanged(() => this.ProductionConnection);
            }
        }

		/// <summary>
		/// <para>Page Margin Left</para>
		/// <para></para>
		/// </summary>

		[FieldInformation("Page Left Margin", Sort = 12)]
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

		[FieldInformation("Page Right Margin", Sort = 13)]
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

		[FieldInformation("Page Top Margin", Sort = 14)]
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

		[FieldInformation("Page Bottom Margin", Sort = 15)]
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

		/// <summary>
		/// <para>Category ID</para>
		/// <para></para>
		/// </summary>
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

		public DataItemModel[] ReportXMLVersions
		{
			get
			{
				ReportTablesRepository repo = new ReportTablesRepository();

				List<DataItemModel> result = new List<DataItemModel>();

				foreach(int version in repo.GetReportXmlVersions(this.MasterReport_Id))
				{
					result.Add(new DataItemModel { DisplayValue = version.ParseToString(), ItemKey = version });
				}

				return result.ToArray();
			}
		}

		public DataItemModel[] ProductionConnectionValues
        {
			get
            {
				List<DataItemModel> result = new List<DataItemModel>();

				result.Add(new DataItemModel { DisplayValue = Connections.Instance.DefaultConnectionName, ItemKey = Connections.Instance.DefaultConnectionName });

				foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
                {
					result.Add(new DataItemModel { DisplayValue = connectionKey.Value.ConnectionName, ItemKey = connectionKey.Value.ConnectionName });
                }

				return result.ToArray();
            }
        }

		public DataItemModel[] CoverPageValues
		{
			get
			{
				List<DataItemModel> result = new List<DataItemModel>();

				result.Add(new DataItemModel { DisplayValue = "<None>", ItemKey = 0 });

				ReportTablesRepository repo = new ReportTablesRepository();

				foreach (ReportMasterModel item in repo.GetReportMasterByReportTypeEnum((int)ViSo.SharedEnums.ReportEnums.ReportTypeEnum.CoverPage, General.ProjectModel.ModelName))
				{
					result.Add(new DataItemModel { DisplayValue = item.ReportName, ItemKey = item.MasterReport_Id });
				}

				return result.ToArray();
			}
		}

		public DataItemModel[] PageHeadersAndFootersValues
		{
			get
			{
				List<DataItemModel> result = new List<DataItemModel>();

				result.Add(new DataItemModel { DisplayValue = "<None>", ItemKey = 0 });

				ReportTablesRepository repo = new ReportTablesRepository();

				foreach (ReportMasterModel item in repo.GetReportMasterByReportTypeEnum((int)ViSo.SharedEnums.ReportEnums.ReportTypeEnum.PageHeaderAndFooter, General.ProjectModel.ModelName))
				{
					result.Add(new DataItemModel { DisplayValue = item.ReportName, ItemKey = item.MasterReport_Id });
				}

				return result.ToArray();
			}
		}

		public DataItemModel[] FinalPageValues
		{
			get
			{
				List<DataItemModel> result = new List<DataItemModel>();

				result.Add(new DataItemModel { DisplayValue = "<None>", ItemKey = 0 });

				ReportTablesRepository repo = new ReportTablesRepository();

				foreach (ReportMasterModel item in repo.GetReportMasterByReportTypeEnum((int)ViSo.SharedEnums.ReportEnums.ReportTypeEnum.FinalPage, General.ProjectModel.ModelName))
				{
					result.Add(new DataItemModel { DisplayValue = item.ReportName, ItemKey = item.MasterReport_Id });
				}

				return result.ToArray();
			}
		}

		public DataItemModel[] PaperKindValues
		{
			get
			{
				List<DataItemModel> result = new List<DataItemModel>();

				foreach (PaperKind kind in Enum.GetValues(typeof(PaperKind)))
				{
					result.Add(new DataItemModel { DisplayValue = kind.ToString(), ItemKey = (int)kind });
				}

				return result.ToArray();

			}
		}

		public DataItemModel[] PageOrientationValues
		{
			get
			{
				List<DataItemModel> result = new List<DataItemModel>();

				foreach (PageOrientationEnum item in Enum.GetValues(typeof(PageOrientationEnum)))
				{
					result.Add(new DataItemModel { DisplayValue = item.ToString(), ItemKey = (int)item });
				}

				return result.ToArray();
			}
		}
	}
}