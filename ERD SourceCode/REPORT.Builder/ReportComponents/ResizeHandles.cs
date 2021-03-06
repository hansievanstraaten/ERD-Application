﻿using GeneralExtensions;
using REPORT.Builder.ReportTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.Exstention;
using WPF.Tools.Mesurements;

namespace REPORT.Builder.ReportComponents
{
    public static class ResizeHandles
    {
        public static readonly double HandleSize = 10;

        private static double minSizeParameter = 10;

        private static double top;

        private static double left;

        private static double bottom;

        private static double right;

        private static Dictionary<KeyValuePair<Guid, ResizeHandlesEnum>, ResizeHandle> elementHandles = new Dictionary<KeyValuePair<Guid, ResizeHandlesEnum>, ResizeHandle>();

        private static Dictionary<Guid, UIElement> canvaselements = new Dictionary<Guid, UIElement>();

        public static UIElement SelectedElement { get; set; }

        public static int SelectedElementCount 
        { 
            get
            {
                return canvaselements.Count;
            }
        }
                
        public static void NotchLeft(double distance)
        {
            foreach (UIElement item in canvaselements.Values)
            {
                double notchPlacement = Canvas.GetLeft(item) - distance;

                if (notchPlacement < 0)
                {
                    notchPlacement = 0;
                }

                Canvas.SetLeft(item, notchPlacement);

                item.MoveHandles();
            }
        }

        public static void NotchUp(double distance)
        {
            foreach (UIElement item in canvaselements.Values)
            {
                double notchPlacement = Canvas.GetTop(item) - distance;

                if (notchPlacement < 0)
                {
                    notchPlacement = 0;
                }

                Canvas.SetTop(item, notchPlacement);

                item.MoveHandles();
            }
        }

        public static void NotchRight(double distance)
        {
            foreach (UIElement item in canvaselements.Values)
            {
                double notchPlacement = Canvas.GetLeft(item) + distance;

                SectionCanvas parent = item.FindParentControlBase(typeof(SectionCanvas)) as SectionCanvas;

                double itemWidth = item.GetPropertyValue("ActualWidth").ToDouble();

                if ((notchPlacement + itemWidth) > parent.ActualWidth)
                {
                    notchPlacement = parent.ActualWidth - itemWidth;
                }

                Canvas.SetLeft(item, notchPlacement);

                item.MoveHandles();
            }
        }

        public static void NotchDown(double distance)
        {
            foreach (UIElement item in canvaselements.Values)
            {
                double notchPlacement = Canvas.GetTop(item) + distance;

                SectionCanvas parent = item.FindParentControlBase(typeof(SectionCanvas)) as SectionCanvas;

                double itemHeight = item.GetPropertyValue("ActualHeight").ToDouble();

                if ((notchPlacement + itemHeight) > parent.ActualHeight)
                {
                    notchPlacement = parent.ActualHeight - itemHeight;
                }

                Canvas.SetTop(item, notchPlacement);

                item.MoveHandles();
            }
        }

        public static void MoveObject(double canvasWidth, double canvasHeight, double elementLeft, double elementWidth, double elementTop, double elementHeigth)
        {
            if (canvasWidth > (elementLeft + elementWidth))
            {
                Canvas.SetLeft(ResizeHandles.SelectedElement, elementLeft);
            }

            if (canvasHeight > (elementTop + elementHeigth))
            {
                Canvas.SetTop(ResizeHandles.SelectedElement, elementTop);
            }
        }

