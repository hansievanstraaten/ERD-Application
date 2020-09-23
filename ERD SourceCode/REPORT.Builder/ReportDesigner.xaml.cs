using ERD.Common;
using ERD.Models;
using GeneralExtensions;
using Microsoft.Win32;
using REPORT.Builder.ReportComponents;
using REPORT.Builder.ReportTools;
using REPORT.Data.Models;
using REPORT.Data.SQLRepository.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using ViSo.Dialogs.Controls;
using ViSo.Dialogs.TextEditor;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.BaseClasses;
using WPF.Tools.Exstention;
using WPF.Tools.ToolModels;

namespace REPORT.Builder
{
    /// <summary>
    /// Interaction logic for ReportDesigner.xaml
    /// </summary>
    public partial class ReportDesigner : UserControlBase
    {
        private double markerMargin = 79;

        private ReportTypeEnum reportDesignType;

        private ReportMasterModel reportMaster;

        private UIElement selectedReportObject;

        private DataSourceMasterModel dataSourceModel;

        private List<ReportSection> dataReportSections = new List<ReportSection>();

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

                    this.dataReportSections.Add(section);
                }
            }

            this.uxReportSections.MinWidth = this.CanvasWidth + 200;

            this.AddMarginMarkers();

            this.SetupDataReportsOptions();

            this.ReportMaster.PropertyChanged += this.ReportMaster_PropertyChanged;
        }

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

                XDocument result = new XDocument();

                XElement root = new XElement("Root");

                XElement report = new XElement("ReportSettings");

                report.Add(new XAttribute("ReportTypeEnum", (int)this.reportDesignType));

                foreach(ReportSection section in this.uxReportSections.Children)
                {
                    report.Add(section.SectionXml);
                }

                root.Add(report);

                result.Add(root);

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

                reportXml.BinaryXML = result.ToString().ZipFile();

                repo.UpdateReportXML(reportXml);

                #endregion

                #region REPORT CONNECTIONS
                    
                ReportConnectionModel connection = new ReportConnectionModel
                {
                    MasterReport_Id = this.ReportMaster.MasterReport_Id,
                    ReportConnection_Id = 0
                };

                if (this.ReportMaster.ProductionConnection == 0)
                {   // Use the Default Connection
                    //connection.DatabaseTypeEnum = Connections.Instance[]

                    //Connections.Instance.DefaultConnectionName
                }
                else
                {

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

        private void ReportDesigner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.uxHorizontalRuler.Refresh(this.CanvasWidth, 20, this.CanvasHeight);
        }

        private void ReportObject_Selected(object sender)
        {
            try
            {
                this.uxProperties.Items.Clear();

                if (this.selectedReportObject != null)
                {
                    // TODO: Remove Selected Heiglight from object
                    //this.selectedReportObject.SetPropertyValue("ItemSelected", false);

                    this.selectedReportObject = null;
                }

                if (sender == null)
                {
                    return;
                }

                this.selectedReportObject = sender as UIElement;

                this.selectedReportObject.PreviewMouseRightButtonUp += this.SelecteReportObject_RightClick;
                    
                // TODO: Heiglight Selected object for resizing
                //this.selectedReportObject.SetPropertyValue("ItemSelected", true);

                this.uxProperties.Items.Add(sender);
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
                this.SetSectionTitle(column, sectionGroupIndex);
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

                    this.SetSectionTitle(column, newGroupIndex);

                    dataSection = this.dataReportSections
                    .FirstOrDefault(rp => rp.SectionGroupIndex == newGroupIndex
                                        && rp.SectionType == SectionTypeEnum.TableData);

                }
                
                dataSection.AddReportColumn(column);
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

                MenuItem delete = new MenuItem { Name = "uxDelete", Header = "Delete Object" };

                delete.Click += this.SelectedMenuItem_Click;

                menu.Items.Add(delete);

                menu.IsOpen = true;
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
                SectionCanvas parent = (SectionCanvas)this.selectedReportObject.FindParentControlBase(typeof(SectionCanvas));

                parent.Children.Remove(this.selectedReportObject);

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

                }

                this.AddMarginMarkers();
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
                if (this.ReportMaster.MasterReport_Id <= 0)
                {
                    if (!this.Save())
                    {
                        return;
                    }
                }

                DatasourceSelector selector = new DatasourceSelector(this.ReportMaster.MasterReport_Id);

                if (ControlDialog.ShowDialog("Data Source", selector, "Accept").IsFalse())
                {
                    return;
                }

                this.dataSourceModel = selector.MainTable;

                this.LoadDataSource();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }

        private void InitializeReportSections()
        {
            this.uxReportSections.Children.Clear();

            switch(this.reportDesignType)
            {
                case ReportTypeEnum.CoverPage:
                case ReportTypeEnum.FinalPage:

                    this.uxReportSections.Children.Add(new ReportSection
                    {
                        SectionTitle = this.reportDesignType.GetDescriptionAttribute(),
                        SectionIndex = 0,
                        SectionGroupIndex = 0,
                        SectionType = SectionTypeEnum.Page,
                        IsDesignMode = true,
                        PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                        PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum
                    }); ;

                    break;

                case ReportTypeEnum.ReportContent:

                    this.CreateDatareportSections();                    

                    break;

                case ReportTypeEnum.PageHeaderAndFooter:

                    this.uxReportSections.Children.Add(new ReportSection 
                    { 
                        SectionTitle = "Page Header", 
                        SectionIndex = 0,
                        SectionGroupIndex = 0,
                        SectionType = SectionTypeEnum.Header,
                        IsDesignMode = true,
                        PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                        PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum
                    });

                    this.uxReportSections.Children.Add(new ReportSection 
                    { 
                        SectionTitle = "Page Footer", 
                        SectionIndex = 1,
                        SectionGroupIndex = 0,
                        SectionType = SectionTypeEnum.Footer,
                        IsDesignMode = true,
                        PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                        PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum
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

        private void AddMarginMarkers()
        {
            double pageWidth = this.CanvasWidth;

            this.uxHorizontalRuler.ClearMarkers(true);

            this.uxHorizontalRuler.Refresh(pageWidth, 25, this.CanvasHeight);

            this.uxHorizontalRuler.AddMarker(this.markerMargin, true);

            this.uxHorizontalRuler.AddMarker((pageWidth - this.markerMargin), true);
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

                this.dataSourceModel = sourceMaster;

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

                section.SectionTitle = $"{this.dataSourceModel.MainTableName} - {section.SectionType.GetDescriptionAttribute()}";

                section.SectionTableName = this.dataSourceModel.MainTableName; // Do this to reserve the first section for the main data source
            }

            TreeViewItem mainTreeItem = new TreeViewItem { Header = this.dataSourceModel.MainTableName };

            foreach(DataItemModel column in Integrity.GetColumnsForTable(this.dataSourceModel.MainTableName))
            {
                ColumnObjectModel tableColumn = Integrity.GetObjectModel(this.dataSourceModel.MainTableName, column.DisplayValue);

                ReportColumnModel reportColumn = tableColumn.CopyToObject(new ReportColumnModel { TableName = this.dataSourceModel.MainTableName }) as ReportColumnModel;

                ToolsMenuItem treeMainColumn = new ToolsMenuItem
                {
                    Caption = column.DisplayValue,
                    ToolType = typeof(ReportDataObject),
                    Tag = column.CopyToObject(reportColumn)
                };

                mainTreeItem.Items.Add(treeMainColumn);
            }

            this.uxTableTree.Items.Add(mainTreeItem);

            foreach(DataSourceTableModel sourceTable in this.dataSourceModel.SelectedSourceTables)
            {
                if (sourceTable.TableName == this.dataSourceModel.MainTableName)
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

        #region NEW DATA SECTION METHODS

        private int CreateDatareportSections()
        {
            int groupSectionId = this.NextGroupIndex();

            int[] sectionIndex = this.CalculateSectionIndexes();

            ReportSection header = new ReportSection
            {
                SectionTitle = "Table Header",
                SectionIndex = sectionIndex[0],
                SectionGroupIndex = groupSectionId,
                SectionType = SectionTypeEnum.TableHeader,
                IsDesignMode = true,
                PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum
            };

            this.uxReportSections.Children.Insert(sectionIndex[0], header);

            this.dataReportSections.Add(header);

            ReportSection data = new ReportSection
            {
                SectionTitle = "Table Data",
                SectionIndex = sectionIndex[1],
                SectionGroupIndex = groupSectionId,
                SectionType = SectionTypeEnum.TableData,
                IsDesignMode = true,
                PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum
            };

            this.uxReportSections.Children.Insert(sectionIndex[1], data);

            this.dataReportSections.Add(data);

            ReportSection footer = new ReportSection
            {
                SectionTitle = "Table Footer",
                SectionIndex = sectionIndex[2],
                SectionGroupIndex = groupSectionId,
                SectionType = SectionTypeEnum.TableFooter,
                IsDesignMode = true,
                PaperKind = (PaperKind)this.ReportMaster.PaperKindEnum,
                PageOrientation = (PageOrientationEnum)this.ReportMaster.PageOrientationEnum
            };

            this.uxReportSections.Children.Insert(sectionIndex[2], footer);

            this.dataReportSections.Add(footer);

            header.ReportObjectSelected += this.ReportObject_Selected;

            data.ReportObjectSelected += this.ReportObject_Selected;

            data.RequestNewDataSections += this.NewDataSection_Requested;

            data.ReportColumnAdded += this.ReportColumn_Added;

            footer.ReportObjectSelected += this.ReportObject_Selected;

            return groupSectionId;
        }

        private void SetSectionTitle(ReportColumnModel column, int sectionGroupIndex)
        {
            foreach (ReportSection section in this.dataReportSections.Where(gr => gr.SectionGroupIndex == sectionGroupIndex))
            {
                section.SectionTitle = $"{column.TableName} - {section.SectionType.GetDescriptionAttribute()}";
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

            for (int x = insertIndexStart; x < this.dataReportSections.Count; ++x)
            {
                this.dataReportSections[x].SectionIndex = shiftIndex;

                ++shiftIndex;
            }

            return result;
        }

        #endregion
    }
}
