using GeneralExtensions;
using System;
using System.Drawing.Printing;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ToolModels;
using WPF.Tools.Exstention;
using System.Collections.Generic;
using WPF.Tools.ModelViewer;

namespace REPORT.Data.SQLRepository.Agrigates
{
	[ModelNameAttribute("Report", allowHeaderCollapse: true)]
	public class ReportMasterModel : ModelsBase
	{
		private Int64 _MasterReport_Id;
		private string _ReportName;
		private string _DescriptionText;
		private byte[] _Description;
		private int _ReportTypeEnum;
        private int _ReportXMLVersion;
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
		[FieldInformationAttribute("Name", IsRequired = true)]
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

		[FieldInformation("Description")]
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

		[FieldInformation("Report Type", IsReadOnly = true)]
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

		[FieldInformation("Report Version", IsRequired = true)]
		public int ReportXMLVersion
        {
			get
            {
				return this._ReportXMLVersion == 0 ? 1 : this._ReportXMLVersion;
			}

			set
            {
				this._ReportXMLVersion = value;

				base.OnPropertyChanged("ReportXMLVersion", ref this._ReportXMLVersion, value);

			}
        }

		/// <summary>
		/// <para>PaperKind</para>
		/// <para></para>
		/// </summary>
		[FieldInformation("Paper Kind", IsRequired = true)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit:false)]
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
	}
}