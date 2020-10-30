using ERD.DatabaseScripts.Engineering;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.Models
{
	public class ReportSQLReplaceHeaderModel : ModelsBase
	{
		private string _ReplaceColumn;
		private string _ReplaceTable;
		private List<string> _UseColumn;
		private string _UseTable;

		public ReportSQLReplaceHeaderModel()
		{
			this.UseColumns = new List<string>(0);

			this.WhereDetails = new List<ReportSQLReplaceDetailModel>();
		}

		public XElement ItemXml
		{
			get
			{
				XElement result = new XElement("ReportWhereHeader");

				result.Add(new XAttribute("ObjectType", "ReportWhereHeaderModel"));
				result.Add(new XAttribute("ReplaceColumn", this.ReplaceColumn));
				result.Add(new XAttribute("ReplaceTable", this.ReplaceTable));
				result.Add(new XAttribute("UseTable", this.UseTable));

				XElement selectColumns = new XElement("SelectColumns");
				XElement whereDetails = new XElement("WhereDetails");

				foreach(string column in this.UseColumns)
				{
					if (column == "<None>")
					{
						continue;
					}

					selectColumns.Add(new XElement("ColumnName", column));
				}

				foreach(ReportSQLReplaceDetailModel detail in this.WhereDetails)
				{
					whereDetails.Add(detail.ItemXml);
				}

				result.Add(whereDetails);

				result.Add(selectColumns);

				return result;
			}

			set
			{
				foreach (XAttribute element in value.Attributes())
				{
					this.SetPropertyValue(element.Name.LocalName, element.Value);
				}

				foreach (XElement column in value.Descendants("ColumnName"))
				{
					this.UseColumns.Add(column.Value);
				}

				foreach (XElement column in value.Descendants("ReportWhereDetail"))
				{
					this.WhereDetails.Add(new ReportSQLReplaceDetailModel { ItemXml = column });
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
		public List<string> UseColumns
		{
			get
			{
				return this._UseColumn;
			}

			set
			{
				this._UseColumn = value;
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

		public List<ReportSQLReplaceDetailModel> WhereDetails;

	}
}