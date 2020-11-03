using GeneralExtensions;
using System.Xml.Linq;

namespace REPORT.Data.Models
{
	public class UpdateValueModel
	{
		public XElement ItemXml
		{
			get
			{
				XElement result = new XElement("UpdateValue");

				result.Add(new XAttribute("ObjectType", "UpdateValueModel"));
				result.Add(new XAttribute("ColumnName", this.ColumnName));
				result.Add(new XAttribute("UpdateValue", this.UpdateValue));
				result.Add(new XAttribute("IsDatabaseValue", this.IsDatabaseValue));
				result.Add(new XAttribute("ItemIndex", this.ItemIndex));

				return result;
			}

			set
			{
				foreach (XAttribute element in value.Attributes())
				{
					this.SetPropertyValue(element.Name.LocalName, element.Value);
				}
			}
		}

		public string ColumnName { get; set; }

		public string UpdateValue { get; set; }

		public bool IsDatabaseValue { get; set; }

		public int ItemIndex { get; set; }
	}
}
