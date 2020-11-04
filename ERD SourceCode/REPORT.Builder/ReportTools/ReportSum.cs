using GeneralExtensions;
using System;
using System.Xml.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Builder.ReportTools
{
	[ModelName("Report Sum")]
	public class ReportSum : LabelBase
	{
		public delegate void DataSectionIndexChangedEvent(object sender, Guid elemntId, int sectionIndex);

		public event DataSectionIndexChangedEvent DataSectionIndexChanged;

		private int dataSectionIndex;
		private string sumColumn;
		private DataItemModel[] sumColumnValues;

		public ReportSum()
		{
			this.DataContext = this;

			this.Text = "Report Sum";
		}

		public new XElement ItemXml
		{
			get
			{
				XElement result = base.ItemXml;


				result.Add(new XAttribute("ObjectType", "ReportSum"));
				result.Add(new XAttribute("ParentSectionGroupIndex", this.ParentSectionGroupIndex));
				result.Add(new XAttribute("SectionGroupIndex", this.SectionGroupIndex));
				result.Add(new XAttribute("SectionIndex", this.SectionIndex));
				result.Add(new XAttribute("SumColumn", this.SumColumn));
				result.Add(new XAttribute("Text", this.Text));

				return result;
			}

			set
			{
				foreach (XAttribute item in value.Attributes())
				{
					this.SetPropertyValue(item.Name.LocalName, item.Value);
				}
			}
		}

		public int ParentSectionGroupIndex
		{
			get;

			set;
		}

		public int SectionGroupIndex
		{
			get; 
			
			set;
		}

		[FieldInformation("Source Data Section", Sort = 500)]
		[ItemType(ModelItemTypeEnum.ComboBox, false)]
		[ValuesSource("DataSections")]
		public int SectionIndex
		{
			get
			{
				return this.dataSectionIndex;
			}

			set
			{
				this.dataSectionIndex = value;

				this.DataSectionIndexChanged?.Invoke(this, this.ElemntId, value);
			}
		}

		[FieldInformation("Sum Column", Sort = 501)]
		[ItemType(ModelItemTypeEnum.ComboBox, false)]
		[ValuesSource("SumColumnValues")]
		public string SumColumn
		{
			get
			{
				return this.sumColumn.IsNullEmptyOrWhiteSpace() ? string.Empty : this.sumColumn;
			}

			set
			{
				this.sumColumn = value;
			}
		}
	
		public DataItemModel[] DataSections
		{
			get;

			set;
		}

		public DataItemModel[] SumColumnValues
		{
			get
			{
				if (this.sumColumnValues.HasElements())
				{
					return this.sumColumnValues;
				}

				return new DataItemModel[] { };
			}

			set
			{
				this.sumColumnValues = value;
			}
		}
	}
}
