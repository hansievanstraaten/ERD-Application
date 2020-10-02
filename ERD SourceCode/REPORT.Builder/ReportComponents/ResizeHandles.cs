using GeneralExtensions;
using REPORT.Builder.ReportTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace REPORT.Builder.ReportComponents
{
    public static class ResizeHandles
    {
        public static readonly double HandleSize = 10;

        private static double top;

        private static double left;

        private static double bottom;

        private static double right;

        private static Dictionary<KeyValuePair<Guid, ResizeHandlesEnum>, ResizeHandle> elementHandles = new Dictionary<KeyValuePair<Guid, ResizeHandlesEnum>, ResizeHandle>();

        private static Dictionary<Guid, UIElement> canvaselements = new Dictionary<Guid, UIElement>();

        public static Guid GetElementId(this UIElement element)
        {
            return Guid.Parse(element.GetPropertyValue("ElemntId").ParseToString());
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

        public static void RemoveHandles(this UIElement element, SectionCanvas canvas)
        {
            if (element == null || element.GetType() == typeof(ResizeHandle))
            {
                return;
            }

            Guid elementId = element.GetElementId();

            canvaselements.Remove(elementId);

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

        public static void ResizeElement(this UIElement element, double handleTop, double handleLeft)
        {
            if (element.GetType() != typeof(ResizeHandle))
            {
                return;
            }

            ResizeHandle handle = (ResizeHandle)element;

            UIElement canvasElement = canvaselements[handle.ElemntId];

            SetBounds(canvasElement);

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

            MoveHandles(canvasElement, handle.ResizeHandleType);
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

        private static double CalculateRightWidth(double handleLeft)
        {
            double width = (right - left) + (handleLeft - right) - HandleSize;

            if (width < 10)
            {
                return 10;
            }

            return width;
        }

        private static double CalculateLeftWidth(double handleLeft)
        {
            double width = (right - handleLeft) - HandleSize;

            if (width < 10)
            {
                return 10;
            }

            return width;
        }

        private static double CalculateTopHeight(double handleTop)
        {
            double height = (bottom - handleTop) - HandleSize;

            if (height < 10)
            {
                return 10;
            }

            return height;
        }

        private static double CalculateBottomHeight(double handleTop)
        {
            double height = (bottom - top) + (handleTop - bottom) - HandleSize;

            if (height < 10)
            {
                return 10;
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
                thumb.Background = Brushes.Orange;
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
