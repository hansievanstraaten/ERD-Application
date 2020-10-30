using GeneralExtensions;
using REPORT.Builder.Common;
using REPORT.Builder.ReportTools;
using System;
using System.Text;
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
        }

        public void AddPageFooterObject(XElement item)
        {
            UIElement element = ObjectCreator.CreateReportObject(item, false);

            double sectionOffset = this.Height - (this.BottomMargin + this.FooterHeight);

            Canvas.SetLeft(element, element.GetPropertyValue("Left").ToDouble());

            Canvas.SetTop(element, (element.GetPropertyValue("Top").ToDouble() + sectionOffset));

            this.Children.Add(element);
        }

        public bool AddObject(XElement item, SectionTypeEnum sectionType, out double elementBottom, out string textOverflow)
        {
            UIElement element = ObjectCreator.CreateReportObject(item, false);

            double itemHeight = 0;

            if (item.IsDataObject())
            {
                this.MeasureDataElement(element, out itemHeight);
            }
            else
			{
                itemHeight = element.GetPropertyValue("TextHeight").ToDouble();
            }

            textOverflow = string.Empty;

            Canvas.SetLeft(element, element.GetPropertyValue("Left").ToDouble());

            Canvas.SetTop(element, (element.GetPropertyValue("Top").ToDouble() + this.TopOffset));

            this.Children.Add(element);

            elementBottom = Canvas.GetTop(element) + itemHeight;

            if (elementBottom > this.BottomOffset)
			{
                if (!this.TryTrimWrapedText(element, elementBottom, out textOverflow))
                {
                    this.Children.Remove(element);

                    elementBottom = 0;

                    return false;
                }
			}

            this.HaveElements = true;

            return true;
        }

        private bool TryTrimWrapedText(UIElement element, double itemHeight, out string textRemainder)
		{
            StringBuilder remainder = new StringBuilder();

            string elementText = element.GetPropertyValue("Text").ParseToString();

            remainder.Append(elementText);

            int trimCut = Convert.ToInt32(itemHeight - this.BottomOffset);

            int trimLenght = elementText.Length - trimCut;

            while (itemHeight > this.BottomOffset)
            {
                remainder.Remove(trimLenght, trimCut);

                element.SetPropertyValue("Text", remainder.ToString());

                this.MeasureDataElement(element, out itemHeight);

                itemHeight = Canvas.GetTop(element) + itemHeight;

                trimCut = Convert.ToInt32(itemHeight - this.BottomOffset);

                trimLenght -= trimCut;

                if (trimLenght <= 0)
				{
                    textRemainder = string.Empty;

                    return false;
				}
            }

            textRemainder = elementText.Replace(remainder.ToString(), string.Empty);

            return true;
        }

		public void MeasureDataElement(UIElement element, out double itemHeight)
		{
            ReportDataObject dataObj = element.To<ReportDataObject>();

            if (dataObj.TextWrapping == TextWrapping.NoWrap && dataObj.Width != double.NaN)
			{
                itemHeight = dataObj.GetPropertyValue("TextHeight").ToDouble();

                return;
			}

            Size size = dataObj.MeasureString();

            dataObj.MaxWidth = size.Width;

            dataObj.Measure(size);

            dataObj.Height = dataObj.DesiredSize.Height;

            dataObj.Arrange(new Rect(dataObj.DesiredSize));

            dataObj.Height = dataObj.ActualHeight;

            itemHeight = dataObj.Height;
		}
	}
}
