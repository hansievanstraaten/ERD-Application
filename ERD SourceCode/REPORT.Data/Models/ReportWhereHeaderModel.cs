using ERD.DatabaseScripts.Engineering;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.Models
{
	public class ReportWhereHeaderModel : ModelsBase
	{
		private string _ReplaceColumn;
		private string _ReplaceTable;
		private string _UseColumn;
		private string _UseTable;

		public ReportWhereHeaderModel()
		{
			this.WhereDetails = new List<ReportWhereDetailModel>();
		}

		public XElement ItemXml
		{
			get
			{
				XElement result = new XElement("ReportWhereHeader");

				result.Add(new XAttribute("ObjectType", "ReportWhereHeaderModel"));
				result.Add(new XAttribute("ReplaceColumn", this.ReplaceColumn));
				result.Add(new XAttribute("ReplaceTable", this.ReplaceTable));
				result.Add(new XAttribute("UseColumn", this.UseColumn));
				result.Add(new XAttribute("UseTable", this.UseTable));

				XElement whereDetails = new XElement("WhereDetails");

				foreach(ReportWhereDetailModel detail in this.WhereDetails)
				{
					whereDetails.Add(detail.ItemXml);
				}

				result.Add(whereDetails);

				return result;
			}

			set
			{
				foreach (XAttribute element in value.Attributes())
				{
					this.SetPropertyValue(element.Name.LocalName, element.Value);
				}

				foreach(XElement column in value.Descendants("ReportWhereDetail"))
				{
					this.WhereDetails.Add(new ReportWhereDetailModel { ItemXml = column });
				}
			}
		}


		///// <summary>
		///// <para>Master Report ID</para>
		///// <para>Master Report ID</para>
		///// </summary>
		//public Int64 MasterReport_Id
		//{
		//	get
		//	{
		//		return this._MasterReport_Id;
		//	}

		//	set
		//	{
		//		base.OnPropertyChanged("MasterReport_Id", ref this._MasterReport_Id, value);
		//	}
		//}

		/// <summary>
		/// <para>ReplaceColumn</para>
		/// <para></para>
		/// </summary>
		public string ReplaceColumn
		{
			get
			{
				return this._ReplaceColumn;
			}

			set
			{
				base.OnPropertyChanged("ReplaceColumn", ref this._ReplaceColumn, value);
			}
		}

		/// <summary>
		/// <para>ReplaceTable</para>
		/// <para></para>
		/// </summary>
		public string ReplaceTable
		{
			get
			{
				return this._ReplaceTable;
			}

			set
			{
				base.OnPropertyChanged("ReplaceTable", ref this._ReplaceTable, value);
			}
		}

		/// <summary>
		/// <para>Use Column</para>
		/// <para></para>
		/// </summary>
		public string UseColumn
		{
			get
			{
				return this._UseColumn;
			}

			set
			{
				base.OnPropertyChanged("UseColumn", ref this._UseColumn, value);
			}
		}

		/// <summary>
		/// <para>Use Table</para>
		/// <para></para>
		/// </summary>
		public string UseTable
		{
			get
			{
				return this._UseTable;
			}

			set
			{
				base.OnPropertyChanged("UseTable", ref this._UseTable, value);
			}
		}

		public List<ReportWhereDetailModel> WhereDetails;

	}
}