        public static void MoveObjects(double canvasWidth, 
            double canvasHeight, 
            double elementLeft, 
            double elementWidth, 
            double elementTop, 
            double elementHeigth,
            double offsetLeft,
            double offsetTop
            )
        {
            ResizeHandles.MoveObject(canvasWidth, canvasHeight, elementLeft, elementWidth, elementTop, elementHeigth);

            ResizeHandles.SelectedElement.MoveHandles();

            Guid elementId = SelectedElement.GetElementId();

            foreach (UIElement item in canvaselements.Values)
            {
                if (elementId == item.GetElementId())
                {
                    continue;
                }

                SectionCanvas itemCanvas = item.FindParentControlBase(typeof(SectionCanvas)) as SectionCanvas;

                double canvasItemWidth = ResizeHandles.SelectedElement.GetPropertyValue("ActualWidth").ToDouble();

                double canvasItemHeigth = ResizeHandles.SelectedElement.GetPropertyValue("ActualHeight").ToDouble();

                double canvasItemLeft = Canvas.GetLeft(item) + offsetLeft;

                double canvasItemTop = Canvas.GetTop(item) + offsetTop;

                if (canvasItemLeft <= 0 
                    || canvasItemTop <= 0
                    || (canvasItemLeft + canvasItemWidth) > itemCanvas.ActualWidth
                    || (canvasItemTop + canvasItemHeigth) > itemCanvas.ActualHeight)
                {
                    continue;
                }

                Canvas.SetLeft(item, canvasItemLeft);

                Canvas.SetTop(item, canvasItemTop);

                item.MoveHandles();
            }
        }

        public static void RemoveHandles()
        {
            foreach(UIElement element in canvaselements.Values)
            {
                element.RemoveHandles();
            }

            canvaselements.Clear();
        }

        public static void RemoveHandles(this UIElement element)
        {
            if (element == null || element.GetType() == typeof(ResizeHandle))
            {
                return;
            }

            Guid elementId = element.GetElementId();

            SectionCanvas canvas = element.FindParentControlBase(typeof(SectionCanvas)) as SectionCanvas;

            foreach (ResizeHandlesEnum item in Enum.GetValues(typeof(ResizeHandlesEnum)))
            {
                KeyValuePair<Guid, ResizeHandlesEnum> key = new KeyValuePair<Guid, ResizeHandlesEnum>(elementId, item);

                if (!elementHandles.ContainsKey(key))
                {
                    continue;
                }

                canvas.Children.Remove(elementHandles[key]);

                elementHandles.Remove(key);
            }
        }

        public static void ShowHandles(this UIElement element, SectionCanvas canvas)
        {
            if (element.GetType() == typeof(ResizeHandle))
            {
                return;
            }

            Guid elementId = element.GetElementId();

            if (HaveHandles(elementId))
            {
                return;
            }

            canvaselements.Add(elementId, element);

            CreateHandles(elementId);

            SetBounds(element);
                        
            foreach (KeyValuePair<KeyValuePair<Guid, ResizeHandlesEnum>, ResizeHandle> hanle in elementHandles
                .Where(k => k.Key.Key == elementId &&
                            k.Key.Value != ResizeHandlesEnum.None))
            {
                switch(hanle.Key.Value)
                {
                    case ResizeHandlesEnum.LeftBottom:
                        
                        Canvas.SetTop(hanle.Value, bottom);

                        Canvas.SetLeft(hanle.Value, left);


                        break;

                    case ResizeHandlesEnum.LetfTop:

                        Canvas.SetTop(hanle.Value, top);

                        Canvas.SetLeft(hanle.Value, left);

                        break;

                    case ResizeHandlesEnum.RightBottom:

                        Canvas.SetTop(hanle.Value, bottom);

                        Canvas.SetLeft(hanle.Value, right);

                        break;

                    case ResizeHandlesEnum.RightTop:

                        Canvas.SetTop(hanle.Value, top);

                        Canvas.SetLeft(hanle.Value, right);

                        break;
                }

                canvas.Children.Add(hanle.Value);
            }  
        }

