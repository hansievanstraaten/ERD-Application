using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Builder.ReportComponents;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Builder.Printing
{
    public class BuildReportToCanvas
    {
        private int pageMarginTop;

        private int pageMarginBottom;

        private double pageHeaderHeight;

        private double pageFooterHeight;

        private PaperKind paperKind;

        private PageOrientationEnum pageOrientation;

        private PageMediaSize pageSize;

        private PrintCanvas activeCanvas;

        private XElement pageHeader;

        private XElement pageFooter;

        private XElement reportData;

        private Dictionary<int, XElement> dataSections;

        private Dictionary<int, PrintCanvas> canvasesDictionary = new Dictionary<int, PrintCanvas>();

        public Dictionary<int, PrintCanvas> Pages
        {
            get
            {
                return this.canvasesDictionary;
            }
        }

        public void PrintDocument(XDocument report)
        {
            #region GATHER PAGE SETUP INFORMATION

            XElement pageSetup = report.Root.Element("PageSetup");

            this.paperKind = (PaperKind)pageSetup.Attribute("PaperKindEnum").Value.ToInt32();

            this.pageOrientation = (PageOrientationEnum)pageSetup.Attribute("PageOrientationEnum").Value.ToInt32();

            this.pageSize = PageSetupOptions.GetPageMediaSize(this.paperKind);

            this.pageMarginTop = pageSetup.Attribute("PageMarginTop").Value.ToInt32();

            this.pageMarginBottom = pageSetup.Attribute("PageMarginBottom").Value.ToInt32();

            XElement coverPage = report.GetReportSettings(ReportTypeEnum.CoverPage);

            if (coverPage != null)
            {
                SectionTypeEnum sectionType =  SectionTypeEnum.None;

                XElement reportSection = coverPage.GetReportSection(0, out sectionType);

                this.CreateSinglePageCanvas(reportSection.GetCanvasXml(), sectionType);
            }

            XElement headersAndFooters = report.GetReportSettings(ReportTypeEnum.PageHeaderAndFooter);

            if (headersAndFooters != null)
            {
                SectionTypeEnum sectionType = SectionTypeEnum.None;

                this.pageHeader = headersAndFooters.GetReportSection(0, out sectionType);

                this.pageFooter = headersAndFooters.GetReportSection(1, out sectionType);
            }

            #endregion

            #region START BUILD DATA REPORT

            this.dataSections = report.GetDataReportSections();

            if (this.dataSections.Count > 0)
            {
                this.CreateDataPageCanvas();

                this.reportData = report.Root.Element("ReportData");

                if (this.dataSections.Count == 3)
                {
                    this.SetSectionHeader(0);
                }

                foreach (XElement table in this.reportData.Elements())
                {
                    foreach (XElement row in table.Elements())
                    {
                        if (this.dataSections.Count > 3)
                        {
                            this.SetSectionHeader(0);
                        }

                        this.BuildReportData(row);

                        if (this.dataSections.Count != 3)
                        {
                            this.SetSectionFooter(0);
                        }
                    }

                    if (this.dataSections.Count == 3)
                    {
                        this.SetSectionFooter(0);
                    }
                }
            }

            if (this.activeCanvas != null && this.activeCanvas.HaveElements)
            {
                this.CompleteDataPageCanvas();
            }

            #endregion

            #region FINALIZE PAGES

            XElement finalPage = report.GetReportSettings(ReportTypeEnum.FinalPage);

            if (finalPage != null)
            {
                SectionTypeEnum sectionType = SectionTypeEnum.None;

                XElement reportSection = finalPage.GetReportSection(0, out sectionType);

                this.CreateSinglePageCanvas(reportSection.GetCanvasXml(), sectionType);
            }

            #endregion
        }

        private double PageHeaderHeight
        {
            get
            {
                if (this.pageHeader == null)
                {
                    return 0;
                }

                if (this.pageHeaderHeight == 0)
                {
                    this.pageHeaderHeight = this.pageHeader.Attribute("CanvasHeight").Value.ToDouble();
                } 
                
                return this.pageHeaderHeight;
            }
        }

        private double PageFooterHeight
        {
            get
            {
                if (this.pageFooter == null)
                {
                    return 0;
                }

                if (this.pageFooterHeight == 0)
                {
                    this.pageFooterHeight = this.pageFooter.Attribute("CanvasHeight").Value.ToDouble();
                }

                return this.pageFooterHeight;
            }
        }

        private double PageWidth
        {
            get
            {
                return this.pageOrientation == PageOrientationEnum.Portrait ? this.pageSize.Width.Value : this.pageSize.Height.Value;
            }
        }

        private double PageHeight
        {
            get
            {
                return this.pageOrientation == PageOrientationEnum.Portrait ? this.pageSize.Height.Value : this.pageSize.Width.Value;
            }
        }

        private void CreateSinglePageCanvas(XElement canvasXml, SectionTypeEnum sectionType)
        {
            PrintCanvas result = new PrintCanvas 
            { 
                Width =  this.PageWidth, 
                Height = this.PageHeight
            };

            foreach(XElement item in canvasXml.GetReportObjects())
            {
                double bottom = 0;

                result.AddObject(item, sectionType, out bottom);
            }

            this.AddCanvas(result);
        }

        private void CreateDataPageCanvas()
        {
            this.activeCanvas = new PrintCanvas
            {
                Width = this.PageWidth,
                Height = this.PageHeight,
                HeaderHeight = this.PageHeaderHeight,
                FooterHeight = this.PageFooterHeight,
                TopMargin = this.pageMarginTop,
                BottomMargin = this.pageMarginBottom,
                Margin = new Thickness(5),
                TopOffset = this.pageMarginTop
            };

            if (this.pageHeader != null)
            {
                XElement headerCanvas = this.pageHeader.GetCanvasXml();

                foreach(XElement item in headerCanvas.GetReportObjects())
                {
                    this.activeCanvas.AddPageHeaderObject(item);
                }
            }
        }

        private void CompleteDataPageCanvas()
        {
            if (this.pageFooter != null)
            {
                XElement footerCanvas = this.pageFooter.GetCanvasXml();

                foreach (XElement item in footerCanvas.GetReportObjects())
                {
                    this.activeCanvas.AddPageFooterObject(item);
                }
            }

            this.AddCanvas(this.activeCanvas);

            this.activeCanvas = null;
        }

        private void AddCanvas(PrintCanvas canvas)
        {
            int key = this.canvasesDictionary.Count;

            this.canvasesDictionary.Add(key, canvas);
        }

        #region BUILD REPORT DATA SECTION

        private void SetSectionHeader(int groupIndex)
        {
            XElement sectionHeader = this.GetReportSection(SectionTypeEnum.TableHeader, groupIndex);

            if (sectionHeader.Attribute("CanvasHeight").Value.ToDecimal() <= 0)
			{
                return;
			}

            this.AddObjectModels(sectionHeader, null);
        }

        private void SetSectionFooter(int groupIndex)
        {
            XElement sectionFooter = this.GetReportSection(SectionTypeEnum.TableFooter, groupIndex);

            if (sectionFooter.Attribute("CanvasHeight").Value.ToDecimal() <= 0)
            {
                return;
            }

            this.AddObjectModels(sectionFooter, null);
        }

        private void BuildReportData(XElement row)
        {
            int sectionIndex = row.GetRowSectionIndex();

            XElement dataSection = this.dataSections[sectionIndex];

            this.AddObjectModels(dataSection, row);

            if (row.Elements().Any(e => e.HasElements))
            {
                List<int> groupIndexes = new List<int>();

                foreach (XElement childtable in row.Elements().Where(ch => ch.HasElements))
                {
                    int groupIndex = childtable.GetSectionGroupIndex();

                    groupIndexes.Add(groupIndex);

                    this.SetSectionHeader(groupIndex);

                    foreach (XElement childRow in childtable.Elements())
                    {
                        this.BuildReportData(childRow);
                    }
                
                    for(int x = groupIndexes.Count; x > 0; --x)
                    {
                        this.SetSectionFooter(groupIndexes[x -1]);
                    }
                }
            }
        }

        private void AddObjectModels(XElement sectionElement, XElement row)
        {
            double minBottom = sectionElement.Attribute("CanvasHeight").Value.ToDouble() + this.activeCanvas.TopOffset;

            XElement canvas = sectionElement.GetCanvasXml();

            List<XElement> reportObjects = canvas.GetReportObjects();

            double lowestBottom = this.activeCanvas.TopOffset;

            bool isReset = false;

            foreach (XElement item in reportObjects)
            {
                if (item.IsDataObject())
                {
                    XElement dataValue = row.Element(item.GetObjectColumn());

                    if (item.IsEmptyDataSuppressed() && (dataValue == null || dataValue.Value.IsNullEmptyOrWhiteSpace()))
					{
                        continue;
					}

                    XElement dataItem = new XElement(item); // Do this tp preserve the original xml

                    dataItem.Attribute("ColumnModel").Remove();

                    //SuppressIfNoData

                    dataItem.Add(new XAttribute("Text", (dataValue == null ? "OOPS What a mess" : dataValue.Value)));

                    lowestBottom = this.AddObectModel(dataItem, lowestBottom, out isReset);
                }
                else
                {
                    lowestBottom = this.AddObectModel(item, lowestBottom, out isReset);
                }
                
                if (isReset)
                {
                    minBottom = sectionElement.Attribute("CanvasHeight").Value.ToDouble() + this.activeCanvas.TopOffset;
                }
            }
                            
            this.activeCanvas.TopOffset = minBottom > lowestBottom ? minBottom : lowestBottom;
        }

        private double AddObectModel(XElement item, double lowestBottom, out bool reset)
        {
            double result = lowestBottom;

            reset = false;

            double elementBottom = 0;

            if (item.IsPageBreak())
            {
                result = 0;

                this.CompleteDataPageCanvas();

                this.CreateDataPageCanvas();

                reset = true;

                return result;
            }

            bool canAdd = this.activeCanvas.AddObject(item, SectionTypeEnum.TableData, out elementBottom);

            if (canAdd)
            {
                if (elementBottom > result)
                {
                    result = elementBottom;
                }
            }
            else
            {
                result = 0;

                this.CompleteDataPageCanvas();
                
                this.CreateDataPageCanvas();

                result = this.AddObectModel(item, result, out reset);

                reset = true;
            }

            return result;
        }

        private XElement GetReportSection(SectionTypeEnum sectionType, int sectionGroupIndex)
        {
            return this.dataSections
                .Values
                .FirstOrDefault(si => si.Attribute("SectionType").Value.ToInt32() == (int)sectionType
                                   && si.Attribute("SectionGroupIndex").Value.ToInt32() == sectionGroupIndex);
        }

        #endregion
    }
}
