using GeneralExtensions;
using System;
using System.Xml.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Data.Models
{
	[ModelName("Method Setup (.Net Framework 4.7.2 or older)")]
	[Serializable()]
	public class ReportsInvokeReplaceModel : ModelsBase
	{
		private string selectedDll;

		private string namespaceValue;

		private string selectedMethod;

		private string assemblyName;

		public ReportsInvokeReplaceModel()
		{
			this.NamespacesValuesList = new DataItemModel[] { };

			this.MethodNamesList = new DataItemModel[] { };
		}

		public XElement ItemXml
		{
			get
			{
				XElement result = new XElement("ReportsInvokeReplace");

				result.Add(new XAttribute("ObjectType", "ReportsInvokeReplaceModel"));
				result.Add(new XAttribute("TableName", this.TableName));
				result.Add(new XAttribute("ColumnName", this.ColumnName));
				result.Add(new XAttribute("AssemblyName", this.AssemblyName));
				result.Add(new XAttribute("SelectDll", this.SelectDll));
				result.Add(new XAttribute("NamespaceValue", this.NamespaceValue));
				result.Add(new XAttribute("SelectedMethod", this.SelectedMethod));

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

		public string TableName { get; set; }

		public string ColumnName { get; set; }

		public string AssemblyName
		{
			get
			{
				return this.assemblyName;
			}

			set
			{
				this.assemblyName = value;

				base.OnPropertyChanged(() => this.AssemblyName);
			}
		}

		[FieldInformation("Static DLL Path", Sort = 1)]
		[BrowseButton("SelectDllKey", "Search", "Search")]
		public string SelectDll 
		{ 
			get
			{
				return this.selectedDll;
			}
			
			set
			{
				this.selectedDll = value;

				base.OnPropertyChanged(() => this.SelectDll);
			}
		}

		[FieldInformation("Namespaces", Sort = 2)]
		[ItemType(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
		[ValuesSourceAttribute("NamespacesValuesList")]
		public string NamespaceValue
		{ 
			get
			{
				return this.namespaceValue;
			}
			
			set
			{
				this.namespaceValue = value;

				base.OnPropertyChanged(() => this.NamespaceValue);
			}
		}

		[FieldInformation("Method", Sort = 3)]
		[ItemType(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
		[ValuesSourceAttribute("MethodNamesList")]
		[BrowseButton("InvokeDllKey", "Invoke", "Execute")]
		public string SelectedMethod
		{
			get
			{
				return this.selectedMethod;
			}

			set
			{
				this.selectedMethod = value;

				base.OnPropertyChanged(() => this.SelectedMethod);
			}
		}

		public DataItemModel[] NamespacesValuesList
		{
			get;

			set;
		}

		public DataItemModel[] MethodNamesList
		{
			get;

			set;
		}
	}
}