        public static void MoveHandles(this UIElement element, ResizeHandlesEnum except = ResizeHandlesEnum.None)
        {
            SetBounds(element);

            Guid elementId = element.GetElementId();

            foreach (KeyValuePair<KeyValuePair<Guid, ResizeHandlesEnum>, ResizeHandle> hanle in elementHandles
                .Where(k => k.Key.Key == elementId && k.Key.Value != except))
            {
                switch (hanle.Key.Value)
                {
                    case ResizeHandlesEnum.LeftBottom:

                        Canvas.SetTop(hanle.Value, bottom);

                        Canvas.SetLeft(hanle.Value, left);


                        break;

                    case ResizeHandlesEnum.LetfTop:

                        Canvas.SetTop(hanle.Value, top);

                        Canvas.SetLeft(hanle.Value, left);

                        break;

                    case ResizeHandlesEnum.RightBottom:

                        Canvas.SetTop(hanle.Value, bottom);

                        Canvas.SetLeft(hanle.Value, right);

                        break;

                    case ResizeHandlesEnum.RightTop:

                        Canvas.SetTop(hanle.Value, top);

                        Canvas.SetLeft(hanle.Value, right);

                        break;
                }
            }
        }

        public static void ResizeElements(this UIElement element, double handleTop, double handleLeft)
        {
            Guid elementId = element.GetElementId();

            element.ResizeElement(handleTop, handleLeft);

            UIElement canvasElement = canvaselements[elementId];

            ResizeHandle handle = (ResizeHandle)element;

            double width = canvasElement.GetPropertyValue("Width").ToDouble();

            double height = canvasElement.GetPropertyValue("Height").ToDouble();

            foreach (UIElement item in canvaselements.Values.Where(c => c.GetElementId() != elementId))
            {
                if (handle.ResizeHandleType == ResizeHandlesEnum.RightTop)
                {
                    double topOffset = item.GetPropertyValue("ActualHeight").ToDouble() - height;

                    Canvas.SetTop(item, (Canvas.GetTop(item) + topOffset));
                }
                else if (handle.ResizeHandleType == ResizeHandlesEnum.LetfTop)
                {
                    double topOffset = item.GetPropertyValue("ActualHeight").ToDouble() - height;

                    double leftOffset = item.GetPropertyValue("ActualWidth").ToDouble() - width;

                    Canvas.SetTop(item, (Canvas.GetTop(item) + topOffset));

                    Canvas.SetLeft(item, (Canvas.GetLeft(item) + leftOffset));
                }
                else if (handle.ResizeHandleType == ResizeHandlesEnum.LeftBottom)
                {
                    double leftOffset = item.GetPropertyValue("ActualWidth").ToDouble() - width;

                    Canvas.SetLeft(item, (Canvas.GetLeft(item) + leftOffset));
                }

                    item.SetPropertyValue("Width", width);

                item.SetPropertyValue("Height", height);

                //Canvas.SetTop(item, (handleTop - HandleSize));

                //Canvas.SetLeft(item, (handleLeft - HandleSize));

                item.MoveHandles();
            }
        }

