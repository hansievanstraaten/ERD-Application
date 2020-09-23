using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralExtensions;
using ERD.Models;

namespace REPORT.Builder.ReportComponents
{
    public class CanvasSqlManager
    {
        private Dictionary<string, int> columnCountDictionary = new Dictionary<string, int>();

        private Dictionary<string, ReportColumnModel> columnsDictionary = new Dictionary<string, ReportColumnModel>();

        public ColumnObjectModel[] Columns { get; private set; }

        public string TableName { get; private set; }
        
        public void AddColumn(ReportColumnModel column)
        {
            if (!this.TableName.IsNullEmptyOrWhiteSpace() && this.TableName != column.TableName)
            {
                throw new ApplicationException($"Table Column Mismatch. Column {column.ColumnName} does not belong to table {this.TableName}.");
            }
            else if (this.TableName.IsNullEmptyOrWhiteSpace())
            {
                this.TableName = column.TableName;
            }            

            this.AddColumnCount(column.ColumnName);

            if (!this.columnsDictionary.ContainsKey(column.ColumnName))
            {
                this.columnsDictionary.Add(column.ColumnName, column);
            }
        }

        public void RemoveColumn(string columnName)
        {
            if (!this.columnCountDictionary.ContainsKey(columnName))
            {
                return;
            }

            if (this.columnCountDictionary[columnName] == 1)
            {
                this.columnCountDictionary.Remove(columnName);

                this.columnsDictionary.Remove(columnName);

                return;
            }

            int lastCount = this.columnCountDictionary[columnName];

            this.columnCountDictionary.Remove(columnName);

            --lastCount;

            this.columnCountDictionary.Add(columnName, lastCount);
        }

        private void AddColumnCount(string columnName)
        {
            int lastColumnCount = this.columnCountDictionary.ContainsKey(columnName) ? this.columnCountDictionary[columnName] : 0;

            if (lastColumnCount > 0)
            {
                lastColumnCount = this.columnCountDictionary[columnName];

                this.columnCountDictionary.Remove(columnName);
            }

            ++lastColumnCount;

            this.columnCountDictionary.Add(columnName, lastColumnCount);
        }
    }
}
