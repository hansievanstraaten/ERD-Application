using ERD.Models;
using GeneralExtensions;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder.ReportComponents
{
    /// <summary>
    /// Interaction logic for ReportSection.xaml
    /// </summary>
    public partial class ReportSection : UserControlBase
    {
        public delegate void RequestNewDataSectionsEvent(object sender, ReportColumnModel column, int sectionGroupIndex);

        public delegate void ReportColumnAddedEvent(object sender, ReportColumnModel column, int sectionGroupIndex);

        public delegate void ReportObjectSelectedEvent(object sender, object reportObject);

        public event RequestNewDataSectionsEvent RequestNewDataSections;

        public event ReportColumnAddedEvent ReportColumnAdded;

        public event ReportObjectSelectedEvent ReportObjectSelected;

        private int markerTopMargin = 100;

        private int markerBottomMargin = 100;

        private double canvasHeight;

        private SectionTypeEnum sectionType;

        private PaperKind paperKind;

        private PageMediaSize pageSize;

        private PageOrientationEnum pageOrientation;
        
        public ReportSection()
        {
            this.InitializeComponent();

            this.SizeChanged += this.ReportSection_SizeChanged;
        }

        public XElement SectionXml
        {
            get
            {
                bool isCollapsed = this.uxCollapse.Direction == WPF.Tools.Specialized.DirectionsEnum.Right;

                XElement result = new XElement("ReportSection");

                if (this.SectionTableName.IsNullEmptyOrWhiteSpace())
                {
                    if (this.SectionType == SectionTypeEnum.TableData
                        || this.SectionType == SectionTypeEnum.TableFooter
                        || this.SectionType == SectionTypeEnum.TableHeader)
                    {
                        throw new ApplicationException("Data source not selected");
                    }

                    this.SectionTableName = string.Empty;
                }

                result.Add(new XAttribute("SectionType", (int)this.SectionType));
                result.Add(new XAttribute("SectionTitle", this.SectionTitle));
                result.Add(new XAttribute("PaperKind", this.PaperKind));
                result.Add(new XAttribute("SectionIndex", this.SectionIndex));
                result.Add(new XAttribute("SectionGroupIndex", this.SectionGroupIndex));
                result.Add(new XAttribute("CanvasHeight", (isCollapsed ? this.CanvasHeight : this.uxMainGrid.RowDefinitions[1].Height.Value)));
                result.Add(new XAttribute("PageOrientation", this.PageOrientation));
                result.Add(new XAttribute("MarkerTopMargin", this.MarkerTopMargin));
                result.Add(new XAttribute("MarkerBottomMargin", this.MarkerBottomMargin));
                result.Add(new XAttribute("SectionTableName", this.SectionTableName));

                result.Add(this.uxSectionCanvas.CanvasXml);

                return result;
            }

            set
            {
                this.uxSectionCanvas.CanvasXml = value.Element("CanvasXml");

                foreach (XAttribute property in value.Attributes())
                {
                    this.SetPropertyValue(property.Name.LocalName, property.Value);
                }

                this.CanvasHeight = value.Attribute("CanvasHeight").Value.ToDouble();
            }
        }

        public int SectionIndex { get; set; }

        public int SectionGroupIndex { get; set; }

        public int MarkerTopMargin
        {
            get
            {
                return this.markerTopMargin;
            }

            set
            {
                this.markerTopMargin = value;
            }
        }

        public int MarkerBottomMargin
        {
            get
            {
                return this.markerBottomMargin;
            }

            set
            {
                this.markerBottomMargin = value;
            }
        }

        public double CanvasHeight
        {
            get
            {
                return this.canvasHeight;
            }

            set
            {
                this.canvasHeight = value;

                this.uxMainGrid.RowDefinitions[1].Height = new GridLength(value);
            }
        }

        public bool IsDesignMode
        {
            get
            {
                return this.uxSectionCanvas.IsDesignMode;
            }

            set
            {
                this.uxSectionCanvas.IsDesignMode = value;
            }
        }

        public string SectionTitle
        {
            get
            {
                return this.uxSectionCaption.Content.ParseToString();
            }

            private set
            {
                this.uxSectionCaption.Content = value;

                this.Title = value;
            }
        }

        public string SectionTableName
        {
            get
            {
                return this.uxSectionCanvas.SectionTableName;
            }

            set
            {
                this.uxSectionCanvas.SectionTableName = value;
            }
        }

        public string SQLQuery
        {
            get
            {
                return this.uxSectionCanvas.SQLQuery;
            }
        }

        public SectionTypeEnum SectionType
        {
            get
            {
                return this.sectionType;
            }

            set
            {
                this.sectionType = value;

                this.uxSectionCanvas.SectionType = value;

                this.SetPageAndCanvasSize();
            }
        }

        public PaperKind PaperKind
        {
            get
            {
                return this.paperKind;
            }

            set
            {
                this.paperKind = value;

                this.SetPageAndCanvasSize();

                if (this.SectionType == SectionTypeEnum.Page)
                {
                    this.RefreshPageStaticMarkers();
                }
            }
        }

        public PageOrientationEnum PageOrientation 
        { 
            get
            {
                return this.pageOrientation;
            }

            set
            {
                this.pageOrientation = value;

                this.SetPageAndCanvasSize();

                if (this.SectionType == SectionTypeEnum.Page)
                {
                    this.RefreshPageStaticMarkers();
                }
            }
        }

        public PageMediaSize PageSize
        {
            get
            {
                return this.pageSize;
            }

            private set
            {
                this.pageSize = value;
            }
        }

        public CanvasSqlManager SqlManager
        {
            get
            {
                return this.uxSectionCanvas.SqlManager;
            }
        }

        public List<ReportColumnModel> ReportColumns
        {
            get
            {
                return this.uxSectionCanvas.ReportColumns;
            }
        }

        public ReportSQLReplaceHeaderModel GetReplacementColumn(string tableName, string columnName)
        {
            return this.SqlManager.GetReplacementColumn(tableName, columnName);
        }

        public void UpdateReplacementColumn(ReportSQLReplaceHeaderModel replacementValues)
        {
            this.SqlManager.UpdateReplacementColumn(replacementValues);
        }

        public ReportsInvokeReplaceModel GetInvokeMethod(string tableName, string columnName)
        {
            return this.SqlManager.GetInvokeMethod(tableName, columnName);
        }

        public void UpdateInvokeReplaceModel(ReportsInvokeReplaceModel invokeModel)
        {
            this.SqlManager.UpdateInvokeReplaceModel(invokeModel);
        }

        public void RefreshPageStaticMarkers()
        {
            this.uxVerticalRuler.ClearMarkers(true);

            this.uxVerticalRuler.AddMarker(this.MarkerTopMargin.ToDouble(), true);

            this.uxVerticalRuler.AddMarker((this.PageHeight - this.MarkerBottomMargin.ToDouble()), true);
        }

        public void RefresSectionTitle()
        {
            string sectionTitle = this.SectionType == SectionTypeEnum.TableData || this.SectionType == SectionTypeEnum.TableFooter || this.SectionType == SectionTypeEnum.TableHeader ?
                $"{this.SectionIndex} - {this.SectionTableName} - {this.SectionType.GetDescriptionAttribute()}" :
                this.SectionType.GetDescriptionAttribute();

            this.uxSectionCaption.Content = sectionTitle;

            this.Title = sectionTitle;
        }

        public void AddReportColumn(ReportColumnModel column)
        {
            this.uxSectionCanvas.AddReportColumn(column);
        }

        private void ReportSection_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RefreshRuler(this.uxMainGrid.RowDefinitions[1].ActualHeight, this.uxMainGrid.ColumnDefinitions[2].ActualWidth);
        }

        private void Collapse_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                bool isCollapsing = this.uxCollapse.Direction == WPF.Tools.Specialized.DirectionsEnum.Down;

                if (isCollapsing)
                {
                    this.CanvasHeight = this.uxMainGrid.RowDefinitions[1].Height.Value;

                    this.uxSectionCanvas.Visibility = Visibility.Collapsed;

                    this.uxVerticalRuler.Visibility = Visibility.Collapsed;
                    
                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(0);

                    this.uxMainGrid.RowDefinitions[2].Height = new GridLength(0);

                    this.uxCollapse.Direction = WPF.Tools.Specialized.DirectionsEnum.Right;
                }
                else
                {
                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(this.CanvasHeight);

                    this.uxMainGrid.RowDefinitions[2].Height = new GridLength(3);

                    this.uxCollapse.Direction = WPF.Tools.Specialized.DirectionsEnum.Down;

                    this.uxSectionCanvas.Visibility = Visibility.Visible;

                    this.uxVerticalRuler.Visibility = Visibility.Visible;

                    this.RefreshRuler(this.CanvasHeight, this.uxMainGrid.ColumnDefinitions[2].ActualWidth);
                }

                this.ReportObjectSelected?.Invoke(this, null);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportObject_Selected(object sender)
        {
            this.ReportObjectSelected?.Invoke(this, sender);
        }
        
        private void ReportColumn_Added(object sender, ReportColumnModel column)
        {
            this.ReportColumnAdded?.Invoke(sender, column, this.SectionGroupIndex);
        }

        private void SectionCanvas_RequestNewDataSection(object sender, ReportColumnModel column)
        {
            this.RequestNewDataSections?.Invoke(sender, column, this.SectionGroupIndex);
        }

        private void SetPageAndCanvasSize()
        {
            this.PageSize = PageSetupOptions.GetPageMediaSize(this.PaperKind);

            //var margins = PageSetupOptions.GetPageMargins(this.paperKind);

            this.uxSectionCanvas.Width = this.PageWidth;

            switch (this.SectionType)
            {
                case SectionTypeEnum.Page:

                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(this.PageHeight);

                    this.uxMainGrid.RowDefinitions[2].Height = new GridLength(0);

                    this.uxBottomHandle.Visibility = Visibility.Collapsed;

                    this.uxSectionCanvas.Height = this.PageHeight;

                    break;

                case SectionTypeEnum.Header:
                case SectionTypeEnum.Footer:
                default:

                    this.uxMainGrid.RowDefinitions[1].Height = new GridLength(150);
                    
                    break;

            }

            this.RefreshRuler(this.uxSectionCanvas.Height, this.uxMainGrid.ColumnDefinitions[2].ActualWidth);
        }
        
        private void RefreshRuler(double heigh, double traceLength)
        {
            this.uxVerticalRuler.Refresh(25, heigh, traceLength + 25);
        }
        
        public double PageHeight
        {
            get
            {
                return this.PageOrientation == PageOrientationEnum.Portrait ? this.PageSize.Height.Value : this.PageSize.Width.Value;
            }
        }

        public double PageWidth
        {
            get
            {
                return this.PageOrientation == PageOrientationEnum.Portrait ? this.PageSize.Width.Value : this.PageSize.Height.Value;
            }
        }
    }
}