        public static void ResizeElement(this UIElement element, double handleTop, double handleLeft)
        {
            if (element.GetType() != typeof(ResizeHandle))
            {
                return;
            }

            ResizeHandle handle = (ResizeHandle)element;

            UIElement canvasElement = canvaselements[handle.ElemntId];

            SetBounds(canvasElement);

            if (canvasElement.GetType().BaseType == typeof(ReportLineBase))
            {
                minSizeParameter = 1;

                switch (handle.ResizeHandleType)
                {
                    case ResizeHandlesEnum.LeftBottom:

                        canvasElement.SetPropertyValue("StrokeThickness", CalculateBottomHeight(handleTop));

                        canvasElement.SetPropertyValue("Height", CalculateBottomHeight(handleTop));

                        canvasElement.SetPropertyValue("Width", CalculateLeftWidth(handleLeft));

                        canvasElement.SetPropertyValue("LineLength", DistanceConverter.ConvertPixelToCm(CalculateLeftWidth(handleLeft)));

                        Canvas.SetLeft(canvasElement, (handleLeft + HandleSize));

                        break;

                    case ResizeHandlesEnum.LetfTop:

                        double leftTophicknes = (bottom - handleTop) - HandleSize;

                        if (leftTophicknes < 1)
                        {
                            leftTophicknes = 1;
                        }

                        canvasElement.SetPropertyValue("StrokeThickness", leftTophicknes);

                        Canvas.SetTop(canvasElement, (handleTop + HandleSize));

                        canvasElement.SetPropertyValue("Height", leftTophicknes);

                        canvasElement.SetPropertyValue("Width", CalculateLeftWidth(handleLeft));

                        canvasElement.SetPropertyValue("LineLength", DistanceConverter.ConvertPixelToCm(CalculateLeftWidth(handleLeft)));

                        Canvas.SetLeft(canvasElement, (handleLeft + HandleSize));

                        break;

                    case ResizeHandlesEnum.RightBottom:

                        canvasElement.SetPropertyValue("StrokeThickness", CalculateBottomHeight(handleTop));

                        canvasElement.SetPropertyValue("Height", CalculateBottomHeight(handleTop));

                        canvasElement.SetPropertyValue("Width", CalculateRightWidth(handleLeft));

                        canvasElement.SetPropertyValue("LineLength", DistanceConverter.ConvertPixelToCm(CalculateRightWidth(handleLeft)));

                        break;

                    case ResizeHandlesEnum.RightTop:

                        double rightTopThicknes = (bottom - handleTop) - HandleSize;

                        if (rightTopThicknes < 1)
                        {
                            rightTopThicknes = 1;
                        }

                        canvasElement.SetPropertyValue("StrokeThickness", rightTopThicknes);

                        Canvas.SetTop(canvasElement, (handleTop + HandleSize));

                        canvasElement.SetPropertyValue("Height", rightTopThicknes);

                        canvasElement.SetPropertyValue("Width", CalculateRightWidth(handleLeft));

                        canvasElement.SetPropertyValue("LineLength", DistanceConverter.ConvertPixelToCm(CalculateRightWidth(handleLeft)));

                        break;
                }
            }
            else
            {
                minSizeParameter = 10;

                switch (handle.ResizeHandleType)
                {
                    case ResizeHandlesEnum.LeftBottom:

                        canvasElement.SetPropertyValue("Height", CalculateBottomHeight(handleTop));

                        canvasElement.SetPropertyValue("Width", CalculateLeftWidth(handleLeft));

                        Canvas.SetLeft(canvasElement, (handleLeft + HandleSize));

                        break;

                    case ResizeHandlesEnum.LetfTop:

                        canvasElement.SetPropertyValue("Height", CalculateTopHeight(handleTop));

                        Canvas.SetTop(canvasElement, (handleTop + HandleSize));

                        canvasElement.SetPropertyValue("Width", CalculateLeftWidth(handleLeft));

                        Canvas.SetLeft(canvasElement, (handleLeft + HandleSize));

                        break;

                    case ResizeHandlesEnum.RightBottom:

                        canvasElement.SetPropertyValue("Height", CalculateBottomHeight(handleTop));

                        canvasElement.SetPropertyValue("Width", CalculateRightWidth(handleLeft));

                        break;

                    case ResizeHandlesEnum.RightTop:

                        canvasElement.SetPropertyValue("Height", CalculateTopHeight(handleTop));

                        Canvas.SetTop(canvasElement, (handleTop + HandleSize));

                        canvasElement.SetPropertyValue("Width", CalculateRightWidth(handleLeft));

                        break;
                }
            }

            MoveHandles(canvasElement, handle.ResizeHandleType);
        }

        public static Guid GetElementId(this UIElement element)
        {
            return Guid.Parse(element.GetPropertyValue("ElemntId").ParseToString());
        }
                
        public static void RemoveElement(UIElement element)
        {
            Guid elementId = element.GetElementId();

            element.RemoveHandles();

            canvaselements.Remove(elementId);
        }

