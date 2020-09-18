﻿using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Builder.ReportTools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace REPORT.Builder.ReportComponents
{
    public class SectionCanvas : Canvas
    {
        public delegate void ReportObjectSelectedEvent(object sender);

        public event ReportObjectSelectedEvent ReportObjectSelected;

        private bool isDragging;

        private Point startPoint;

        private Point selectedElementOrigins;

        private UIElement selectedElement;

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

                foreach(UIElement child in this.Children)
                {
                    result.Add(child.GetPropertyValue("ItemXml") as XElement);
                }

                return result;
            }

            set
            {
                foreach (XElement item in value.Elements())
                {
                    if (item.Name.LocalName == "ReportObject")
                    {
                        UIElement child = ObjectCreator.CreateReportObject(item);

                        this.AddReportToolItem(child);
                    }
                    else
                    {
                        this.SetPropertyValue(item.Name.LocalName, item.Value);
                    }
                }                
            }
        }

        public bool IsDesignMode { get; set; }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (e.Source != this)
            {
                if (!this.isDragging)
                {
                    this.startPoint = e.GetPosition(this);

                    if (e.Source.GetType() == typeof(System.Windows.Shapes.Line))
                    {
                        this.selectedElement = ((System.Windows.Shapes.Line)e.Source as System.Windows.Shapes.Line).Parent as UIElement;
                    }
                    else
                    {
                        this.selectedElement = e.Source as UIElement;
                    }

                    this.selectedElementOrigins = new Point(Canvas.GetLeft(this.selectedElement), Canvas.GetTop(this.selectedElement));

                    this.isDragging = true;

                    this.CaptureMouse();

                    this.ReportObjectSelected?.Invoke(this.selectedElement);
                }

                e.Handled = true;
            }
            else
            {
                this.isDragging = false;
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (this.IsMouseCaptured)
            {
                this.isDragging = false;

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
                        elementLeft = 0;
                    }

                    if (elementTop < 0)
                    {
                        elementTop = 0;
                    }

                    if (base.ActualWidth > (elementLeft + elementWidth))
                    {
                        Canvas.SetLeft(this.selectedElement, elementLeft);
                    }

                    if (base.ActualHeight > (elementTop + elementHeigth))
                    {
                        Canvas.SetTop(this.selectedElement, elementTop);
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
                    object dataValue = e.Data.GetData(typeof(UIElement));

                    //if (dataValue == null || dataValue.GetType() != typeof(TableModel))
                    //{
                    //    return;
                    //}

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

        private void ReportObject_Selected(object sender, MouseButtonEventArgs e)
        {
            this.ReportObjectSelected?.Invoke(sender);
        }

        private void SectionCanvas_Seleced(object sender, MouseButtonEventArgs e)
        {
            this.ReportObjectSelected?.Invoke(null);
        }

        private void AddReportToolItem(ToolsMenuItem toolItem, Point location)
        {
            UIElement toolObject = Activator.CreateInstance(toolItem.ToolType) as UIElement;

            toolObject.SetPropertyValue("Top", location.Y);

            toolObject.SetPropertyValue("Left", location.X);

            this.AddReportToolItem(toolObject);

            this.ReportObjectSelected?.Invoke(toolObject);
        }

        private void AddReportToolItem(UIElement toolObject)
        {
            toolObject.MouseLeftButtonUp += this.ReportObject_Selected;

            toolObject.SetPropertyValue("IsDesignMode", this.IsDesignMode);

            this.Children.Add(toolObject as UIElement);
        }
    }
}
