using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using Microsoft.Win32;
using REPORT.Builder.Printing;
using REPORT.Builder.ReportComponents;
using REPORT.Builder.ReportTools;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using ViSo.Dialogs.Controls;
using ViSo.Dialogs.TextEditor;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.ColoutPicker;
using WPF.Tools.Exstention;
using WPF.Tools.Functions;
using WPF.Tools.ToolModels;


namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner : UserControlBase
    {
        #region FIELDS

        private ReportTypeEnum reportDesignType;

        private ReportMasterModel reportMaster;

        private UIElement selectedReportObject;

        private DataSourceMasterModel dataSourceMainTable;

        private List<ReportSection> dataReportSections = new List<ReportSection>();

        #endregion

        public ReportDesigner(ReportMasterModel masterModel)
        {
            this.InitializeComponent();

            this.InitializeToolsStack();

            this.SizeChanged += this.ReportDesigner_SizeChanged;

            this.reportDesignType = (ReportTypeEnum)masterModel.ReportTypeEnum;

            this.ReportMaster = masterModel;

            this.uxReportMasterModel.Items.Add(this.ReportMaster);

            if (this.ReportMaster.MasterReport_Id == 0)
            {   // New Report
                this.ReportMaster.ReportXMLVersion = 1;

                this.InitializeReportSections();
            }
            else
            {
                ReportTablesRepository repo = new ReportTablesRepository();

                this.ReportMaster.ReportXMLVersion = repo.GetReportXMLVersion(masterModel.MasterReport_Id);

                ReportXMLModel reportXML = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, masterModel.MasterReport_Id);

                string reportXml = reportXML.BinaryXML.UnzipFile().ParseToString();

                XDocument report = XDocument.Parse(reportXml);

                this.reportDesignType = (ReportTypeEnum)report.Root.Element("ReportSettings").Attribute("ReportTypeEnum").Value.ToInt32();

                foreach (XElement sectionElement in report.Root.Element("ReportSettings").Elements("ReportSection").OrderBy(si => si.Attribute("SectionIndex").Value.ToInt32()))
                {
                    ReportSection section = new ReportSection { IsDesignMode = true };

                    section.SectionXml = sectionElement;

                    this.uxReportSections.Children.Add(section);

                    section.ReportObjectSelected += this.ReportObject_Selected;

                    if (section.SectionType == SectionTypeEnum.TableData)
                    {
                        section.RequestNewDataSections += this.NewDataSection_Requested;

                        section.ReportColumnAdded += this.ReportColumn_Added;
                    }
                    else if (this.reportDesignType == ReportTypeEnum.CoverPage 
                        || this.reportDesignType == ReportTypeEnum.FinalPage)
                    {
                        section.RefreshPageStaticMarkers();
                    }

                    this.dataReportSections.Add(section);
                }
            }

            this.uxReportSections.MinWidth = this.CanvasWidth + 200;

            this.RefreshMarginMarkers();

            this.SetupDataReportsOptions();

            this.ReportMaster.PropertyChanged += this.ReportMaster_PropertyChanged;
        }

        #region PBLIC PROPERTIES

        public ReportMasterModel ReportMaster
        {
            get
            {
                return this.reportMaster;
            }

            set
            {
                this.reportMaster = value;
            }
        }

        public DataSourceMasterModel DataSourceMainTable
        {
            get
            {
                return this.dataSourceMainTable;
            }

            set
            {
                this.dataSourceMainTable = value;
            }
        }

        #endregion

        #region PUBLIC METHODS

        public bool Save()
        {
            try
            {
                if (this.uxReportMasterModel.HasValidationError)
                {
                    return false;
                }

                ReportTablesRepository repo = new ReportTablesRepository();

                repo.UpdateReportMaster(this.ReportMaster);

                #region REPORT XML

                ReportXMLModel reportXml = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, this.ReportMaster.MasterReport_Id);

                if (reportXml == null)
                {
                    reportXml = new ReportXMLModel
                    {
                        MasterReport_Id = this.ReportMaster.MasterReport_Id,
                        ReportXMLVersion = this.ReportMaster.ReportXMLVersion,
                        PrintCount = 0
                    };
                }

                reportXml.BinaryXML = this.GetReportXml().ToString().ZipFile();

                repo.UpdateReportXML(reportXml);

                #endregion

                #region REPORT CONNECTIONS

                repo.SetReportConnectionsSttus(this.ReportMaster.MasterReport_Id, false, false);

                DatabaseModel databaseModel = Connections.Instance.GetConnection(this.reportMaster.ProductionConnection);

                if (databaseModel != null)
                {
                    ReportConnectionModel connection = new ReportConnectionModel
                    {
                        MasterReport_Id = this.ReportMaster.MasterReport_Id,
                        ReportConnectionName = this.reportMaster.ProductionConnection,
                        DatabaseTypeEnum = (int)databaseModel.DatabaseType,
                        ServerName = databaseModel.ServerName,
                        DatabaseName = databaseModel.DatabaseName,
                        UserName = databaseModel.UserName,
                        Password = databaseModel.Password.Encrypt(),
                        TrustedConnection = databaseModel.TrustedConnection,
                        IsProductionConnection = true,
                        IsActive = true
                    };
                
                    repo.UpdateReportConnection(connection);
                }

                #endregion

                #region SAVE DATASOURCE SELECTION

                if (this.DataSourceMainTable != null)
                {
                    DataSourceRepository sourceRepo = new DataSourceRepository();

                    this.DataSourceMainTable.MasterReport_Id = this.ReportMaster.MasterReport_Id;

                    sourceRepo.UpdateDataSourceMaster(this.DataSourceMainTable);

                    sourceRepo.SetIsAvailable(this.ReportMaster.MasterReport_Id, false);

                    foreach (DataSourceTableModel sourceModel in this.DataSourceMainTable.SelectedSourceTables)
                    {
                        sourceModel.MasterReport_Id = this.ReportMaster.MasterReport_Id;

                        sourceRepo.UpdateDataSourceTable(sourceModel);
                    }
                }

                #endregion

                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
        }

        #endregion

        #region OVERRIDEs

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);

            switch(e.Key)
            {
                case Key.Up:

                    ResizeHandles.NotchUp(1);

                    break;

                case Key.Down:

                    ResizeHandles.NotchDown(1);

                    break;

                case Key.Left:

                    ResizeHandles.NotchLeft(1);

                    break;

                case Key.Right:

                    ResizeHandles.NotchRight(1);

                    break;
            }
        }

        #endregion

        #region EVENT HANDLERS

        private void ReportDesigner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.uxHorizontalRuler.Refresh(this.CanvasWidth, 20, this.CanvasHeight);
        }

        private void ReportObject_Selected(object sender, object reportObject)
        {
            try
            {
                this.uxProperties.Items.Clear();

                this.SelectedSectionGroupIndex = sender.GetPropertyValue("SectionGroupIndex").ToInt32();

                int sectionIndex = sender.GetPropertyValue("SectionIndex").ToInt32();

                if (this.SelectedSectionGroupIndex > 0 && reportObject == null)
                {
                    this.uxCanvasSql.Text = sender == null ? string.Empty : sender.GetPropertyValue("SQLQuery").ParseToString();
                    
                    this.uxWhereBuilder.AddSectionOptions(this.dataReportSections.ToArray(), sender.GetPropertyValue("SectionGroupIndex").ToInt32());

                    this.uxPropertiesCaption.Visibility = Visibility.Collapsed;

                    this.uxProperties.Visibility = Visibility.Collapsed;

                    this.uxWhereBuilder.Visibility = Visibility.Visible;

                    this.uxWhereBuilderCaption.Visibility = Visibility.Visible;

                    this.uxCanvasSql.Visibility = Visibility.Visible;

                    this.uxCanvasSqlCaption.Visibility = Visibility.Visible;

                    return;
                }
                else if (this.SelectedSectionGroupIndex == 0 && reportObject == null)
                {
                    this.uxPropertiesCaption.Visibility = Visibility.Collapsed;

                    this.uxProperties.Visibility = Visibility.Collapsed;

                    this.uxWhereBuilder.Visibility = Visibility.Collapsed;

                    this.uxWhereBuilderCaption.Visibility = Visibility.Collapsed;

                    this.uxCanvasSql.Visibility = Visibility.Collapsed;

                    this.uxCanvasSqlCaption.Visibility = Visibility.Collapsed;
                }
                else if (reportObject != null)
                {
                    this.uxPropertiesCaption.Visibility = Visibility.Visible;

                    this.uxProperties.Visibility = Visibility.Visible;

                    this.uxWhereBuilder.Visibility = Visibility.Collapsed;

                    this.uxWhereBuilderCaption.Visibility = Visibility.Collapsed;

                    this.uxCanvasSql.Visibility = Visibility.Collapsed;

                    this.uxCanvasSqlCaption.Visibility = Visibility.Collapsed;
                    
                    this.selectedReportObject = null;
                
                    this.selectedReportObject = reportObject as UIElement;

                    this.selectedReportObject.PreviewMouseRightButtonUp += this.SelecteReportObject_RightClick;
                    
                    this.uxProperties.Items.Add(reportObject);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportColumn_Added(object sender, ReportColumnModel column, int sectionGroupIndex)
        {
            try
            {
                this.RefreshSectionTitles();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void NewDataSection_Requested(object sender, ReportColumnModel column, int sectionGroupIndex)
        {
            try
            {
                ReportSection dataSection = this.dataReportSections
                    .FirstOrDefault(rp => rp.SectionTableName == column.TableName
                                        && rp.SectionType == SectionTypeEnum.TableData);

                if (dataSection == null)
                {
                    int newGroupIndex =this.CreateDatareportSections();

                    foreach(ReportSection section in this.dataReportSections.Where(si => si.SectionGroupIndex == newGroupIndex))
                    {
                        section.SectionTableName = column.TableName;
                    }

                    dataSection = this.dataReportSections
                    .FirstOrDefault(rp => rp.SectionGroupIndex == newGroupIndex
                                        && rp.SectionType == SectionTypeEnum.TableData);
                }
                
                dataSection.AddReportColumn(column);
                    
                this.RefreshSectionTitles();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SelecteReportObject_RightClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ContextMenu menu = new ContextMenu();

                //if (KeyActions.IsCtrlPressed)
                if (ResizeHandles.SelectedElementCount > 1)
                {
                    foreach (ReportAlignmentEnum alignment in Enum.GetValues(typeof(ReportAlignmentEnum)))
                    {
                        MenuItem alignmentItem = new MenuItem { Header = alignment.GetDescriptionAttribute(), Tag = alignment };

                        alignmentItem.Click += this.Alignment_Click;

                        menu.Items.Add(alignmentItem);
                    }
                }

                if (ResizeHandles.SelectedElementCount == 1)
                {
                    MenuItem delete = new MenuItem { Name = "uxDelete", Header = "Delete Object" };

                    delete.Click += this.SelectedMenuItem_Click;

                    menu.Items.Add(delete);
                }                
                    
                menu.IsOpen = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void Alignment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = (MenuItem)sender;

                ReportAlignmentEnum alignment = menu.Tag.To<ReportAlignmentEnum>();

                ResizeHandles.AlignmentObjects(alignment);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void SelectedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SectionCanvas canvas = (SectionCanvas)this.selectedReportObject.FindParentControlBase(typeof(SectionCanvas));

                ResizeHandles.RemoveElement(this.selectedReportObject);
                
                canvas.Children.Remove(this.selectedReportObject);

                this.selectedReportObject = null;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportMaster_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                switch(e.PropertyName)
                {
                    case "PaperKindEnum":

                        foreach(ReportSection section in this.uxReportSections.Children)
                        {
                            section.PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum;
                        }

                        break;

                    case "PageOrientationEnum":

                        foreach (ReportSection section in this.uxReportSections.Children)
                        {
                            section.PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum;
                        }

                        break;

                    case "PageMarginLeft":
                    case "PageMarginRight":

                    case "PageMarginTop":
                    case "PageMarginBottom":

                        if (this.reportDesignType == ReportTypeEnum.ReportContent
                            || this.reportDesignType == ReportTypeEnum.PageHeaderAndFooter)
                        {
                            break;
                        }

                        foreach(ReportSection section in this.uxReportSections.Children)
                        {
                            section.MarkerTopMargin = this.ReportMaster.PageMarginTop.ToInt32();

                            section.MarkerBottomMargin = this.ReportMaster.PageMarginBottom.ToInt32();

                            section.RefreshPageStaticMarkers();
                        }

                        break;

                }

                this.RefreshMarginMarkers();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportMasterDescription_Browse(object sender, string buttonKey)
        {
            try
            {
                if (TextEditing.ShowDialog("Description", this.ReportMaster.DescriptionText).IsFalse())
                {
                    return;
                }

                this.ReportMaster.DescriptionText = TextEditing.Text;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void PropertiesObject_Browse(object sender, string buttonKey)
        {
            try
            {
                switch(buttonKey)
                {
                    case "ReportImageKey":

                        OpenFileDialog dlg = new OpenFileDialog();

                        bool? result = dlg.ShowDialog();

                        if (!result.HasValue || !result.Value)
                        {
                            return;
                        }

                        this.selectedReportObject.SetPropertyValue("ImagePath", dlg.FileName);

                        break;

                    default:

                        ColourPicker picker = new ColourPicker();

                        if (picker.ShowDialog().IsFalse())
                        {
                            return;
                        }

                        this.selectedReportObject.SetPropertyValue(buttonKey, picker.SelectedColour);

                        break;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void DataSourseSelect_Browse(object sender, RoutedEventArgs e)
        {
            try
            {
                DatasourceSelector selector = new DatasourceSelector(this.ReportMaster.MasterReport_Id);

                if (ControlDialog.ShowDialog("Data Source", selector, "Accept").IsFalse())
                {
                    return;
                }

                this.DataSourceMainTable = selector.MainTable;

                this.LoadDataSource();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportPrint_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu menu = new ContextMenu();
                

                MenuItem defaultItem = new MenuItem
                {
                    Header = $"{Connections.Instance.DefaultConnectionName} ({Connections.Instance.DefaultDatabaseName})",
                    Tag = Connections.Instance.DefaultConnectionName
                };

                defaultItem.Click += this.ReportPrintConnection_Cliked;

                menu.Items.Add(defaultItem);

                foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
                {
                    MenuItem alternativeItem = new MenuItem 
                    { 
                        Header = $"{connectionKey.Key} ({connectionKey.Value.DatabaseName})", 
                        Tag = connectionKey.Key 
                    };

                    alternativeItem.Click += this.ReportPrintConnection_Cliked;

                    menu.Items.Add(alternativeItem);
                }

                menu.IsOpen = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void ReportPrintConnection_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.uxReportMasterModel.HasValidationError)
                {
                    return;
                }

                BuildReportPDF reportPrint = new BuildReportPDF();

                MenuItem item = (MenuItem)e.Source;

                DatabaseModel connection = Connections.Instance.GetConnection(item.Tag.ParseToString());

                BuildReportXML xmlBuild = new BuildReportXML();

                XDocument repotXml = xmlBuild.GetReport(this.GetReportXml(), connection, this.ReportMaster.CopyToObject(new ReportMaster()) as ReportMaster);

                reportPrint.PrintDocument(repotXml);

                PrintPreview preview = new PrintPreview(reportPrint.Pages);

                ControlDialog.ShowDialog("Reports", preview, string.Empty);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        #endregion

        #region PRIVATE PROPERTIES

        private int SelectedSectionGroupIndex { get; set; }

        private double CanvasWidth
        {
            get
            {
                return ((ReportSection)this.uxReportSections.Children[0]).GetPropertyValue("PageWidth").ToDouble();
            }
        }

        private double CanvasHeight
        {
            get
            {
                return this.uxReportSections.ActualHeight + 25;
            }
        }

        #endregion

        #region PRIVATE METHODS

        private void InitializeReportSections()
        {
            this.uxReportSections.Children.Clear();

            switch(this.reportDesignType)
            {
                case ReportTypeEnum.CoverPage:
                case ReportTypeEnum.FinalPage:

                    this.uxReportSections.Children.Add(new ReportSection
                    {
                        SectionIndex = 0,
                        SectionGroupIndex = 0,
                        SectionType = SectionTypeEnum.Page,
                        IsDesignMode = true,
                        PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                        PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
                        MarkerBottomMargin = 100,
                        MarkerTopMargin = 100
                    }); ;

                    break;

                case ReportTypeEnum.ReportContent:

                    this.CreateDatareportSections();                    

                    break;

                case ReportTypeEnum.PageHeaderAndFooter:

                    this.uxReportSections.Children.Add(new ReportSection 
                    { 
                        SectionIndex = 0,
                        SectionGroupIndex = 0,
                        SectionType = SectionTypeEnum.Header,
                        IsDesignMode = true,
                        PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                        PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
                        MarkerBottomMargin = 100,
                        MarkerTopMargin = 100
                    });

                    this.uxReportSections.Children.Add(new ReportSection 
                    { 
                        SectionIndex = 1,
                        SectionGroupIndex = 0,
                        SectionType = SectionTypeEnum.Footer,
                        IsDesignMode = true,
                        PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                        PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
                        MarkerBottomMargin = 100,
                        MarkerTopMargin = 100
                    });

                    break;
            }

            foreach(ReportSection section in this.uxReportSections.Children)
            {
                section.ReportObjectSelected += this.ReportObject_Selected;

                if (section.SectionType == SectionTypeEnum.TableData)
                {
                    section.RequestNewDataSections += this.NewDataSection_Requested;

                    section.ReportColumnAdded += this.ReportColumn_Added;
                }
            }
        }

        private void InitializeToolsStack()
        {
            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Label",  ToolType = typeof(ReportLabel) });

            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Border", ToolType = typeof(ReportBorder) });

            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Current Date", ToolType = typeof(CurrentDate) });

            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Image", ToolType = typeof(ReportImage) });

            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Horizontal Line", ToolType = typeof(ReportHorizontalLine) });

            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Vertical Line", ToolType = typeof(ReportVerticalLine) });

            this.uxToolsStack.Children.Add(new ToolsMenuItem { Caption = "Page Break", ToolType = typeof(ReportPageBreak) });
        }

        private void RefreshMarginMarkers()
        {
            double pageWidth = this.CanvasWidth;

            this.uxHorizontalRuler.ClearMarkers(true);

            this.uxHorizontalRuler.Refresh(pageWidth, 25, this.CanvasHeight);

            if (this.ReportMaster.PageMarginLeft.HasValue)
            {
                this.uxHorizontalRuler.AddMarker(this.ReportMaster.PageMarginLeft.Value, true);
            }

            if (this.ReportMaster.PageMarginRight.HasValue)
            {
                this.uxHorizontalRuler.AddMarker((pageWidth - this.ReportMaster.PageMarginRight.Value), true);
            }
        }

        private void SetupDataReportsOptions()
        {
            if (this.reportDesignType != ReportTypeEnum.ReportContent)
            {
                return;
            }

            this.uxReportMasterModel["Cover Page"].Visibility = Visibility.Visible;

            this.uxReportMasterModel["Page Headers and Footers"].Visibility = Visibility.Visible;

            this.uxReportMasterModel["Final Page"].Visibility = Visibility.Visible;

            this.uxReportMasterModel["Production Connection"].Visibility = Visibility.Visible;

            this.uxReportMasterModel["Production Connection"].IsRequired = true;

            this.uxDataMenueBorder.Visibility = Visibility.Visible;

            this.uxDataMenue.Visibility = Visibility.Visible;

            this.uxTableTree.Visibility = Visibility.Visible;

            DataSourceRepository repo = new DataSourceRepository();

            DataSourceMasterModel sourceMaster = repo.GetDataSourceMasterByPrimaryKey(this.ReportMaster.MasterReport_Id);

            if (sourceMaster != null)
            {
                sourceMaster.SelectedSourceTables.AddRange(repo.GetDataSourceTableByForeignKeyMasterReport_Id(this.ReportMaster.MasterReport_Id));

                this.DataSourceMainTable = sourceMaster;

                this.LoadDataSource();
            }
        }

        private void LoadDataSource()
        {
            this.uxTableTree.Items.Clear();

            foreach (ReportSection section in this.uxReportSections.Children)
            {
                if (section.SectionGroupIndex != 0)
                {
                    continue;
                }
                                
                section.SectionTableName = this.DataSourceMainTable.MainTableName; // Do this to reserve the first section for the main data source

                section.RefresSectionTitle();
            }

            TreeViewItem mainTreeItem = new TreeViewItem { Header = this.DataSourceMainTable.MainTableName };

            foreach(DataItemModel column in Integrity.GetColumnsForTable(this.DataSourceMainTable.MainTableName))
            {
                ColumnObjectModel tableColumn = Integrity.GetObjectModel(this.DataSourceMainTable.MainTableName, column.DisplayValue);

                ReportColumnModel reportColumn = tableColumn.CopyToObject(new ReportColumnModel { TableName = this.DataSourceMainTable.MainTableName }) as ReportColumnModel;

                ToolsMenuItem treeMainColumn = new ToolsMenuItem
                {
                    Caption = column.DisplayValue,
                    ToolType = typeof(ReportDataObject),
                    Tag = column.CopyToObject(reportColumn)
                };

                mainTreeItem.Items.Add(treeMainColumn);
            }

            this.uxTableTree.Items.Add(mainTreeItem);

            foreach(DataSourceTableModel sourceTable in this.DataSourceMainTable.SelectedSourceTables)
            {
                if (sourceTable.TableName == this.DataSourceMainTable.MainTableName)
                {
                    continue;
                }

                TreeViewItem childTreeItem = new TreeViewItem { Header = sourceTable.TableName };

                foreach (DataItemModel column in Integrity.GetColumnsForTable(sourceTable.TableName))
                {
                    ColumnObjectModel tableColumn = Integrity.GetObjectModel(sourceTable.TableName, column.DisplayValue);

                    ReportColumnModel reportColumn = tableColumn.CopyToObject(new ReportColumnModel { TableName = sourceTable.TableName }) as ReportColumnModel;

                    ToolsMenuItem treeChildColumn = new ToolsMenuItem
                    {
                        Caption = column.DisplayValue,
                        ToolType = typeof(ReportDataObject),
                        Tag = column.CopyToObject(reportColumn)
                    };

                    childTreeItem.Items.Add(treeChildColumn);
                }

                this.uxTableTree.Items.Add(childTreeItem);
            }            
        }
                
        private XDocument GetReportXml()
        {
            XDocument result = new XDocument();

            XElement root = new XElement("Root");

            XElement report = new XElement("ReportSettings");

            report.Add(new XAttribute("ReportTypeEnum", (int)this.reportDesignType));

            foreach (ReportSection section in this.uxReportSections.Children)
            {
                report.Add(section.SectionXml);
            }

            root.Add(report);

            result.Add(root);

            //ReportXMLModel reportXml = repo.GetReportXMLByPrimaryKey(this.ReportMaster.ReportXMLVersion, this.ReportMaster.MasterReport_Id);

            //if (reportXml == null)
            //{
            //    reportXml = new ReportXMLModel
            //    {
            //        MasterReport_Id = this.ReportMaster.MasterReport_Id,
            //        ReportXMLVersion = this.ReportMaster.ReportXMLVersion,
            //        PrintCount = 0
            //    };
            //}

            return result;
        }

        #endregion

        #region NEW DATA SECTION METHODS

        private int CreateDatareportSections()
        {
            int groupSectionId = this.NextGroupIndex();

            int[] sectionIndex = this.CalculateSectionIndexes();

            ReportSection header = new ReportSection
            {
                SectionIndex = sectionIndex[0],
                SectionGroupIndex = groupSectionId,
                SectionType = SectionTypeEnum.TableHeader,
                IsDesignMode = true,
                PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
                MarkerBottomMargin = 100,
                MarkerTopMargin = 100
            };

            this.uxReportSections.Children.Insert(sectionIndex[0], header);

            this.dataReportSections.Add(header);

            ReportSection data = new ReportSection
            {
                SectionIndex = sectionIndex[1],
                SectionGroupIndex = groupSectionId,
                SectionType = SectionTypeEnum.TableData,
                IsDesignMode = true,
                PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
                MarkerBottomMargin = 100,
                MarkerTopMargin = 100
            };

            this.uxReportSections.Children.Insert(sectionIndex[1], data);

            this.dataReportSections.Add(data);

            ReportSection footer = new ReportSection
            {
                SectionIndex = sectionIndex[2],
                SectionGroupIndex = groupSectionId,
                SectionType = SectionTypeEnum.TableFooter,
                IsDesignMode = true,
                PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum,
                MarkerBottomMargin = 100,
                MarkerTopMargin = 100
            };

            this.uxReportSections.Children.Insert(sectionIndex[2], footer);

            this.dataReportSections.Add(footer);

            header.ReportObjectSelected += this.ReportObject_Selected;

            data.ReportObjectSelected += this.ReportObject_Selected;

            data.RequestNewDataSections += this.NewDataSection_Requested;

            data.ReportColumnAdded += this.ReportColumn_Added;

            //data.ReportSectionWhereClauseChanged += this.ReportSectionWhereClause_Changed;

            footer.ReportObjectSelected += this.ReportObject_Selected;

            return groupSectionId;
        }
        
        private void RefreshSectionTitles()
        {
            foreach (ReportSection section in this.dataReportSections)
            {
                section.RefresSectionTitle();
            }
        }

        private int NextGroupIndex()
        {
            return this.dataReportSections.Count == 0 ? 0 :
                (this.dataReportSections.Select(gr => gr.SectionGroupIndex).Max() + 1);
        }

        private int GetMaxSectionIndex()
        {   // Zero based index
            return this.dataReportSections.Count == 0 ? 2 :
                (this.dataReportSections.Select(gr => gr.SectionIndex).Max());
        }

        private int[] CalculateSectionIndexes()
        {
            int nextGroupIndex = this.NextGroupIndex();

            if (nextGroupIndex == 0)
            {
                return new int[] { 0, 1, 2 };
            }

            int maxSectionIndex = this.GetMaxSectionIndex();

            int insertIndexStart = ((maxSectionIndex + 1) / 3) + ((maxSectionIndex + 1) / 3);

            int[] result = new int[3];

            result[0] = insertIndexStart;
            result[1] = insertIndexStart + 1;
            result[2] = insertIndexStart + 2;

            int shiftIndex = result[2] + 1;

            ReportSection[] changeSections = this.dataReportSections
                .Where(si => si.SectionIndex >= insertIndexStart)
                .OrderBy(oi => oi.SectionIndex)
                .ToArray();

            foreach(ReportSection section in changeSections)
            {
                section.SectionIndex = shiftIndex;

                ++shiftIndex;
            }

            return result;
        }

        #endregion        
    }
}
