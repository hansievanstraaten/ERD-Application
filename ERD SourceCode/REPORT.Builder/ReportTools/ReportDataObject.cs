using GeneralExtensions;
using REPORT.Data.Models;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
	[ModelName("Data Object")]
	public class ReportDataObject : LabelBase
	{
		private ReportColumnModel columnModel;

		private bool suppressed;
		private bool useAsPrintParameter;
		private string printParameterCaption;
		private bool useInOrderBy;
		private string printParameterDefaultValue;
		private bool suppressIfNoData;
		private bool isRequiredParameter;

		public ReportDataObject()
		{
			//this.ColumnModel = new ReportColumnModel();

			this.DataContext = this;

			base.Loaded += this.ReportLabel_Loaded;
		}

		public new XElement ItemXml
		{
			get
			{
				XElement result = base.ItemXml;

				result.Add(new XAttribute("ObjectType", "ReportDataObject"));
				result.Add(new XAttribute("ObjectTable", this.ColumnModel.TableName));
				result.Add(new XAttribute("ObjectColumn", this.ColumnModel.ColumnName));
				result.Add(new XAttribute("UseAsPrintParameter", this.UseAsPrintParameter));
				result.Add(new XAttribute("IsRequiredParameter", this.IsRequiredParameter));
				result.Add(new XAttribute("PrintParameterCaption", this.PrintParameterCaption));
				result.Add(new XAttribute("PrintParameterDefaultValue", this.PrintParameterDefaultValue));
				result.Add(new XAttribute("UseInOrderBy", this.UseInOrderBy));
				result.Add(new XAttribute("Suppressed", this.Suppressed));
				result.Add(new XAttribute("SuppressIfNoData", this.SuppressIfNoData));
				result.Add(new XAttribute("ColumnModel", this.ColumnModel.ZipFile().ConvertBytesToString()));

				return result;
			}

			set
			{
				foreach (XAttribute item in value.Attributes())
				{
					if (item.Name.LocalName == "ColumnModel")
					{
						this.ColumnModel = item.Value.ConvertStringToBytes().UnzipFile() as ReportColumnModel;

						continue;
					}

					this.SetPropertyValue(item.Name.LocalName, item.Value);
				}
			}
		}

		[FieldInformation("Use as Print Parameter", Sort = 500)]
		public bool UseAsPrintParameter
		{
			get
			{
				return this.useAsPrintParameter;
			}

			set
			{
				this.useAsPrintParameter = value;
			}
		}

		[FieldInformation("Is Parameter Required", Sort = 501)]
		public bool IsRequiredParameter
		{
			get
			{
				return this.isRequiredParameter;
			}

			set
			{
				this.isRequiredParameter = value;
			}
		}

		[FieldInformation("Print Parameter Caption", Sort = 502)]
		public string PrintParameterCaption
		{
			get
			{
				return this.printParameterCaption == null ? string.Empty : this.printParameterCaption;
			}

			set
			{
				this.printParameterCaption = value;
			}
		}

		[FieldInformation("Print Parameter Default", Sort = 503)]
		public string PrintParameterDefaultValue
		{
			get
			{
				return this.printParameterDefaultValue.ParseToString();
			}

			set
			{
				this.printParameterDefaultValue = value;
			}
		}

		[FieldInformation("Use In Order By", Sort = 504)]
		public bool UseInOrderBy
		{
			get
			{
				return this.useInOrderBy;
			}

			set
			{
				this.useInOrderBy = value;
			}
		}

		[FieldInformation("Is Suppressed", Sort = 505)]
		public bool Suppressed
		{
			get
			{
				return this.suppressed;
			}

			set
			{
				this.suppressed = value;

				if (this.IsDesignMode && value)
				{
					this.Foreground = Brushes.Gray;
				}
				else if (value)
				{
					this.Visibility = Visibility.Hidden;
				}
			}
		}

		[FieldInformation("Suppress if No Data", Sort = 506)]
		public bool SuppressIfNoData
		{
			get
			{
				return this.suppressIfNoData;
			}

			set
			{
				this.suppressIfNoData = value;
			}
		}

		public ReportColumnModel ColumnModel
		{
			get
			{
				return this.columnModel == null ? new ReportColumnModel() : this.columnModel;
			}

			set
			{
				this.columnModel = value;

				base.Text = value.ColumnName;
			}
		}

		private void ReportLabel_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.IsDesignMode)
			{
				this.Background = Brushes.WhiteSmoke;
			}
			else
			{
				this.Background = Brushes.Transparent;
			}
		}
	}
}
