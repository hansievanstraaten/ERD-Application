using ERD.Common;
using GeneralExtensions;
using REPORT.Builder.Constants;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
	/// <summary>
	/// Interaction logic for ReplaceBuilder.xaml
	/// </summary>
	public partial class ReplaceBuilder : UserControlBase
	{
		private List<DataItemModel> whereOptions = new List<DataItemModel>();

		private List<DataItemModel> valueColumns = new List<DataItemModel>();

		private List<ReplaceWhere> replaceWheres = new List<ReplaceWhere>();

		public ReplaceBuilder()
		{
			this.InitializeComponent();

			this.uxFromTable.Items.Add(new DataItemModel { DisplayValue = ReportConstants.None, ItemKey = ReportConstants.None });

			foreach (DataItemModel systemTable in Integrity.GetSystemTables().OrderBy(t => t.DisplayValue))
			{
				this.uxFromTable.Items.Add(systemTable);
			}
		}

		public ReportWhereHeaderModel WhereHeader
		{
			get
			{
				ReportWhereHeaderModel result = new ReportWhereHeaderModel
				{
					ReplaceColumn = this.ReplaceColumn,
					ReplaceTable = this.ReplaceTable,
					UseTable = this.uxFromTable.SelectedValue.ParseToString(),
					UseColumn = this.uxValueFromTable.SelectedValue.ParseToString()
				};

				foreach(ReplaceWhere item in this.replaceWheres)
				{
					if (item.WhereOption == ReportConstants.None)
					{
						continue;
					}

					result.WhereDetails.Add(new ReportWhereDetailModel 
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

				if (value.UseTable.IsNullEmptyOrWhiteSpace() || value.UseTable == ReportConstants.None)
				{
					return;
				}

				this.uxFromTable.SelectedValue = value.UseTable;

				this.uxValueFromTable.SelectedValue = value.UseColumn;

				for(int x = 0; x < value.WhereDetails.Count; ++x)
				{
					ReportWhereDetailModel item = value.WhereDetails[x];

					ReplaceWhere replaceOption = this.replaceWheres.FirstOrDefault(i => i.Index == x);

					replaceOption.WhereOption = item.WhereOption;

					replaceOption.WhereValue = item.WhereValue;
				}
			}
		}

		public void SetValueColumns(List<ReportColumnModel> reportColumns)
		{
			this.valueColumns.Clear();

			DataItemModel[] columns = reportColumns
				.Select(c => new DataItemModel { DisplayValue = $"{c.TableName}.{c.ColumnName}", ItemKey = $"{c.TableName}.{c.ColumnName}" })
				.ToArray();

			this.valueColumns.AddRange(columns);
		}

		public void Clear()
		{
			this.uxFromTable.SelectedValue = ReportConstants.None;
		}

		private void FromTable_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			try
			{
				this.replaceWheres.Clear();

				this.uxValueFromTable.Items.Clear();

				this.uxWhereOptions.Children.Clear();

				this.whereOptions.Clear();

				DataItemModel fromTable = this.uxFromTable.SelectedItem.To<DataItemModel>();

				this.whereOptions.Add(new DataItemModel { DisplayValue = ReportConstants.None, ItemKey = ReportConstants.None });

				foreach (DataItemModel tableColumn in Integrity.GetColumnsForTable(fromTable.DisplayValue).OrderBy(t => t.DisplayValue))
				{
					DataItemModel optionColumn = new DataItemModel
					{
						DisplayValue = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}",
						ItemKey = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}"
					};

					this.uxValueFromTable.Items.Add(optionColumn);

					this.whereOptions.Add(optionColumn);
				}

				if (this.uxFromTable.SelectedValue.ParseToString() != ReportConstants.None)
				{
					this.AddReplaceWhere();
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
					&& selectedValue == ReportConstants.None)
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
					&& selectedValue != ReportConstants.None)
				{
					this.AddReplaceWhere();
				}

			}
			catch (Exception err)
			{
				MessageBox.Show(err.InnerExceptionMessage());
			}
		}

		private void AddReplaceWhere()
		{
			ReplaceWhere replaceOption = new ReplaceWhere { Index = this.uxWhereOptions.Children.Count};

			replaceOption.SetSelectableColumns(this.whereOptions);

			replaceOption.SetValueColumns(this.valueColumns);
				
			replaceOption.WhereValue = this.WhereHeaderKey;

			replaceOption.ColumnSelectionChanged += this.WhereOptionsColumn_Changed;

			this.uxWhereOptions.Children.Add(replaceOption);

			this.replaceWheres.Add(replaceOption);
		}

		private string ReplaceTable { get; set; }

		private string ReplaceColumn { get; set; }

		private string WhereHeaderKey 
		{ 
			get
			{
				return $"{this.ReplaceTable}.{this.ReplaceColumn}";
			}
		}
	}
}
