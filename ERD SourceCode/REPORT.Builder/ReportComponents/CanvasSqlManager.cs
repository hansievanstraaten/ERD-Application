using ERD.Models;
using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace REPORT.Builder.ReportComponents
{
    public class CanvasSqlManager
    {
        private Dictionary<string, int> columnCountDictionary = new Dictionary<string, int>();

        private Dictionary<string, ReportColumnModel> columnsDictionary = new Dictionary<string, ReportColumnModel>();

        //public ColumnObjectModel[] Columns { get; private set; }

        public string SQLQuery
        {
            get
            {
                return ObjectCreator.GetCanvasSQL(this.columnsDictionary.Values.ToArray());
            }
        }

        public string TableName { get; private set; }
        
        public List<ReportColumnModel> ReportColumns
        {
            get
            {
                return this.columnsDictionary.Values.ToList();
            }
        }

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
