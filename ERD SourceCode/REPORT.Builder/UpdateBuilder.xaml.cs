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
    /// Interaction logic for UpdateBuilder.xaml
    /// </summary>
    public partial class UpdateBuilder : UserControlBase
    {
		private UpdateStatementModel updateStatement;

        private List<DataItemModel> valueSelectColumns = new List<DataItemModel>();

        private List<DataItemModel> valueOptionColumns = new List<DataItemModel>();

        private List<ReplaceWhere> selectedValueOptions = new List<ReplaceWhere>();

        private List<ReplaceWhere> selectedWhereOptions = new List<ReplaceWhere>();

        public UpdateBuilder()
        {
            this.InitializeComponent();

            this.uxTableName.Items.Add(new DataItemModel { DisplayValue = ReportConstants.None, ItemKey = ReportConstants.None });

            foreach (DataItemModel systemTable in Integrity.GetSystemTables().OrderBy(t => t.DisplayValue))
            {
                this.uxTableName.Items.Add(systemTable);
            }
        }

        public UpdateStatementModel UpdateStatement
		{
            get
			{
                this.updateStatement.UpdateTableName = this.uxTableName.SelectedValue.ParseToString();

                this.updateStatement.Values.Clear();

                this.updateStatement.WhereValus.Clear();

                this.updateStatement.UpdateTableName = this.uxTableName.SelectedValue.ParseToString();

                int itemIndex = 0;

                foreach (ReplaceWhere item in this.selectedValueOptions)
				{
                    if (item.WhereOption == ReportConstants.None)
					{
                        continue;
					}

                    this.updateStatement.Values.Add(new UpdateValueModel 
                    { 
                         ColumnName = item.WhereOption,
                         UpdateValue = item.WhereValue,
                         IsDatabaseValue = this.valueOptionColumns.Any(a => a.DisplayValue == item.WhereValue),
                         ItemIndex = itemIndex
                    });

                    ++itemIndex;
				}

                itemIndex = 0;

                foreach (ReplaceWhere item in this.selectedWhereOptions)
                {
                    if (item.WhereOption == ReportConstants.None)
                    {
                        continue;
                    }

                    this.updateStatement.WhereValus.Add(new UpdateValueModel
                    {
                        ColumnName = item.WhereOption,
                        UpdateValue = item.WhereValue,
                        IsDatabaseValue = this.valueOptionColumns.Any(a => a.DisplayValue == item.WhereValue),
                        ItemIndex = itemIndex
                    });

                    ++itemIndex;
                }

                return this.updateStatement;
			}

            set
			{
                this.updateStatement = value;

                this.uxTableName.SelectedValue = value.UpdateTableName;

                foreach(UpdateValueModel item in value.Values)
				{
                    ReplaceWhere valueItem = this.selectedValueOptions.FirstOrDefault(i => i.Index == item.ItemIndex);

                    valueItem.WhereOption = item.ColumnName;
                        
                    valueItem.WhereValue = item.UpdateValue;
                }

                foreach (UpdateValueModel item in value.WhereValus)
                {
                    ReplaceWhere whereItem = this.selectedWhereOptions.FirstOrDefault(i => i.Index == item.ItemIndex);

                    whereItem.WhereOption = item.ColumnName;

                    whereItem.WhereValue = item.UpdateValue;
                }
            }
		}

        public void Clear()
		{
            this.uxValues.Children.Clear();

            this.uxWhere.Children.Clear();

            this.selectedValueOptions.Clear();

            this.selectedWhereOptions.Clear();

            this.uxValues.Children.Clear();

            this.uxWhere.Children.Clear();
        }

        public void SetValueColumns(List<ReportColumnModel> reportColumns)
        {
            this.valueOptionColumns.Clear();

            DataItemModel[] columns = reportColumns
                .Select(c => new DataItemModel { DisplayValue = $"{c.TableName}.{c.ColumnName}", ItemKey = $"{c.TableName}.{c.ColumnName}" })
                .ToArray();

            this.valueOptionColumns.AddRange(columns);
        }

        private void TableName_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
            try
			{
                this.Clear();

                if (this.uxTableName.SelectedValue.ParseToString() == ReportConstants.None)
				{
                    return;
				}

                this.SetColumnOptions();

                this.AddVauesItem();

                this.AddWhereItem();

			}
            catch (Exception err)
			{
                MessageBox.Show(err.InnerExceptionMessage());
			}
		}

        private void ValueOptionsColumn_Changed(object sender, int index, string selectedValue)
        {
            try
            {
                if (index != (this.uxValues.Children.Count - 1)
                    && selectedValue == ReportConstants.None)
                {
                    int endCount = this.uxValues.Children.Count;

                    for (int x = (index + 1); x < endCount; ++x)
                    {
                        ReplaceWhere removeItem = this.selectedValueOptions.FirstOrDefault(i => i.Index == x);

                        this.uxValues.Children.Remove(removeItem);

                        this.selectedValueOptions.Remove(removeItem);
                    }

                    return;
                }
                else if (index == (this.uxValues.Children.Count - 1)
                    && selectedValue != ReportConstants.None)
                {
                    this.AddVauesItem();
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
                if (index != (this.uxWhere.Children.Count - 1)
                    && selectedValue == ReportConstants.None)
                {
                    int endCount = this.uxWhere.Children.Count;

                    for (int x = (index + 1); x < endCount; ++x)
                    {
                        ReplaceWhere removeItem = this.selectedWhereOptions.FirstOrDefault(i => i.Index == x);

                        this.uxWhere.Children.Remove(removeItem);

                        this.selectedWhereOptions.Remove(removeItem);
                    }

                    return;
                }
                else if (index == (this.uxWhere.Children.Count - 1)
                    && selectedValue != ReportConstants.None)
                {
                    this.AddWhereItem();
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SetColumnOptions()
		{
            this.valueSelectColumns.Clear();

            DataItemModel fromTable = this.uxTableName.SelectedItem.To<DataItemModel>();

            if (fromTable == null)
			{
                return;
			}

            this.valueSelectColumns.Add(new DataItemModel { DisplayValue = ReportConstants.None, ItemKey = ReportConstants.None });

            foreach (DataItemModel tableColumn in Integrity.GetColumnsForTable(fromTable.DisplayValue).OrderBy(t => t.DisplayValue))
            {
                DataItemModel optionColumn = new DataItemModel
                {
                    DisplayValue = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}",
                    ItemKey = $"{fromTable.DisplayValue}.{tableColumn.DisplayValue}"
                };

                this.valueSelectColumns.Add(optionColumn);
            }
        }

        private void AddVauesItem()
		{
            ReplaceWhere valueItem = new ReplaceWhere { Index = this.uxValues.Children.Count };

            valueItem.SetSelectableColumns(this.valueSelectColumns);

            valueItem.SetValueColumns(this.valueOptionColumns);

            valueItem.ColumnSelectionChanged += this.ValueOptionsColumn_Changed;

            this.uxValues.Children.Add(valueItem);

            this.selectedValueOptions.Add(valueItem);
        }		
	
        private void AddWhereItem()
		{
            ReplaceWhere whereItem = new ReplaceWhere { Index = this.uxWhere.Children.Count };

            whereItem.SetSelectableColumns(this.valueSelectColumns);

            whereItem.SetValueColumns(this.valueOptionColumns);

            whereItem.ColumnSelectionChanged += this.WhereOptionsColumn_Changed;

            this.uxWhere.Children.Add(whereItem);

            this.selectedWhereOptions.Add(whereItem);
        }
	}
}
