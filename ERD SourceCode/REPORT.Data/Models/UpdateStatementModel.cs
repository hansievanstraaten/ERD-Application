using GeneralExtensions;
using System.Collections.Generic;
using System.Xml.Linq;

namespace REPORT.Data.Models
{
	public class UpdateStatementModel
	{
		public UpdateStatementModel()
		{
			this.Values = new List<UpdateValueModel>();

			this.WhereValus = new List<UpdateValueModel>();
		}

		public XElement ItemXml
		{
			get
			{
				XElement result = new XElement("UpdateStatement");

				result.Add(new XAttribute("ObjectType", "UpdateStatementModel"));
				result.Add(new XAttribute("TriggerTable", this.TriggerTable));
				result.Add(new XAttribute("TriggerColumn", this.TriggerColumn));
				result.Add(new XAttribute("UpdateTableName", this.UpdateTableName));

				XElement values = new XElement("Values");
				XElement whereValus = new XElement("WhereValus");

				foreach (UpdateValueModel value in this.Values)
				{
					values.Add(value.ItemXml);
				}

				foreach (UpdateValueModel value in this.WhereValus)
				{
					whereValus.Add(value.ItemXml);
				}

				result.Add(values);

				result.Add(whereValus);

				return result;
			}

			set
			{
				foreach (XAttribute element in value.Attributes())
				{
					this.SetPropertyValue(element.Name.LocalName, element.Value);
				}

				foreach(XElement item in value.Element("Values").Elements())
				{
					this.Values.Add(new UpdateValueModel { ItemXml = item });
				}

				foreach (XElement item in value.Element("WhereValus").Elements())
				{
					this.WhereValus.Add(new UpdateValueModel { ItemXml = item });
				}
			}
		}

		public string TriggerTable { get; set; }

		public string TriggerColumn { get; set; }

		public string UpdateTableName { get; set; }

		public List<UpdateValueModel> Values { get; set; }

		public List<UpdateValueModel> WhereValus { get; set; }
	}
}
