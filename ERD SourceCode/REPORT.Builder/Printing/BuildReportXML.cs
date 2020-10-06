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
using System.Security.Policy;
using System.Windows;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Builder.Printing
{
    public class BuildReportXML
    {
        private int sectionIndex = 0;

        private int maxDataSectionIndex;

        private long masterReport_Id;

        private BuildReportRepository repo;

        private DataAccess data;

        public DatabaseModel dataAccessConnection;

        private XElement SelectedSection;

        private Dictionary<int, XElement> dataSections;

        private Dictionary<string, string> whereParameters = new Dictionary<string, string>();

        private Dictionary<int, CanvasSqlManager> indexSqlManager = new Dictionary<int, CanvasSqlManager>();

        private Dictionary<string, string> executingValues = new Dictionary<string, string>();

        public XDocument GetReport(XDocument xmlReport, DatabaseModel connection, ReportMaster reportMaster)
        {
            if (this.repo == null)
            {
                this.repo = new BuildReportRepository();
            }

            this.dataAccessConnection = connection;

            string reportTypeValue = xmlReport.Root.Element("ReportSettings").Attribute("ReportTypeEnum").Value;

            #region BUILD REPORT DATA
            
            ReportTypeEnum reportType = (ReportTypeEnum)reportTypeValue.ToInt32();

            if (reportType == ReportTypeEnum.ReportContent)
            {
                this.OpenDataAccess();

                XElement dataNode = new XElement("ReportData");

                this.dataSections = xmlReport.Root
                    .Descendants("ReportSection")
                    .Where(st => st.Attribute("SectionType").Value == "5") // SectionTypeEnum.TableData
                    .ToDictionary(d => d.Attribute("SectionIndex").Value.ToInt32());

                this.maxDataSectionIndex = this.dataSections.Max(k => k.Key);

                this.SetNextSextion();

                this.BuildReportData(dataNode);

                xmlReport.Root.Add(dataNode);
            }

            #endregion

            #region PAGE OPTIONS

            if (reportMaster.FinalPage_Id.HasValue)
            {
                XDocument finalPage = this.repo.GetReportXml(reportMaster.FinalPage_Id.Value);

                if (finalPage != null)
                {
                    xmlReport.Root.AddFirst(finalPage.Root.Elements());
                }
            }

            if (reportMaster.HeaderAndFooterPage_Id.HasValue)
            {
                XDocument headerAndFooterPage = this.repo.GetReportXml(reportMaster.HeaderAndFooterPage_Id.Value);

                if (headerAndFooterPage != null)
                {
                    xmlReport.Root.AddFirst(headerAndFooterPage.Root.Elements());
                }
            }

            if (reportMaster.CoverPage_Id.HasValue)
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

            return xmlReport;
        }

        public XDocument GetReport(long masterreportId)
        {
            this.masterReport_Id = masterreportId;

            this.repo = new BuildReportRepository();

            XDocument result = this.repo.GetReportXml(this.masterReport_Id);

            ReportMaster reportMaster = this.repo.GetReportMaster(this.masterReport_Id);

            return this.GetReport(result, null, reportMaster);
        }

        private void BuildReportData(XElement dataNode)
        {
            this.ExtractRequiredParameters();

            XDocument data = this.data.ExecuteQuery(this.FormatSQL(this.indexSqlManager[this.sectionIndex].SQLQuery));

            string sectionTableName = this.indexSqlManager[this.sectionIndex].TableName;

            //XElement sectionData = new XElement(sectionTableName);

            foreach (XElement row in data.Root.Elements())
            {
                row.Add(new XAttribute("SectionIndex", this.sectionIndex));

                if (this.SetNextSextion())
                {
                    this.SetExecutingParameters(sectionTableName, row);

                    this.BuildReportData(row);
                
                    this.SetPreviousSextion();
                }
                
                dataNode.Add(row);
            }

            //dataNode.Add(sectionData);
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

        private bool SetNextSextion()
        {
            this.SelectedSection = null;

            bool result = false;

            ++this.sectionIndex;

            while (this.sectionIndex <= this.maxDataSectionIndex)
            {
                if (this.dataSections.ContainsKey(this.sectionIndex))
                {
                    this.SelectedSection = this.dataSections[this.sectionIndex];

                    this.SQLManagerSetup();

                    result = true;

                    break;
                }

                ++this.sectionIndex;
            }

            if (this.sectionIndex > this.maxDataSectionIndex)
            {
                this.sectionIndex = this.maxDataSectionIndex;
            }

            return result;
        }

        private void SetPreviousSextion()
        {
            this.SelectedSection = null;

            --this.sectionIndex;

            while (this.sectionIndex >= 0)
            {
                if (this.dataSections.ContainsKey(this.sectionIndex))
                {
                    this.SelectedSection = this.dataSections[this.sectionIndex];

                    break;
                }

                --this.sectionIndex;
            }
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

            foreach (XElement index in canvasXml.Element("ForeignGroupIndexes").Elements())
            {
                sqlManager.AddForeignGroupIndex(index.Value.ToInt32());
            }

            foreach (XElement item in canvasXml.Element("ParameterModels").Elements())
            {
                paramatersList.Add(new WhereParameterModel { ItemXml = item });
            }

            sqlManager.AddWhereModels(paramatersList.ToArray());

            this.indexSqlManager.Add(this.sectionIndex, sqlManager);
        }
    
        private void OpenDataAccess()
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
