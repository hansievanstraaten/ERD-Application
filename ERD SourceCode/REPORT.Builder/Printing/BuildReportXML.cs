using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Builder.ReportComponents;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Builder.Printing
{
    public class BuildReportXML
    {
        private int commandTimeout = 300;

        private int sectionIndex = 0;

        private int tableRowIndex = 0;

        private long masterReport_Id;

        private BuildReportRepository repo;

        private DataAccess data;

        public DatabaseModel dataAccessConnection;

        private XElement SelectedSection;

        private Dictionary<int, XElement> dataSections;

        private Dictionary<string, string> whereParameters = new Dictionary<string, string>();

        private Dictionary<int, CanvasSqlManager> indexSqlManager = new Dictionary<int, CanvasSqlManager>();

        private Dictionary<string, string> executingValues = new Dictionary<string, string>();

        private Dictionary<string, string> orderByDictionary = new Dictionary<string, string>();

        private Dictionary<string, List<ReportXMLPrintParameterModel>> tableFilters;

        private Dictionary<string, object> invokeInstances = new Dictionary<string, object>();

        public XDocument GetReport(
          XDocument xmlReport, 
          DatabaseModel connection, 
          ReportMaster reportMaster, 
          List<ReportXMLPrintParameterModel> filters)
        {
            if (filters.Count > 0 && filters.Any(p => p.IsRequired.IsTrue() && p.FilterValue.IsNullEmptyOrWhiteSpace()))
			{
                throw new ApplicationException("One or more required filters was not passed with a value. Report Print canceled.");
			}

            if (this.repo == null)
            {
                this.repo = new BuildReportRepository();
            }

            if (filters.Count == 0)
            {
                this.tableFilters = new Dictionary<string, List<ReportXMLPrintParameterModel>>();
            }
            else
            {
                this.tableFilters = filters
                    .GroupBy(g => g.TableName)
                    .ToDictionary(t => t.Key, c => c.ToList());
            }

            this.dataAccessConnection = connection;

            Dictionary<string, XElement[]> sqlOrderBy = xmlReport
                .Root
                .Descendants("ReportObject")
                .Where(ro => ro.IsDataObject() && ro.Attribute("UseInOrderBy").Value == "true")
                .GroupBy(tn => tn.Attribute("ObjectTable").Value)
                .ToDictionary(td => td.Key, td => td.ToArray());

            Dictionary<string, string> orderByDictionary = new Dictionary<string, string>();

            foreach(KeyValuePair<string, XElement[]> orderKey in sqlOrderBy)
			{
                string[] orderString = orderKey.Value
                    .Select(x => x.Attribute("ObjectColumn").Value)
                    .ToArray();

                this.orderByDictionary.Add(orderKey.Key, orderString.Concatenate(","));
            }

            string reportTypeValue = xmlReport.Root.Element("ReportSettings").Attribute("ReportTypeEnum").Value;

            #region BUILD REPORT DATA

            ReportTypeEnum reportType = (ReportTypeEnum)reportTypeValue.ToInt32();

            if (reportType == ReportTypeEnum.ReportContent)
            {
                this.CreateConnectionObject();

                XElement dataNode = new XElement("ReportData");

                this.dataSections = xmlReport.Root
                    .Descendants("ReportSection")
                    .Where(st => st.Attribute("SectionType").Value == "5") // SectionTypeEnum.TableData
                    .ToDictionary(d => d.Attribute("SectionIndex").Value.ToInt32());

                this.sectionIndex = 1;

                this.DoSectionSetup();

                this.BuildReportData(dataNode);

                xmlReport.Root.Add(dataNode);
            }

            #endregion

            foreach(XElement reportSum in xmlReport.Root.Descendants().Where(a => a.Attribute("ObjectType") != null
                                                                               && a.Attribute("ObjectType").Value == "ReportSum"))
            {
                int parentSetionGroupIndex = reportSum.Attribute("ParentSectionGroupIndex").Value.ToInt32();

                int sectionGroupIndex = reportSum.Attribute("SectionGroupIndex").Value.ToInt32();

                string sumColumn = reportSum.Attribute("SumColumn").Value;

                if (sumColumn.IsNullEmptyOrWhiteSpace())
				{   // The user did not perfom a full setup
                    continue;
				}

                foreach(XElement tableRow in xmlReport.Root
                    .Element("ReportData")
                    .Descendants()
                    .Where(pg => pg.Attribute("SectionGroupIndex") != null
                                 && pg.Attribute("SectionGroupIndex").Value.ToInt32() == parentSetionGroupIndex))
                {
					IEnumerable<XElement> sumItems = parentSetionGroupIndex == sectionGroupIndex ?
                        tableRow
                        .Elements()
                        .Elements()
                        .Where(ln => ln.Name.LocalName == sumColumn)
                        :
                        tableRow
                        .Descendants()
                        .Where(ln => ln.Name.LocalName == sumColumn
                                  && ln.Parent.Parent.Attribute("SectionGroupIndex") != null
                                  && ln.Parent.Parent.Attribute("SectionGroupIndex").Value.ToInt32() == sectionGroupIndex);

					decimal result = 0;

                    if (sumItems.Count() != 0)
                    {
                        result = sumItems.Sum(si =>
                        {
                            decimal? innerResult = si.Value.TryToDecimal().Value;

                            return innerResult.HasValue ? innerResult.Value : 0;
                        });
                    }

                    XElement sumElemet = new XElement(sumColumn);

                    sumElemet.Add(new XAttribute("Value", result));

                    sumElemet.Add(new XAttribute("TableRowIndex", tableRow.Attribute("TableRowIndex").Value));

                    reportSum.Add(sumElemet);
                }
			}

            #region PAGE OPTIONS

            if (reportMaster.FinalPage_Id.HasValue && reportMaster.FinalPage_Id.Value > 0)
            {
                XDocument finalPage = this.repo.GetReportXml(reportMaster.FinalPage_Id.Value);

                if (finalPage != null)
                {
                    xmlReport.Root.AddFirst(finalPage.Root.Elements());
                }
            }

            if (reportMaster.HeaderAndFooterPage_Id.HasValue && reportMaster.HeaderAndFooterPage_Id.Value > 0)
            {
                XDocument headerAndFooterPage = this.repo.GetReportXml(reportMaster.HeaderAndFooterPage_Id.Value);

                if (headerAndFooterPage != null)
                {
                    xmlReport.Root.AddFirst(headerAndFooterPage.Root.Elements());
                }
            }

            if (reportMaster.CoverPage_Id.HasValue && reportMaster.CoverPage_Id.Value > 0)
            {
                XDocument coverPage = this.repo.GetReportXml(reportMaster.CoverPage_Id.Value);

                if (coverPage != null)
                {
                    xmlReport.Root.AddFirst(coverPage.Root.Elements());
                }
            }

            #endregion

            #region PAGE SETUP

            XElement pageSetup = new XElement("PageSetup");

            pageSetup.Add(new XAttribute("PaperKindEnum", reportMaster.PaperKindEnum));
            pageSetup.Add(new XAttribute("PageOrientationEnum", reportMaster.PageOrientationEnum));
            pageSetup.Add(new XAttribute("PageMarginBottom", reportMaster.PageMarginBottom));
            pageSetup.Add(new XAttribute("PageMarginLeft", reportMaster.PageMarginLeft));
            pageSetup.Add(new XAttribute("PageMarginRight", reportMaster.PageMarginRight));
            pageSetup.Add(new XAttribute("PageMarginTop", reportMaster.PageMarginTop));

            xmlReport.Root.AddFirst(pageSetup);

            #endregion

            this.repo.UpdateXmlPrinteCount(reportMaster.MasterReport_Id);

            this.RunUpdateStements(xmlReport);

            return xmlReport;
        }

        public XDocument GetReport(long masterreportId, List<ReportXMLPrintParameterModel> filters)
        {
            this.masterReport_Id = masterreportId;

            this.repo = new BuildReportRepository();

            XDocument result = this.repo.GetReportXml(this.masterReport_Id);

            ReportMaster reportMaster = this.repo.GetReportMaster(this.masterReport_Id);

            return this.GetReport(result, null, reportMaster, filters);
        }

        private void BuildReportData(XElement dataNode)
        {
            this.ExtractRequiredParameters();

            string sectionTableName = this.indexSqlManager[this.sectionIndex].TableName;

			this.indexSqlManager[this.sectionIndex]
				.AddFilterParameters((this.tableFilters.ContainsKey(sectionTableName) ?
					this.tableFilters[sectionTableName] :
					new List<ReportXMLPrintParameterModel>()));

            if (this.orderByDictionary.ContainsKey(sectionTableName))
			{
                this.indexSqlManager[this.sectionIndex]
                    .AddOrderByString(this.orderByDictionary[sectionTableName]);
			}

			XDocument data = this.data.ExecuteQuery(this.FormatSQL(this.indexSqlManager[this.sectionIndex].SQLQuery), this.commandTimeout);

            XElement sectionData = new XElement(sectionTableName);

            sectionData.Add(new XAttribute("SectionGroupIndex", this.SelectedSection.GetSectionGroupIndex()));

            sectionData.Add(new XAttribute("TableRowIndex", this.tableRowIndex));

            ++this.tableRowIndex;

            foreach (XElement row in data.Root.Elements())
            {
                row.Add(new XAttribute("SectionIndex", this.sectionIndex));

                List<int> foreignSections = this.SelectedSection.GetSectionForeignSectionIndexes();

                foreignSections.Sort();

                int holdSectionIndex = this.sectionIndex;

                foreach (int foreignIndex in foreignSections)
                {
                    this.sectionIndex = foreignIndex;

                    this.DoSectionSetup();

                    this.SetExecutingParameters(sectionTableName, row);

                    this.BuildReportData(row);
                }

                this.sectionIndex = holdSectionIndex;

                this.DoSectionSetup();

                this.SetInvokedValues(sectionTableName, row);

                sectionData.Add(row);
            }

            dataNode.Add(sectionData);
        }

        public string FormatSQL(string sql)
        {
            foreach (KeyValuePair<string, string> value in this.executingValues)
            {
                sql = sql.Replace(value.Key, $"'{value.Value}'");
            }

            return sql;
        }

        private void SetExecutingParameters(string tableName, XElement row)
        {
            foreach (XElement column in row.Elements())
            {
                string parameterKey = $"{tableName}.{column.Name.LocalName}";

                List<KeyValuePair<string, string>> parameter = this.whereParameters.Where(v => v.Value == parameterKey).ToList();

                foreach (KeyValuePair<string, string> parameterkeyPair in parameter)
                {
                    string columnSeek = parameterkeyPair.Value.Remove(0, (tableName.Length + 1));

                    string parameterName = $"@{parameterkeyPair.Value}";

                    if (this.executingValues.ContainsKey(parameterName))
                    {
                        this.executingValues.Remove(parameterName);
                    }

                    XElement seekElement = row.Element($"{columnSeek}_Value") == null ?
                        row.Element(columnSeek)
                        :
                        row.Element($"{columnSeek}_Value");

                    this.executingValues.Add(parameterName, (seekElement.Attribute("Value") == null ?
                        seekElement.Value
                        :
                        seekElement.Attribute("Value").Value));
                }
            }
        }

        private void ExtractRequiredParameters()
        {
            List<XElement> expectedWheres = this.dataSections.Where(k => k.Key > this.sectionIndex)
                .Select(e => e.Value)
                .Descendants("WhereParameter")
                .Where(d => d.Attribute("ParameterName").Value.StartsWith(this.indexSqlManager[this.sectionIndex].TableName))
                    .ToList();

            foreach (XElement item in expectedWheres)
            {
                string columnName = item.Attribute("ColumnName").Value;

                if (this.whereParameters.ContainsKey(columnName))
                {
                    continue;
                }

                this.whereParameters.Add(columnName, item.Attribute("ParameterName").Value);
            }
        }

        private void DoSectionSetup()
        {
            this.SelectedSection = this.dataSections[this.sectionIndex];

            this.SQLManagerSetup();
        }

        private void SQLManagerSetup()
        {
            if (this.SelectedSection == null)
            {
                return;
            }

            if (this.indexSqlManager.ContainsKey(this.sectionIndex))
            {
                return;
            }

            CanvasSqlManager sqlManager = new CanvasSqlManager();

            XElement canvasXml = this.SelectedSection.Element("CanvasXml");

            foreach (XElement item in canvasXml.Element("ObjectModels").Elements())
            {
                if (item.Name.LocalName == "ReportObject" && item.Attribute("ObjectType").Value == "ReportDataObject")
                {
                    UIElement child = ObjectCreator.CreateReportObject(item, false);

                    sqlManager.AddColumn(child.GetPropertyValue("ColumnModel") as ReportColumnModel);
                }
            }

            List<WhereParameterModel> paramatersList = new List<WhereParameterModel>();

            foreach (XElement index in canvasXml.Element("ForeignSectionIndexes").Elements())
            {
                sqlManager.AddForeignSectionIndex(index.Value.ToInt32());
            }

            foreach (XElement item in canvasXml.Element("ParameterModels").Elements())
            {
                paramatersList.Add(new WhereParameterModel { ItemXml = item });
            }

            sqlManager.AddWhereModels(paramatersList.ToArray());

            foreach (XElement item in canvasXml.Element("ReplacementColumns").Elements())
            {
                sqlManager.UpdateReplacementColumn(new ReportSQLReplaceHeaderModel { ItemXml = item });
            }

            foreach (XElement item in canvasXml.Element("InvokeMethods").Elements())
            {
                sqlManager.UpdateInvokeReplaceModel(new ReportsInvokeReplaceModel { ItemXml = item });
            }

            foreach (XElement item in canvasXml.Element("UpdateStatements").Elements())
            {
                sqlManager.UpdateUpdateStatement(new UpdateStatementModel { ItemXml = item });
            }

            this.indexSqlManager.Add(this.sectionIndex, sqlManager);
        }

        private void CreateConnectionObject()
        {
            if (this.dataAccessConnection == null)
            {
                ReportConnection connection = this.repo.GetConnection(this.masterReport_Id);

                this.dataAccessConnection = connection.CopyToObject(new DatabaseModel()) as DatabaseModel;
            }

            this.data = new DataAccess(this.dataAccessConnection);
        }
    
        private void SetInvokedValues(string tableName, XElement row)
		{
            foreach(XElement column in row.Elements())
			{
                string columnKey = $"[{tableName}].[{column.Name.LocalName}]";

                if (!this.indexSqlManager[this.sectionIndex].InvokeReplacementColumnModels.ContainsKey(columnKey))
				{
                    continue;
				}

                column.Add(new XAttribute("Value", column.Value));

                column.Value = InvokeReplacement(columnKey, column.Value);
            }
		}

        private void RunUpdateStements(XDocument xmlReport)
		{
            foreach (CanvasSqlManager item in this.indexSqlManager.Values)
            {
                foreach (KeyValuePair<string, UpdateStatementModel> statement in item.UpdateStatementModels)
                {
                    List<string> updateColumns = null;

                    string rawUpdateSql = item.SQLUpdates(statement.Value, out updateColumns);

                    IEnumerable<XElement> values = xmlReport.Root.Descendants(statement.Value.TriggerTable);

                    foreach (XElement row in values.Elements())
                    {
                        XElement valueElement = row.Element($"{statement.Value.TriggerColumn}_Value");

                        if (valueElement == null)
                        {   // Use the Value Attribute
                            valueElement = row.Element($"{statement.Value.TriggerColumn}");
                        }

                        StringBuilder updateSql = new StringBuilder();

                        updateSql.Append(rawUpdateSql);

                        foreach (string column in updateColumns)
                        {
                            XElement columnItem = row.Element(column);

                            updateSql.Replace($"[[{column}]]", columnItem.Value);
                        }

                        this.data.ExecuteNonQuery(updateSql.ToString());
                    }
                }
            }
        }

        private string InvokeReplacement(string columnKey, string columnValue)
		{
			object instance = null;
                
            ReportsInvokeReplaceModel invokeModel = this.indexSqlManager[this.sectionIndex].InvokeReplacementColumnModels[columnKey];

			if (!this.invokeInstances.ContainsKey(columnKey))
			{
                Assembly asm = Assembly.LoadFile(invokeModel.SelectDll);

				Type executingType = asm.GetType(invokeModel.NamespaceValue);

				instance = Activator.CreateInstance(executingType);

				this.invokeInstances.Add(columnKey, instance);
			}
			else
			{
				instance = this.invokeInstances[columnKey];
			}

			string output = instance.InvokeMethod(
				instance,
                invokeModel.SelectedMethod,
				new object[] { columnValue }).ParseToString();

			return output;
		}
    }
}
