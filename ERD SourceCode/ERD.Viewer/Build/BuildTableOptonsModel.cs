using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace ERD.Viewer.Build
{
	[ModelName("Tables")]
	public class BuildTableOptonsModel : ModelsBase
	{
		private string tableName;

		[FieldInformation("Table Selected for Sample Scripting", Sort = 0)]
		[ItemType(ModelItemTypeEnum.ComboBox, isComboboxEdit: false)]
		[ValuesSource("TablesSource")]
		public string TableName 
		{ 
			get
			{
				return this.tableName;
			}
			
			set
			{
				this.tableName = value;

				base.OnPropertyChanged("TableName");
			}
		}

		public DataItemModel[] TablesSource
		{
			get;

			set;
		}
	}
}
