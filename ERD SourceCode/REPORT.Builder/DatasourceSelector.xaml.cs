using ERD.Common;
using GeneralExtensions;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for DatasourceSelector.xaml
    /// </summary>
    public partial class DatasourceSelector : UserControlBase
    {
        private DataSourceMasterModel mainTable;

        private List<TreeViewItemTool> treeTableItems = new List<TreeViewItemTool>();

        public DatasourceSelector(long masterReportId)
        {
            this.InitializeComponent();

            this.InitializeMainTable(masterReportId);
        }

        public DataSourceMasterModel MainTable
        {
            get
            {
                return this.mainTable;
            }

            set
            {
                this.mainTable = value;
            }
        }

        public bool Accept()
        {
            try
            {
                if (this.uxMainTable.HasValidationError)
                {
                    return false;
                }

                foreach(TreeViewItemTool item in this.treeTableItems.Where(ch => ch.IsChecked))
                {
                    DataSourceTableModel sourceTable = new DataSourceTableModel
                    {
                        MasterReport_Id = this.MainTable.MasterReport_Id,
                        TableName = item.Header.ParseToString(),
                        IsAvailable = true
                    };

                    this.MainTable.SelectedSourceTables.Add(sourceTable);
                }

                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
        }

        private void InitializeMainTable(long masterReportId)
        {
            List<DataItemModel> systemTables = Integrity.GetSystemTables();

            DataSourceRepository repo = new DataSourceRepository();

            DataSourceMasterModel savedModel = repo.GetDataSourceMasterByPrimaryKey(masterReportId);

            Dictionary<string, DataSourceTableModel> selectedTables = repo
                .GetDataSourceTableByForeignKeyMasterReport_Id(masterReportId)
                .ToDictionary(d => d.TableName);

            this.MainTable = savedModel == null ? new DataSourceMasterModel { MasterReport_Id = masterReportId } : savedModel;

            this.MainTable.SourceTables = systemTables.ToArray();

            this.uxMainTable.Items.Add(this.MainTable);

            this.uxMainTable[0, 0].IsReadOnly = savedModel != null;

            foreach(DataItemModel table in systemTables)
            {
                if (table.DisplayValue == this.MainTable.MainTableName)
                {
                    continue;
                }

                TreeViewItemTool tableItem = new TreeViewItemTool 
                {
                    Header = table.DisplayValue, 
                    IsCheckBox = true,
                    IsChecked = selectedTables.ContainsKey(table.DisplayValue) ? selectedTables[table.DisplayValue].IsAvailable : false
                };

                this.uxOtherTables.Items.Add(tableItem);

                this.treeTableItems.Add(tableItem);
            }
        }
    }
}
