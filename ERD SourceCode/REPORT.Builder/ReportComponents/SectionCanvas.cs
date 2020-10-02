using ERD.Models;
using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Builder.ReportTools;
using REPORT.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.Functions;

namespace REPORT.Builder.ReportComponents
{
    public class SectionCanvas : Canvas
    {
        public delegate void RequestNewDataSectionsEvent(object sender, ReportColumnModel column);

        public delegate void ReportColumnAddedEvent(object sender, ReportColumnModel column);

        public delegate void ReportObjectSelectedEvent(object sender);

        public event RequestNewDataSectionsEvent RequestNewDataSections;

        public event ReportColumnAddedEvent ReportColumnAdded;

        public event ReportObjectSelectedEvent ReportObjectSelected;

        private bool isDragging;

        private bool isHandle;

        private Point startPoint;

        private Point selectedElementOrigins;

        private UIElement selectedElement;

        private CanvasSqlManager sqlManager = new CanvasSqlManager();

        private Dictionary<Guid, UIElement> selectedReportObjects = new Dictionary<Guid, UIElement>();

        public SectionCanvas()
        {
            base.AllowDrop = true;

            base.PreviewMouseLeftButtonUp += this.SectionCanvas_Seleced;
        }
        
        public XElement CanvasXml
        {
            get
            {
                XElement result = new XElement("CanvasXml");

                XElement objectModels = new XElement("ObjectModels");

                XElement parameterModels = new XElement("ParameterModels");

                XElement foreignGroupIndex = new XElement("ForeignGroupIndexes");

                foreach (UIElement child in this.Children)
                {
                    objectModels.Add(child.GetPropertyValue("ItemXml") as XElement);
                }

                foreach(WhereParameterModel parameter in this.SqlManager.WhereParameterModel)
                {
                    parameterModels.Add(parameter.ItemXml);
                }

                foreach(int foreignIndex in this.SqlManager.ForeignGroupIndexes)
                {
                    foreignGroupIndex.Add(new XElement("Index", foreignIndex));
                }

                result.Add(objectModels);

                result.Add(parameterModels);

                result.Add(foreignGroupIndex);

                return result;
            }

            set
            {
                foreach (XElement item in value.Element("ObjectModels").Elements())
                {
                    if (item.Name.LocalName == "ReportObject")
                    {
                        UIElement child = ObjectCreator.CreateReportObject(item, this.IsDesignMode);

                        this.AddReportToolItem(child);

                        if (child.GetType() == typeof(ReportDataObject))
                        {
                            this.SqlManager.AddColumn(child.GetPropertyValue("ColumnModel") as ReportColumnModel);
                        }
                    }
                    else
                    {
                        this.SetPropertyValue(item.Name.LocalName, item.Value);
                    }
                }

                List<WhereParameterModel> paramatersList = new List<WhereParameterModel>();

                foreach(XElement index in value.Element("ForeignGroupIndexes").Elements())
                {
                    this.SqlManager.AddForeignGroupIndex(index.Value.ToInt32());
                }

                foreach (XElement item in value.Element("ParameterModels").Elements())
                {
                    paramatersList.Add(new WhereParameterModel { ItemXml = item });

                }
                    
                this.SqlManager.AddWhereModels(paramatersList.ToArray());

                this.SectionTableName = this.SqlManager.TableName;
            }
        }

        public bool IsDesignMode { get; set; }

        public string SectionTableName { get; set; }

        public string SQLQuery
        {
            get
            {
                return this.SqlManager.SQLQuery;
            }
        }

        public List<ReportColumnModel> ReportColumns
        {
            get
            {
                return this.SqlManager.ReportColumns;
            }
        }

        public SectionTypeEnum SectionType { get; set; }

        public CanvasSqlManager SqlManager 
        { 
            get
            {
                return this.sqlManager;
            }

            private set
            {
                this.sqlManager = value;
            }
        }

        public void AddReportColumn(ReportColumnModel column)
        {
            UIElement toolObject = Activator.CreateInstance(typeof(ReportDataObject)) as UIElement;

            toolObject.SetPropertyValue("Top", column.DropY);

            toolObject.SetPropertyValue("Left", column.DropX);

            toolObject.SetPropertyValue("ColumnModel", column);

            this.SetEditControl(toolObject, false);

            if (this.SectionTableName.IsNullEmptyOrWhiteSpace())
            {
                this.SectionTableName = column.TableName;
            }

            this.SqlManager.AddColumn(column);
        }