        public static void AlignmentObjects(ReportAlignmentEnum alignmentEnum)
        {
            if (canvaselements.Count == 0)
            {
                return;
            }

            switch (alignmentEnum)
            {
                //case ReportAlignmentEnum.AlignBottom:

                //    double bottom = Canvas.GetBottom(this.selectedReportObjects.Values.First());

                //    foreach(KeyValuePair<Guid, UIElement> itemKeyPair in this.selectedReportObjects)
                //    {
                //        Canvas.SetBottom(itemKeyPair.Value, bottom);
                //    }

                //    break;

                case ReportAlignmentEnum.AlignLeft:

                    double left = Canvas.GetLeft(canvaselements.Values.First());

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in canvaselements)
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

                    double top = Canvas.GetTop(canvaselements.Values.First());

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in canvaselements)
                    {
                        Canvas.SetTop(itemKeyPair.Value, top);
                    }

                    break;

                case ReportAlignmentEnum.SameHeight:

                    double heigh = canvaselements.Values.First().GetPropertyValue("ActualHeight").ToDouble();

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in canvaselements)
                    {
                        itemKeyPair.Value.SetPropertyValue("Height", heigh);
                    }

                    break;

                case ReportAlignmentEnum.SameWidth:

                    double width = canvaselements.Values.First().GetPropertyValue("ActualWidth").ToDouble();

                    foreach (KeyValuePair<Guid, UIElement> itemKeyPair in canvaselements)
                    {
                        itemKeyPair.Value.SetPropertyValue("Width", width);
                    }

                    break;
            }

            foreach (KeyValuePair<Guid, UIElement> itemKeyPair in canvaselements)
            {
                itemKeyPair.Value.MoveHandles();
            }
        }

        private static double CalculateRightWidth(double handleLeft)
        {
            double width = (right - left) + (handleLeft - right) - HandleSize;

            if (width < minSizeParameter)
            {
                return minSizeParameter;
            }

            return width;
        }

        private static double CalculateLeftWidth(double handleLeft)
        {
            double width = (right - handleLeft) - HandleSize;

            if (width < minSizeParameter)
            {
                return minSizeParameter;
            }

            return width;
        }

        private static double CalculateTopHeight(double handleTop)
        {
            double height = (bottom - handleTop) - HandleSize;

            if (height < minSizeParameter)
            {
                return minSizeParameter;
            }

            return height;
        }

        private static double CalculateBottomHeight(double handleTop)
        {
            double height = (bottom - top) + (handleTop - bottom) - HandleSize;

            if (height < minSizeParameter)
            {
                return minSizeParameter;
            }

            return height;
        }

        private static void SetBounds(UIElement element)
        {
            top = Canvas.GetTop(element) - HandleSize;

            left = Canvas.GetLeft(element) - HandleSize;

            bottom = element.DesiredSize.Height + top + HandleSize;

            right = element.DesiredSize.Width + left + HandleSize;
        }

        private static bool HaveHandles(Guid elementId)
        {
            KeyValuePair<Guid, ResizeHandlesEnum> key = new KeyValuePair<Guid, ResizeHandlesEnum>(elementId, ResizeHandlesEnum.LeftBottom);

            return elementHandles.ContainsKey(key);
        }

        private static void CreateHandles(Guid elementId)
        {
            foreach (ResizeHandlesEnum item in Enum.GetValues(typeof(ResizeHandlesEnum)))
            {
                if (item == ResizeHandlesEnum.None)
                {
                    continue;
                }

                ResizeHandle thumb = new ResizeHandle();

                thumb.Width = HandleSize;
                thumb.Height = HandleSize;
                thumb.Background = Brushes.LightBlue;
                thumb.ElemntId = elementId;
                thumb.ResizeHandleType = item;

                KeyValuePair<Guid, ResizeHandlesEnum> key = new KeyValuePair<Guid, ResizeHandlesEnum>(elementId, item);

                elementHandles.Add(key, thumb);
            }
        }
    }

    public enum ResizeHandlesEnum
    {
        None,
        LetfTop,
        RightTop,
        RightBottom,
        LeftBottom
    }
}
