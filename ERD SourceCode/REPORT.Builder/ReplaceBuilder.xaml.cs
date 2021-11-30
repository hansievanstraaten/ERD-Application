using ERD.Common;
using GeneralExtensions;
using Microsoft.Win32;
using REPORT.Data.Common;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ViSo.Dialogs.Input;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Exstention;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for ReplaceBuilder.xaml
	/// </summary>
	public partial class ReplaceBuilder : UserControlBase
	{
		private readonly string fromTable = "From Table";

		private readonly string fromDll = "From dll";

		#region FROM SQL FIELDS

		private List<DataItemModel> whereOptions = new List<DataItemModel>();

		private List<DataItemModel> valueColumns = new List<DataItemModel>();

		private List<ReplaceWhere> replaceWheres = new List<ReplaceWhere>();

		#endregion

		#region INVOKE METHOD SECTION FIELDS

		private ReportsInvokeReplaceModel invokeMethodSetup;

		private Dictionary<string, Type> tyesDictionary;

		#endregion

		public ReplaceBuilder()
		{
			this.InitializeComponent();

			this.uxFromTable.Items.Add(new DataItemModel { DisplayValue = Constants.None, ItemKey = Constants.None });

			this.uxOptionType.Items.Add(new DataItemModel { DisplayValue = this.fromTable, ItemKey = this.fromTable });

			this.uxOptionType.Items.Add(new DataItemModel { DisplayValue = this.fromDll, ItemKey = this.fromDll });

			foreach (DataItemModel systemTable in Integrity.GetSystemTables().OrderBy(t => t.DisplayValue))
			{
				this.uxFromTable.Items.Add(systemTable);
			}

			this.uxOptionType.SelectedValue = this.fromTable;
		}

		public void RefreshCaptionWidth()
		{
			try
			{
				if (this.Visibility != Visibility.Visible)
				{
					return;
				}

				this.InvalidateVisual();

				this.UpdateLayout();

				UIElement[] lables = this.FindVisualControls(typeof(LableItem));

				if (lables.Length == 0)
				{
					return;
				}

				double maxWidth = lables
					.Where(s => !s.GetPropertyValue("Content").ParseToString().StartsWith("Method Setup ("))
					.Max<UIElement>(m => m.GetPropertyValue("TextLenght").ToDouble()) + 10;

				foreach (UIElement element in lables)
				{
					if (element.GetPropertyValue("Content").ParseToString().StartsWith("Method Setup ("))
					{
						continue;
					}

					element.SetPropertyValue("Width", maxWidth);
				}

			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		public void Clear()
		{
			this.uxFromTable.SelectedValue = Constants.None;

			if (this.InvokeMethodSetup != null)
			{
				this.InvokeMethodSetup.SelectDll.IsNullEmptyOrWhiteSpace();
			}
		}

		public ReportSQLReplaceHeaderModel WhereHeader
		{
			get
			{
				ReportSQLReplaceHeaderModel result = new ReportSQLReplaceHeaderModel
				{
					ReplaceColumn = this.ReplaceColumn,
					ReplaceTable = this.ReplaceTable,
					UseTable = this.uxFromTable.SelectedValue.ParseToString()
				};

				foreach(ComboBoxTool boxTool in this.uxSelectColumnsStack.Children)
				{
					result.UseColumns.Add(boxTool.SelectedValue.ParseToString());
				}

				foreach (ReplaceWhere item in this.replaceWheres)
				{
					if (item.WhereOption == Constants.None)
					{
						continue;
					}

					result.WhereDetails.Add(new ReportSQLReplaceDetailModel
					{
						WhereOption = item.WhereOption,
						WhereValue = item.WhereValue,
						IsColumn = item.IsValueFromColumnObject
					});
				}

				return result;
			}

			set
			{
				this.ReplaceTable = value.ReplaceTable;

				this.ReplaceColumn = value.ReplaceColumn;

				if (value.UseTable.IsNullEmptyOrWhiteSpace() || value.UseTable == Constants.None)
				{
					return;
				}

				this.uxFromTable.SelectedValue = value.UseTable;

				for (int x = 0; x < value.UseColumns.Count; ++x)
				{
					this.AddSelectFromColumnOption(value.UseColumns[x], x);
				}

				for (int x = 0; x < value.WhereDetails.Count; ++x)
				{
					ReportSQLReplaceDetailModel item = value.WhereDetails[x];

					ReplaceWhere replaceOption = this.replaceWheres.FirstOrDefault(i => i.Index == x);

					replaceOption.WhereOption = item.WhereOption;

					replaceOption.WhereValue = item.WhereValue;
				}
			}
		}

		public ReportsInvokeReplaceModel InvokeMethodSetup
		{
			get
			{
				return this.invokeMethodSetup;
			}

			set
			{
				if (this.tyesDictionary != null)
				{
					this.tyesDictionary.Clear();
				}

				if (this.invokeMethodSetup != null)
				{
					this.invokeMethodSetup.PropertyChanged -= this.InvokeMethodSetup_Changed;
				}

				this.invokeMethodSetup = value;

				this.uxInvoke.Items.Clear();

				this.uxInvoke.Items.Add(this.invokeMethodSetup);

				if (value != null)
				{
					this.uxInvoke[0, 1].SetComboBoxItems(
						new DataItemModel[] { new DataItemModel { DisplayValue = value.NamespaceValue, ItemKey = value.NamespaceValue } });

					this.uxInvoke[0, 2].SetComboBoxItems(
						new DataItemModel[] { new DataItemModel { DisplayValue = value.SelectedMethod, ItemKey = value.SelectedMethod } });

					this.uxInvoke[0, 1].SetValue(value.NamespaceValue);

					this.uxInvoke[0, 2].SetValue(value.SelectedMethod);
				}

				this.invokeMethodSetup.PropertyChanged += this.InvokeMethodSetup_Changed;

				this.uxOptionType.SelectedValue = value == null || value.SelectDll.IsNullEmptyOrWhiteSpace() ?
					this.fromTable
					:
					this.fromDll;
			}
		}
		
		#region FROM SQL METHODS

		public void SetValueColumns(List<ReportColumnModel> reportColumns)
		{
			this.valueColumns.Clear();

			DataItemModel[] columns = reportColumns
				.Select(c => new DataItemModel { DisplayValue = $"{c.TableName}.{c.ColumnName}", ItemKey = $"{c.TableName}.{c.ColumnName}" })
				.ToArray();

			this.valueColumns.AddRange(columns);
		}

		#endregion

		private void OptionType_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			try
			{
				this.uxSqlReplace.Visibility = this.uxOptionType.SelectedValue.ParseToString() == this.fromDll ? Visibility.Collapsed : Visibility.Visible;

				this.uxInvokeReplace.Visibility = this.uxOptionType.SelectedValue.ParseToString() == this.fromDll ? Visibility.Visible : Visibility.Collapsed;

				this.RefreshCaptionWidth();
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		#region FROM SQL SECTION EVENTS

		private void FromTable_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			try
			{
				this.replaceWheres.Clear();

				this.uxSelectColumnsStack.Children.Clear();
				
				this.uxWhereOptions.Children.Clear();

				this.whereOptions.Clear();

				this.AddSelectFromColumnOption(string.Empty, 0);

				DataItemModel fromTable = this.uxFromTable.SelectedItem.To<DataItemModel>();

				this.whereOptions.Add(new DataItemModel { DisplayValue = Constants.None, ItemKey = Constants.None });

				foreach (DataItemModel tableColumn in Integrity.GetColumnsForTable(fromTable.DisplayValue).OrderBy(t => t.DisplayValue))
				{
					DataItemModel optionColumn = new DataItemModel
					{
						DisplayValue = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}",
						ItemKey = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}"
					};

					this.whereOptions.Add(optionColumn);
				}

				if (this.uxFromTable.SelectedValue.ParseToString() != Constants.None)
				{
					this.AddReplaceWhere();
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void SelectColumnChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (this.uxSelectColumnsStack.Children[this.uxSelectColumnsStack.Children.Count - 1]
					.To<ComboBoxTool>().SelectedValue.ParseToString() != Constants.None)
				{
					this.AddSelectFromColumnOption(Constants.None, this.uxSelectColumnsStack.Children.Count);
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void WhereOptionsColumn_Changed(object sender, int index, string selectedValue)
		{
			try
			{
				if (index != (this.uxWhereOptions.Children.Count - 1)
					&& selectedValue == Constants.None)
				{
					int endCount = this.uxWhereOptions.Children.Count;

					for (int x = (index + 1); x < endCount; ++x)
					{
						ReplaceWhere removeItem = this.replaceWheres.FirstOrDefault(i => i.Index == x);

						this.uxWhereOptions.Children.Remove(removeItem);

						this.replaceWheres.Remove(removeItem);
					}

					return;
				}
				else if (index == (this.uxWhereOptions.Children.Count - 1)
					&& selectedValue != Constants.None)
				{
					this.AddReplaceWhere();
				}

			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		#endregion

		#region INVOKE METHOD SECTION EVENTS

		private void InvokeModel_Browse(object sender, string buttonKey)
		{
			try
			{
				switch(buttonKey)
				{
					case "SelectDllKey":

						this.ShowDllSelect();

						break;

					case "InvokeDllKey":

						this.TestMethodInvoke();

						break;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void InvokeMethodSetup_Changed(object sender, PropertyChangedEventArgs e)
		{
			try
			{
				switch(e.PropertyName)
				{
					case "SelectDll":

						this.LoadNamespaceClasses();

						break;

					case "NamespaceValue":

						this.LoadTypeMethods();

						break;
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		#endregion

		#region FROM SQL METHODS

		private void AddReplaceWhere()
		{
			ReplaceWhere replaceOption = new ReplaceWhere { Index = this.uxWhereOptions.Children.Count };

			replaceOption.SetSelectableColumns(this.whereOptions);

			replaceOption.SetValueColumns(this.valueColumns);

			replaceOption.WhereValue = this.WhereHeaderKey;

			replaceOption.ColumnSelectionChanged += this.WhereOptionsColumn_Changed;

			this.uxWhereOptions.Children.Add(replaceOption);

			this.replaceWheres.Add(replaceOption);
		}

		private void AddSelectFromColumnOption(string selectedValue, int desiredIndex)
		{
			DataItemModel fromTable = this.uxFromTable.SelectedItem.To<DataItemModel>();

			bool haveChild = desiredIndex < this.uxSelectColumnsStack.Children.Count;

			ComboBoxTool resultBox = haveChild ?
				this.uxSelectColumnsStack.Children[desiredIndex].To<ComboBoxTool>()
				:
				new ComboBoxTool();

			if (!haveChild)
			{
				resultBox.Items.Add(new DataItemModel { DisplayValue = Constants.None, ItemKey = Constants.None });

				foreach (DataItemModel tableColumn in Integrity.GetColumnsForTable(fromTable.DisplayValue).OrderBy(t => t.DisplayValue))
				{
					DataItemModel optionColumn = new DataItemModel
					{
						DisplayValue = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}",
						ItemKey = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}"
					};

					resultBox.Items.Add(optionColumn);
				}
				resultBox.SelectionChanged += this.SelectColumnChanged;

				this.uxSelectColumnsStack.Children.Add(resultBox);
			}

			resultBox.SelectedValue = selectedValue;
		}
		
		#endregion

		#region INVOKE METHODS

		private void ShowDllSelect()
		{
			OpenFileDialog dlg = new OpenFileDialog();

			dlg.Filter = "DLL Files | *.dll";

			if (dlg.ShowDialog().IsFalse())
			{
				return;
			}

			this.InvokeMethodSetup.SelectDll = dlg.FileName;
		}

		private void LoadNamespaceClasses()
		{
			if (this.InvokeMethodSetup.SelectDll.IsNullEmptyOrWhiteSpace())
			{
				this.uxInvoke[0, 1].SetComboBoxItems(new DataItemModel[] { });

				this.uxInvoke[0, 2].SetComboBoxItems(new DataItemModel[] { });

				this.InvokeMethodSetup.NamespaceValue = string.Empty;

				this.InvokeMethodSetup.SelectedMethod = string.Empty;

				return;
			}

			Assembly asm = Assembly.LoadFile(this.InvokeMethodSetup.SelectDll);

			this.InvokeMethodSetup.AssemblyName = asm.FullName;

			this.tyesDictionary = asm
				.GetTypes()
				.ToDictionary(t => t.FullName, t => t);

			this.uxInvoke[0, 1].SetComboBoxItems(tyesDictionary
				.Values
				.Select(a => new DataItemModel { DisplayValue = a.FullName, ItemKey = a.FullName })
				.ToArray());
		}

		private void LoadTypeMethods()
		{
			if (this.InvokeMethodSetup.NamespaceValue.IsNullEmptyOrWhiteSpace())
			{
				return;
			}

			Type classType = this.tyesDictionary[this.InvokeMethodSetup.NamespaceValue];

			List<DataItemModel> methodList = new List<DataItemModel>();

			foreach(MethodInfo method in classType.GetMethods())
			{
				methodList.Add(new DataItemModel { DisplayValue = method.Name, ItemKey = method.Name });
			}

			this.uxInvoke[0, 2].SetComboBoxItems(methodList.ToArray());
		}

		private void TestMethodInvoke()
		{
			if (this.InvokeMethodSetup.SelectedMethod.IsNullEmptyOrWhiteSpace())
			{
				MessageBox.Show("Please select a method to execute");

				return;
			}

			if (InputBox.ShowDialog("Invoke Parameter", "Parameter Value").IsFalse())
			{
				return;
			}

			Type executingType = null;

			if (this.tyesDictionary.ContainsKey(this.InvokeMethodSetup.NamespaceValue))
			{
				executingType = this.tyesDictionary[this.InvokeMethodSetup.NamespaceValue];
			}
			else
			{
				Assembly asm = Assembly.LoadFile(this.InvokeMethodSetup.SelectDll);

				executingType = asm.GetType(this.InvokeMethodSetup.NamespaceValue);
			}

			object instance = Activator.CreateInstance(executingType);

			string output = instance.InvokeMethod(
				instance, 
				this.InvokeMethodSetup.SelectedMethod, 
				new object[] { InputBox.Result }).ParseToString();

			MessageBox.Show($"Your method returned: {output}");
		}

		#endregion

		#region FROM SQL PROPERTIES

		private string ReplaceTable { get; set; }

		private string ReplaceColumn { get; set; }

		private string WhereHeaderKey
		{
			get
			{
				return $"{this.ReplaceTable}.{this.ReplaceColumn}";
			}
		}

		#endregion		
	}
}