        public void AlignmentObjects(ReportAlignmentEnum alignmentEnum)
        {
            if (this.selectedReportObjects.Count == 0)
            {
                return;
            }

            switch(alignmentEnum)
            {
                //case ReportAlignmentEnum.AlignBottom:
                    
                //    double bottom = Canvas.GetBottom(this.selectedReportObjects.Values.First());

                //    foreach(KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                //    {
                //        Canvas.SetBottom(itemKeyPair.Value, bottom);
                //    }

                //    break;

                case ReportAlignmentEnum.AlignLeft:

                    double left = Canvas.GetLeft(this.selectedReportObjects.Values.First());

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                    {
                        Canvas.SetLeft(itemKeyPair.Value, left);

                        itemKeyPair.Value.MoveHandles();
                    }

                    break;

                //case ReportAlignmentEnum.AlignRight:

                //    double right = Canvas.GetRight(this.selectedReportObjects.Values.First());

                //    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                //    {
                //        Canvas.SetRight(itemKeyPair.Value, right);
                //    }

                //    break;

                case ReportAlignmentEnum.AlignTop:

                    double top = Canvas.GetTop(this.selectedReportObjects.Values.First());

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                    {
                        Canvas.SetTop(itemKeyPair.Value, top);
                    }

                    break;

                case ReportAlignmentEnum.SameHeight:

                    double heigh = this.selectedReportObjects.Values.First().GetPropertyValue("ActualHeight").ToDouble();

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                    {
                        itemKeyPair.Value.SetPropertyValue("Height", heigh);
                    }

                    break;

                case ReportAlignmentEnum.SameWidth:

                    double width = this.selectedReportObjects.Values.First().GetPropertyValue("ActualWidth").ToDouble();

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                    {
                        itemKeyPair.Value.SetPropertyValue("Width", width);
                    }

                    break;
            }

            foreach (KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
            {
                itemKeyPair.Value.MoveHandles();
            }
        }

        public void RemoveElementHandles()
        {
            foreach(UIElement item in this.selectedReportObjects.Values)
            {
                item.RemoveHandles(this);
            }

            this.selectedReportObjects.Clear();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (e.Source != this)
            {
                if (!this.isDragging)
                {
                    this.startPoint = e.GetPosition(this);

                    UIElement toolObject = null;
                                        
                    if (e.Source.GetType() == typeof(System.Windows.Shapes.Line))
                    {
                        toolObject = ((System.Windows.Shapes.Line)e.Source as System.Windows.Shapes.Line).Parent as UIElement;
                    }
                    else
                    {
                        toolObject = e.Source as UIElement;
                    }

                    this.selectedElementOrigins = new Point(Canvas.GetLeft(toolObject), Canvas.GetTop(toolObject));

                    this.isDragging = true;

                    //if (e.Source.GetType() != typeof(ResizeHandle))
                    //{
                    //}
                    //else
                    //{
                    //    this.selectedElement = toolObject;
                    //}
                        
                    this.SetEditControl(toolObject, true);

                    this.CaptureMouse();
                }

                e.Handled = true;
            }
            else
            {
                this.isDragging = false;

                this.isHandle = false;
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (this.IsMouseCaptured)
            {
                this.isDragging = false;

                this.isHandle = false;

                this.ReleaseMouseCapture();

                e.Handled = true;
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.IsMouseCaptured)
            {
                if (this.isDragging)
                {
                    Point currentPosition = e.GetPosition(this);

                    double elementWidth = this.selectedElement.GetPropertyValue("ActualWidth").ToDouble();

                    double elementHeigth = this.selectedElement.GetPropertyValue("ActualHeight").ToDouble();

                    double elementLeft = (currentPosition.X - this.startPoint.X) + this.selectedElementOrigins.X;

                    double elementTop = (currentPosition.Y - this.startPoint.Y) + this.selectedElementOrigins.Y;

                    if (elementLeft < 0)
                    {
                        elementLeft = this.isHandle ? ResizeHandles.HandleSize * -1 : 0;
                    }

                    if (elementTop < 0)
                    {
                        elementTop = this.isHandle ? ResizeHandles.HandleSize * -1 : 0;
                    }

                    if (base.ActualWidth > (elementLeft + elementWidth))
                    {
                        Canvas.SetLeft(this.selectedElement, elementLeft);
                    }

                    if (base.ActualHeight > (elementTop + elementHeigth))
                    {
                        Canvas.SetTop(this.selectedElement, elementTop);
                    }

                    if (this.isHandle)
                    {
                        this.selectedElement.ResizeElement(elementTop, elementLeft);
                    }
                    else
                    {
                        this.selectedElement.MoveHandles();
                    }
                }
            }
        }
                
        protected override void OnDragOver(DragEventArgs e)
        {
            e.Handled = true;

            base.OnDragOver(e);

            if (e.Effects == DragDropEffects.Move)
            {
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    object dragItem = e.Data.GetData(typeof(ToolsMenuItem));

                    if (dragItem == null)
                    {
                        return;
                    }

                    object dataObject = ((ToolsMenuItem)dragItem).Tag;

                    if (dataObject != null && this.SectionType != SectionTypeEnum.TableData)
                    {
                        e.Effects = DragDropEffects.None;

                        return;
                    }

                    if (e.KeyStates.HasFlag(DragDropKeyStates.ControlKey))
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            e.Handled = true;

            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                #region REPORT TOOL ITEM DROPED

                ToolsMenuItem dropedItem = e.Data.GetData(typeof(ToolsMenuItem)) as ToolsMenuItem;
                
                Point location = e.GetPosition(this);

                this.AddReportToolItem(dropedItem, location);                

                #endregion
            }
            else if (e.Data.GetDataPresent(DataFormats.Serializable))
            {
                #region REMOVE MOVE ON CANVAS

                object[] dataValues = (object[])e.Data.GetData(DataFormats.Serializable);

                #endregion
            }

            base.OnDrop(e);
        }

        private void SectionCanvas_Seleced(object sender, MouseButtonEventArgs e)
        {
            this.RemoveElementHandles();

            this.selectedElement = null;

            this.ReportObjectSelected?.Invoke(null);
        }

        private void SetEditControl(UIElement toolObject, bool isDragObject)
        {
            if (toolObject.GetType() == typeof(ResizeHandle))
            {
                this.isHandle = true;

                this.selectedElement = toolObject;

                return;
            }

            if (!KeyActions.IsCtrlPressed)
            {
                this.RemoveElementHandles();
            }

            this.selectedElement = toolObject;

            this.selectedElement.ShowHandles(this);

            if (!isDragObject)
            {
                this.AddReportToolItem(toolObject);
            }

            Guid elementId = toolObject.GetElementId();

            if (!this.selectedReportObjects.ContainsKey(elementId))
            {
                this.selectedReportObjects.Add(elementId, toolObject);
            }

            this.ReportObjectSelected?.Invoke(toolObject);
        }

        private void AddReportToolItem(ToolsMenuItem toolItem, Point location)
        {
            UIElement toolObject = Activator.CreateInstance(toolItem.ToolType) as UIElement;

            if (toolItem.ToolType == typeof(ReportDataObject) && this.SectionType == SectionTypeEnum.TableData)
            {
                ReportColumnModel column = toolItem.Tag.To<ReportColumnModel>();
                
                column.DropX = location.X;

                column.DropY = location.Y;

                if (!this.MayAddColumn(column))
                {
                    return;
                }

                toolObject.SetPropertyValue("ColumnModel", column);
            }

            toolObject.SetPropertyValue("Top", location.Y);

            toolObject.SetPropertyValue("Left", location.X);

            this.SetEditControl(toolObject, false);
        }

        private void AddReportToolItem(UIElement toolObject)
        {
            Guid elementId = toolObject.GetElementId();

            toolObject.SetPropertyValue("IsDesignMode", this.IsDesignMode);

            this.Children.Add(toolObject as UIElement);
        }
    
        private bool MayAddColumn(ReportColumnModel column)
        {
            if (!this.SectionTableName.IsNullEmptyOrWhiteSpace() && column.TableName != this.SectionTableName)
            {
                this.RequestNewDataSections?.Invoke(this, column);

                return false;
            }

            this.SectionTableName = column.TableName;

            this.SqlManager.AddColumn(column);

            this.ReportColumnAdded?.Invoke(this, column);

            return true;
        }    
    }
}
