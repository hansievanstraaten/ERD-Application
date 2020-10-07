using GeneralExtensions;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.DataContext;
using System;
using System.Linq;
using System.Xml.Linq;

namespace REPORT.Data.SQLRepository.Repositories
{

    public class BuildReportRepository
    {
        private ReportsBuildContext dataContext;

        public BuildReportRepository()
        {
            this.dataContext = new ReportsBuildContext();
        }

        public void UpdateXmlPrinteCount(long masterReport_Id)
        {
            ReportXML exists = this.dataContext
                .ReportsXML
                .FirstOrDefault(e => e.MasterReport_Id == masterReport_Id);

            if (exists == null)
            {
                return;
            }

            int reportXMLVersion = this.dataContext
            .ReportsXML
            .Where(r => r.MasterReport_Id == masterReport_Id)
            .Max(m => m.ReportXMLVersion);

            ReportXML report = this.dataContext
                .ReportsXML
                .FirstOrDefault(xml => xml.MasterReport_Id == masterReport_Id
                       && xml.ReportXMLVersion == reportXMLVersion);

            ++report.PrintCount;

            this.dataContext.SaveChanges();
        }

        public XDocument GetReportXml(long masterReport_Id)
        {
            int reportXMLVersion = this.dataContext
                .ReportsXML
                .Where(r => r.MasterReport_Id == masterReport_Id)
                .Max(m => m.ReportXMLVersion);

            ReportXML report = this.dataContext
                .ReportsXML
                .FirstOrDefault(xml => xml.MasterReport_Id == masterReport_Id
                       && xml.ReportXMLVersion == reportXMLVersion);

            if (report == null)
            {
                return null;
            }

            string rawXml = report.BinaryXML.UnzipFile().ParseToString();

            return XDocument.Parse(rawXml);
        }    
    
        public ReportMaster GetReportMaster(long masterReport_Id)
        {
            return this.dataContext
                .ReportsMaster
                .FirstOrDefault(m => m.MasterReport_Id == masterReport_Id);
        }

        public ReportConnection GetConnection(long masterReportId)
        {
            return this.dataContext
                .ReportConnections
                .FirstOrDefault(con => con.MasterReport_Id == masterReportId
                                    && con.IsActive
                                    && con.IsProductionConnection);
        }
    }
}
