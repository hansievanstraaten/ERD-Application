using ERD.Models;
using ERD.Viewer.Database;
using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Builder.ReportComponents;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Builder.Printing
{
	public class BuildReportXML
    {
        private int sectionIndex = 0;
        
        private long masterReport_Id;

        private BuildReportRepository repo;

        private DataAccess data;

        public DatabaseModel dataAccessConnection;

        private XElement SelectedSection;

        private Dictionary<int, XElement> dataSections;

        private Dictionary<string, string> whereParameters = new Dictionary<string, string>();

        private Dictionary<int, CanvasSqlManager> indexSqlManager = new Dictionary<int, CanvasSqlManager>();

        private Dictionary<string, string> executingValues = new Dictionary<string, string>();
                
        private Dictionary<string, List<ReportXMLPrintParameterModel>> tableFilters;

        public XDocument GetReport(XDocument xmlReport, DatabaseModel connection, ReportMaster reportMaster, List<ReportXMLPrintParameterModel> filtes)
        {
            if (this.repo == null)
            {
                this.repo = new BuildReportRepository();
            }

            if (filtes.Count == 0)
			{
                this.tableFilters = new Dictionary<string, List<ReportXMLPrintParameterModel>>();
			}
            else
			{
                this.tableFilters = filtes
                    .GroupBy(g => g.TableName)
                    .ToDictionary(t => t.Key, c => c.ToList());
			}

            this.dataAccessConnection = connection;

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

            return xmlReport;
        }

        public XDocument GetReport(long masterreportId)
        {
            this.masterReport_Id = masterreportId;

            this.repo = new BuildReportRepository();

            XDocument result = this.repo.GetReportXml(this.masterReport_Id);

            ReportMaster reportMaster = this.repo.GetReportMaster(this.masterReport_Id);

            List<ReportXMLPrintParameterModel> filtes = this.repo.GetPrintparameters(this.masterReport_Id, this.repo.GetReportXMLVersion(this.masterReport_Id));

            return this.GetReport(result, null, reportMaster, filtes);
        }

        private void BuildReportData(XElement dataNode)
        {
            this.ExtractRequiredParameters();

            string sectionTableName = this.indexSqlManager[this.sectionIndex].TableName;

            this.indexSqlManager[this.sectionIndex]
                .AddFilterParameters((this.tableFilters.ContainsKey(sectionTableName) ? 
                    this.tableFilters[sectionTableName] : 
                    new List<ReportXMLPrintParameterModel>()));

            XDocument data = this.data.ExecuteQuery(this.FormatSQL(this.indexSqlManager[this.sectionIndex].SQLQuery));

            XElement sectionData = new XElement(sectionTableName);
                
            sectionData.Add(new XAttribute("SectionGroupIndex", this.SelectedSection.GetSectionGroupIndex()));
                
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

                sectionData.Add(row);
            }

            dataNode.Add(sectionData);
        }

        public string FormatSQL(string sql)
        {
            foreach(KeyValuePair<string, string> value in this.executingValues)
            {
                sql = sql.Replace(value.Key, $"'{value.Value}'");
            }

            return sql;
        }

        private void SetExecutingParameters(string tableName, XElement row)
        {
            foreach(XElement column in row.Elements())
            {
                string parameterKey = $"{tableName}.{column.Name.LocalName}";

                List<KeyValuePair<string, string>> parameter = this.whereParameters.Where(v => v.Value == parameterKey).ToList();

                foreach(KeyValuePair<string, string> parameterkeyPair in parameter)
                {
                    string columnSeek = parameterkeyPair.Value.Remove(0, (tableName.Length + 1));

                    string parameterName = $"@{parameterkeyPair.Value}";

                    if (this.executingValues.ContainsKey(parameterName))
                    {
                        this.executingValues.Remove(parameterName);
                    }

                    this.executingValues.Add(parameterName, row.Element(columnSeek).Value);
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

            foreach(XElement item in expectedWheres)
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
    }
}
