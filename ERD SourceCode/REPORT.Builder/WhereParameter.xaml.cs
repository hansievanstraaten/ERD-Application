using GeneralExtensions;
using REPORT.Data.Models;
using System;
using System.Net.NetworkInformation;
using System.Windows;
using ViSo.SharedEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for WhereParameter.xaml
    /// </summary>
    public partial class WhereParameter : UserControlBase
    {
        public delegate void SqlOperatorChangedEvent(object sender, int operatorIndex, SqlWhereOperatorsEnum option);

        public delegate void WhereClauseChangedEvent(object sender, int operatorIndex);

        public event SqlOperatorChangedEvent SqlOperatorChanged;

        public event WhereClauseChangedEvent WhereClauseChanged;

        public WhereParameter()
        {
            this.InitializeComponent();

            this.uxAndOd.Items.Add(new DataItemModel
            {
                DisplayValue = SqlWhereOperatorsEnum.None.GetDescriptionAttribute(),
                ItemKey = SqlWhereOperatorsEnum.None
            });

            this.uxAndOd.Items.Add(new DataItemModel
            {
                DisplayValue = SqlWhereOperatorsEnum.AND.GetDescriptionAttribute(),
                ItemKey = SqlWhereOperatorsEnum.AND
            });

            this.uxAndOd.Items.Add(new DataItemModel
            {
                DisplayValue = SqlWhereOperatorsEnum.OR.GetDescriptionAttribute(),
                ItemKey = SqlWhereOperatorsEnum.OR
            });
        }

        public WhereParameter(WhereParameterModel parameter) : this()
        {
            this.WhereClause = parameter;
        }

        public int OperatorIndex { get; set; }

        public WhereParameterModel WhereClause
        {
            get
            {
                DataItemModel andOr = this.uxAndOd.SelectedItem as DataItemModel;

                return new WhereParameterModel
                {
                    AndOrOperator = andOr == null ? SqlWhereOperatorsEnum.None : (SqlWhereOperatorsEnum)andOr.ItemKey,
                    ColumnName = this.uxTableColumn.SelectedValue.ParseToString(),
                    ParameterName = this.uxForeignColumn.SelectedValue.ParseToString(),
                    OperatorIndex = this.OperatorIndex
                };
            }

            private set
            {
                this.uxForeignColumn.SelectionChanged -= this.ForeignColumn_Changed;

                this.uxAndOd.SelectionChanged -= this.AndOr_Changed;

                this.uxTableColumn.SelectedValue = value.ColumnName;

                this.uxForeignColumn.SelectedValue = value.ParameterName;

                this.uxAndOd.SelectedValue = value.AndOrOperator;

                this.OperatorIndex = value.OperatorIndex;

                this.uxForeignColumn.SelectionChanged += this.ForeignColumn_Changed;

                this.uxAndOd.SelectionChanged += this.AndOr_Changed;
            }
        }

        public ReportColumnModel[] TableColumns
        {
            set
            {
                this.uxTableColumn.Items.Clear();

                foreach (ReportColumnModel item in value)
                {
                    DataItemModel columnItem = new DataItemModel
                    {
                        DisplayValue = $"{item.TableName}.{item.ColumnName}",
                        ItemKey = $"{item.TableName}.{item.ColumnName}"
                    };

                    this.uxTableColumn.Items.Add(columnItem);
                }
            }
        }

        public ReportColumnModel[] ForeignColumns
        {
            set
            {
                this.uxForeignColumn.Items.Clear();

                foreach (ReportColumnModel item in value)
                {
                    DataItemModel columnItem = new DataItemModel
                    {
                        DisplayValue = $"{item.TableName}.{item.ColumnName}",
                        ItemKey = $"{item.TableName}.{item.ColumnName}"
                    };

                    this.uxForeignColumn.Items.Add(columnItem);
                }
            }
        }

        private void AndOr_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                DataItemModel selection = this.uxAndOd.SelectedItem as DataItemModel;

                this.SqlOperatorChanged?.Invoke(this, this.OperatorIndex, (SqlWhereOperatorsEnum)selection.ItemKey);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ForeignColumn_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                this.WhereClauseChanged?.Invoke(this, this.OperatorIndex);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}
