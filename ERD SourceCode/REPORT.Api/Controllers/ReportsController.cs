using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using Microsoft.AspNetCore.Mvc;
using REPORT.Builder.Printing;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportTablesRepository reportsRepository;
        private readonly IWebHostEnvironment environment;

        public ReportsController(ReportTablesRepository reportTablesRepository, IConfiguration config, IWebHostEnvironment environment)
        {
            this.reportsRepository = reportTablesRepository;
            this.environment = environment;

            General.ProjectModel = new ERD.Models.ProjectModel 
            {
                ModelName = config["ProjectName"]
            };                
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            List<ReportCategoryModel> categories = this.reportsRepository.GetActiveCategories();
            return Ok(categories);
        }

        [HttpGet]
        public IActionResult GetReportsByCategory(long categoryId)
        {
            List<ReportMasterHeaderModel> reports = this.reportsRepository.GetReportMasterByReportTypeEnumHeader((int)ReportTypeEnum.ReportContent, categoryId);
            return Ok(reports);
        }

        [HttpGet]
        public IActionResult GetReportParameters(long masterReport_Id)
        {
            ReportTablesRepository repo = new ReportTablesRepository();

            ReportXMLModel reportXmlModel = repo.GetReportXMLByPrimaryKey(masterReport_Id);

            string reportXml = reportXmlModel.BinaryXML.UnzipFile().ParseToString();

            XDocument report = XDocument.Parse(reportXml);

            List<ReportXMLPrintParameterModel> parameterFilters = this.GetReportparameterFilters(report, reportXmlModel.ReportXMLVersion, masterReport_Id);

            return base.Ok(parameterFilters);
        }

        [HttpPost]
        public IActionResult GetReportById(long masterReport_Id, List<ReportXMLPrintParameterModel> parameterFilters)
        {
            string tempKey = Guid.NewGuid().ToString();
            string tempFilename = $"{tempKey}.pdf";
            string tempPath = Path.Combine(this.environment.ContentRootPath, "Temp", tempKey);
            string fullFilePath = Path.Combine(tempPath, tempFilename);

            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                byte[] pdfBytes = RunOnStaThread(() =>
                {
                    BuildReportXML xmlBuild = new BuildReportXML();
                    XDocument reportDocumentXml = xmlBuild.GetReport(masterReport_Id, parameterFilters);
                    BuildReportToCanvas reportPrint = new BuildReportToCanvas();
                    
                    reportPrint.PrintDocument(reportDocumentXml);

                    CanvasToPDF pdf = new CanvasToPDF();
                    string endFilename = pdf.ConvertToPdf(fullFilePath, reportPrint.Pages);
                    
                    return System.IO.File.ReadAllBytes(endFilename);
                });

                ReportMasterModel master = this.reportsRepository.GetReportMaster(masterReport_Id);

                return File(pdfBytes, "application/pdf", $"{master.ReportName}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
            }
        }

        private List<ReportXMLPrintParameterModel> GetReportparameterFilters(XDocument reportXml, int version, long masterReport_Id)
        {
            List<XElement> filterElements = reportXml
                    .Root
                    .Descendants("ReportObject")
                    .Where(r => r.IsDataObject() && r.Attribute("UseAsPrintParameter").Value == "true")
                    .ToList();

            List<ReportXMLPrintParameterModel> resut = new List<ReportXMLPrintParameterModel>();

            foreach (XElement item in filterElements)
            {
                resut.Add(new ReportXMLPrintParameterModel
                {
                    TableName = item.Attribute("ObjectTable").Value,
                    ColumnName = item.Attribute("ObjectColumn").Value,
                    ReportXMLVersion = version,
                    MasterReport_Id = masterReport_Id,
                    FilterCaption = item.Attribute("PrintParameterCaption").Value,
                    DefaultValue = item.Attribute("PrintParameterDefaultValue").Value,
                    IsActive = true,
                    IsRequired = item.Attribute("IsRequiredParameter").Value.TryToBool()
                });
            }

            return resut;
        }

        private static T RunOnStaThread<T>(Func<T> action)
        {
            T result = default!;
            Exception? error = null;

            var thread = new Thread(() =>
            {
                var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

                dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        result = action();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        dispatcher.BeginInvokeShutdown(
                            System.Windows.Threading.DispatcherPriority.Background);
                    }
                });

                System.Windows.Threading.Dispatcher.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (error != null)
                System.Runtime.ExceptionServices.ExceptionDispatchInfo
                    .Capture(error).Throw();

            return result;
        }
    }
}
