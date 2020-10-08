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
        private Dictionary<string, int> columnCountDictionary;

        private Dictionary<string, ReportColumnModel> columnsDictionary;

        private List<WhereParameterModel> whereParameterModel;

        private List<int> foreignSectionIndex;

        internal CanvasSqlManager()
        {
            this.columnCountDictionary = new Dictionary<string, int>();

            this.columnsDictionary = new Dictionary<string, ReportColumnModel>();

            this.whereParameterModel = new List<WhereParameterModel>();

            this.foreignSectionIndex = new List<int>();
        }

        internal string SQLQuery
        {
            get
            {
                return ObjectCreator.GetCanvasSQL(this.columnsDictionary.Values.ToArray(), this.whereParameterModel);
            }
        }

        internal string TableName { get; private set; }

        internal List<int> ForeignSectionIndexes
        {
            get
            {
                return this.foreignSectionIndex;
            }
        }

        internal List<ReportColumnModel> ReportColumns
        {
            get
            {
                return this.columnsDictionary.Values.ToList();
            }
        }

        internal List<WhereParameterModel> WhereParameterModel
        {
            get
            {
                return this.whereParameterModel;
            }
        }

        internal bool HaveForeignSectionIndex(int index)
        {
            return this.foreignSectionIndex.Contains(index);
        }

        internal void Reset()
        {
            this.columnCountDictionary.Clear();

            this.columnsDictionary.Clear();

            this.whereParameterModel.Clear();

            this.foreignSectionIndex.Clear();
        }

        internal void AddForeignSectionIndex(int index)
        {
            if (this.foreignSectionIndex.Contains(index))
            {
                return;
            }

            this.foreignSectionIndex.Add(index);
        }

        internal void RemoveForeignSectionIndex(int index)
        {
            if (!this.foreignSectionIndex.Contains(index))
            {
                return;
            }

            this.foreignSectionIndex.Remove(index);
        }

        internal void AddColumn(ReportColumnModel column)
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

        internal void RemoveColumn(string columnName)
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

        internal void AddWhereModels(WhereParameterModel[] whereModels)
        {
            this.whereParameterModel.Clear();

            this.whereParameterModel.AddRange(whereModels);
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
