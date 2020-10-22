using GeneralExtensions;
using System;
using System.Xml.Linq;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.Models
{
	public class ReportWhereDetailModel : ModelsBase
	{
		private string _WhereOption;
		private string _WhereValue;
		private bool _IsColumn;

		public XElement ItemXml
		{
			get
			{
				XElement result = new XElement("ReportWhereDetail");

				result.Add(new XAttribute("ObjectType", "ReportWhereDetailModel"));
				result.Add(new XAttribute("WhereOption", this.WhereOption));
				result.Add(new XAttribute("WhereValue", this.WhereValue));
				result.Add(new XAttribute("IsColumn", this.IsColumn));

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


		/// <summary>
		/// <para>WhereOption</para>
		/// <para></para>
		/// </summary>
		public string WhereOption
		{
			get
			{
				return this._WhereOption;
			}

			set
			{
				base.OnPropertyChanged("WhereOption", ref this._WhereOption, value);
			}
		}

		/// <summary>
		/// <para>Where Value</para>
		/// <para></para>
		/// </summary>
		public string WhereValue
		{
			get
			{
				return this._WhereValue;
			}

			set
			{
				base.OnPropertyChanged("WhereValue", ref this._WhereValue, value);
			}
		}

		/// <summary>
		/// <para>Is Derived From Column</para>
		/// <para></para>
		/// </summary>
		public bool IsColumn
		{
			get
			{
				return this._IsColumn;
			}

			set
			{
				base.OnPropertyChanged("IsColumn", ref this._IsColumn, value);
			}
		}

	}
}