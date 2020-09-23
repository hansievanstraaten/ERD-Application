using System;
using System.Collections.Generic;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Data.Models
{
	[ModelNameAttribute("Main Table")]
	[Serializable]
	public class DataSourceMasterModel : ModelsBase
	{
		private Int64 _MasterReport_Id;
		private string _MainTableName;

		public DataSourceMasterModel()
        {
			this.SelectedSourceTables = new List<DataSourceTableModel>();
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
		/// <para>Main Table Name</para>
		/// <para></para>
		/// </summary>
		[FieldInformation("Main Table", IsRequired = true, Sort = 1)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("SourceTables")]
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

		public DataItemModel[] SourceTables { get; set; }

		public List<DataSourceTableModel> SelectedSourceTables { get; set; }
	}
}