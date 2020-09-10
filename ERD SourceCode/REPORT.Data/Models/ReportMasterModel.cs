using GeneralExtensions;
using System;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.SQLRepository.Agrigates
{
	public class ReportMasterModel : ModelsBase
	{
		private Int64 _MasterReport_Id;
		private string _ReportName;
		private byte[] _Description;
		private int _ReportTypeEnum;

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
	}
}