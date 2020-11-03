using ERD.Models;
using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace REPORT.Builder.ReportComponents
{
    public class CanvasSqlManager
    {
        private Dictionary<string, int> columnCountDictionary;

        private Dictionary<string, ReportColumnModel> columnsDictionary;

        private List<int> foreignSectionIndex;

        private List<WhereParameterModel> whereParameterModel;

        private List<ReportXMLPrintParameterModel> reportFilters = new List<ReportXMLPrintParameterModel>();

        private Dictionary<string, ReportSQLReplaceHeaderModel> replacementColumns = new Dictionary<string, ReportSQLReplaceHeaderModel>();

        private Dictionary<string, ReportsInvokeReplaceModel> invokeMethods = new Dictionary<string, ReportsInvokeReplaceModel>();

        private Dictionary<string, UpdateStatementModel> updateStatements = new Dictionary<string, UpdateStatementModel>();

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
                return ObjectCreator.GetCanvasSQL(
                    this.columnsDictionary.Values.ToArray(), 
                    this.whereParameterModel, 
                    this.reportFilters,
                    this.replacementColumns,
                    this.orderByString);
            }
        }

        internal string SQLUpdates(UpdateStatementModel updateStement, out List<string> columnValues)
		{
            return ObjectCreator.UpdateStatements(updateStement, out columnValues);			
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

        internal List<ReportSQLReplaceHeaderModel> ReplacementColumnModels
		{
            get
			{
                return this.replacementColumns.Values.ToList();
            }
		}

        internal Dictionary<string, ReportsInvokeReplaceModel> InvokeReplacementColumnModels
		{
            get
			{
                return this.invokeMethods;
			}
		}

        internal Dictionary<string, UpdateStatementModel> UpdateStatementModels
        {
            get
            {
                return this.updateStatements;
            }
        }

        internal ReportSQLReplaceHeaderModel GetReplacementColumn(string tableName, string columnName)
		{
            string itemKey = this.BuildItemKey(tableName, columnName);

            if (this.replacementColumns.ContainsKey(itemKey))
			{
                return this.replacementColumns[itemKey];
			}

            return new ReportSQLReplaceHeaderModel
            {
                ReplaceTable = tableName,
                ReplaceColumn = columnName
            };
		}

        internal UpdateStatementModel GetUpdateStatement(string tableName, string columnName)
		{
            string itemKey = this.BuildItemKey(tableName, columnName);

            if (this.updateStatements.ContainsKey(itemKey))
			{
                return this.updateStatements[itemKey];
			}

            return new UpdateStatementModel
            {
                TriggerTable = tableName,
                TriggerColumn = columnName
            };
        }

        internal void UpdateReplacementColumn(ReportSQLReplaceHeaderModel replacementValues)
        {
            string itemKey = this.BuildItemKey(replacementValues.ReplaceTable, replacementValues.ReplaceColumn);

            if (this.replacementColumns.ContainsKey(itemKey))
			{
                this.replacementColumns.Remove(itemKey);
			}

            if (replacementValues.WhereDetails.Count == 0)
			{
                return;
			}

            this.replacementColumns.Add(itemKey, replacementValues);
        }

        internal ReportsInvokeReplaceModel GetInvokeMethod(string tableName, string columnName)
		{
            string itemKey = this.BuildItemKey(tableName, columnName);

            if (this.invokeMethods.ContainsKey(itemKey))
			{
                return this.invokeMethods[itemKey];
			}

            return new ReportsInvokeReplaceModel
            {
                TableName = tableName,
                ColumnName = columnName
            };
        }

        internal void UpdateInvokeReplaceModel(ReportsInvokeReplaceModel invokeModel)
		{
            if (invokeModel == null)
			{
                return;
			}

            string itemKey = this.BuildItemKey(invokeModel.TableName, invokeModel.ColumnName);

            if (this.invokeMethods.ContainsKey(itemKey))
			{
                this.invokeMethods.Remove(itemKey);
			}

            if (invokeModel.SelectDll.IsNullEmptyOrWhiteSpace())
			{
                return;
			}

            this.invokeMethods.Add(itemKey, invokeModel);
        }

        internal void UpdateUpdateStatement(UpdateStatementModel updateStatement)
		{
            if (updateStatement == null)
			{
                return;
			}

            string itemKey = this.BuildItemKey(updateStatement.TriggerTable, updateStatement.TriggerColumn);

            if (this.updateStatements.ContainsKey(itemKey))
            {
                this.updateStatements.Remove(itemKey);
            }

            if (updateStatement.Values.Count == 0
                || updateStatement.WhereValus.Count == 0)
			{
                return;
			}

            this.updateStatements.Add(itemKey, updateStatement);
        }

        internal void AddFilterParameters(List<ReportXMLPrintParameterModel> filters)
		{
            this.reportFilters.AddRange(filters);
		}

        internal void AddOrderByString(string orderBy)
		{
            this.orderByString = orderBy;
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

        private string orderByString { get; set; }

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
    
        private string BuildItemKey(string tableName, string columnName)
		{
            return $"[{tableName}].[{columnName}]";
        }
    
    }
}
