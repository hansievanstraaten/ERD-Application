using GeneralExtensions;
using REPORT.Builder.Common;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Builder.Printing
{
    [Serializable()]
    public class PrintCanvas : Canvas
    {
        public PrintCanvas()
        {
            this.Background = System.Windows.Media.Brushes.White;
        }

        public bool HaveElements { get; private set; }

        public double HeaderHeight { get; set; }

        public double FooterHeight { get; set; }

        public double TopMargin { get; set; }

        public double BottomMargin { get; set; }

        public double TopOffset { get; set; }

        public double BottomOffset
        {
            get
            {
                return this.Height - (this.BottomMargin + this.FooterHeight);
            }
        }

        public void AddPageHeaderObject(XElement item)
        {
            UIElement element = ObjectCreator.CreateReportObject(item, false);

            Canvas.SetLeft(element, element.GetPropertyValue("Left").ToDouble());

            Canvas.SetTop(element, (element.GetPropertyValue("Top").ToDouble() + this.TopMargin));

            this.Children.Add(element);

            double itemHeight = element.GetPropertyValue("TextHeight").ToDouble() + Canvas.GetTop(element);

            if (this.TopOffset < itemHeight)
            {
                this.TopOffset = itemHeight;
            }
        }

        public void AddPageFooterObject(XElement item)
        {
            UIElement element = ObjectCreator.CreateReportObject(item, false);

            double sectionOffset = this.Height - (this.BottomMargin + this.FooterHeight);

            Canvas.SetLeft(element, element.GetPropertyValue("Left").ToDouble());

            Canvas.SetTop(element, (element.GetPropertyValue("Top").ToDouble() + sectionOffset));

            this.Children.Add(element);
        }

        public bool AddObject(XElement item, SectionTypeEnum sectionType, out double elementBottom)
        {
            UIElement element = ObjectCreator.CreateReportObject(item, false);

            double itemHeight = element.GetPropertyValue("TextHeight").ToDouble();

            if (sectionType != SectionTypeEnum.Page)
            {
                if ((itemHeight + this.TopOffset) > this.BottomOffset)
                {
                    elementBottom = 0;

                    return false;
                }
            }

            Canvas.SetLeft(element, element.GetPropertyValue("Left").ToDouble());

            Canvas.SetTop(element, (element.GetPropertyValue("Top").ToDouble() + this.TopOffset));

            this.Children.Add(element);

            elementBottom = Canvas.GetTop(element) + itemHeight;

            this.HaveElements = true;

            return true;
        }
    }
}
