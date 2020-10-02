using GeneralExtensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.Mesurements;

namespace REPORT.Builder.ReportTools
{
    public class ReportLineBase : ContentControl // Label
    {
        private Color stroke;

        private bool isHorizontal = true;

        private double lineLengthVal = 50;

        public ReportLineBase()
        {
            this.ElemntId = Guid.NewGuid();

            this.DataContext = this;

            this.Uid = Guid.NewGuid().ToString();

            this.Initialize();
        }

        public Guid ElemntId { get; private set; }

        public XElement ItemXml
        {
            get
            {
                XElement result = new XElement("ReportObject");

                result.Add(new XAttribute("Top", this.Top));
                result.Add(new XAttribute("Left", this.Left));
                result.Add(new XAttribute("Stroke", this.Stroke));
                result.Add(new XAttribute("StrokeThickness", this.StrokeThickness));
                result.Add(new XAttribute("LineLength", this.LineLength));

                //result.Add(new XAttribute("CanGrow", this.CanGrow));

                //result.Add(new XAttribute("X1", this.GetPropertyValue("X1")));

                //result.Add(new XAttribute("X2", this.GetPropertyValue("X2")));

                //result.Add(new XAttribute("Y1", this.GetPropertyValue("Y1")));

                //result.Add(new XAttribute("Y2", this.GetPropertyValue("Y2")));

                return result;
            }

            set
            {
                foreach (XAttribute item in value.Attributes())
                {
                    this.SetPropertyValue(item.Name.LocalName, item.Value);
                }
            }
        }
        
        public Line ReportLine
        {
            get;

            set;
        }

        [FieldInformationAttribute("Stroke Thickness", Sort = 2)]
        public double StrokeThickness
        {
            get
            {
                return this.ReportLine.StrokeThickness;
            }

            set
            {
                this.ReportLine.StrokeThickness = value;
            }
        }

        [FieldInformationAttribute("Stroke", Sort = 1)]
        public Color Stroke
        {
            get
            {
                return this.stroke;
            }

            set
            {
                this.stroke = value;

                this.ReportLine.Stroke = new SolidColorBrush(value);
            }
        }

        public bool IsHorizontal
        {
            get
            {
                if (this.X2 <= 0)
                {
                    return false;
                }

                return true;
            }

            set
            {
                this.isHorizontal = value;

                if (value)
                {
                    this.SetHorizontal();

                    return;
                }

                this.SetVertical();
            }
        }

        [FieldInformationAttribute("Length", Sort = 3)]
        public double LineLength
        {
            get
            {
                if (this.IsHorizontal)
                {
                    if (this.X2 <= 0)
                    {
                        return DistanceConverter.ConvertPixelToCm(this.lineLengthVal);
                    }

                    return DistanceConverter.ConvertPixelToCm(this.X2);
                }

                if (this.Y2 <= 0)
                {
                    return DistanceConverter.ConvertPixelToCm(this.lineLengthVal);
                }

                return DistanceConverter.ConvertPixelToCm(this.Y2);
            }

            set
            {
                if (value <= 0.3)
                {
                    value = 0.3;
                }

                this.lineLengthVal = DistanceConverter.ConvertCmToPixel(value);

                if (this.IsHorizontal)
                {
                    this.X2 = DistanceConverter.ConvertCmToPixel(value);

                    this.Y2 = 0;

                    return;
                }

                this.X2 = 0;

                this.Y2 = DistanceConverter.ConvertCmToPixel(value);
            }
        }

        public bool CanGrow
        {
            get;

            set;
        }

        public double X1
        {
            get
            {
                return this.ReportLine.X1;
            }

            set
            {
                this.ReportLine.X1 = value;
            }
        }

        public double X2
        {
            get
            {
                return this.ReportLine.X2;
            }

            set
            {
                this.ReportLine.X2 = value;
            }
        }

        public double Y1
        {
            get
            {
                return this.ReportLine.Y1;
            }

            set
            {
                this.ReportLine.Y1 = value;
            }
        }

        public double Y2
        {
            get
            {
                return this.ReportLine.Y2;
            }

            set
            {
                this.ReportLine.Y2 = value;
            }
        }

        public double Bottom
        {
            get
            {
                double result = Canvas.GetBottom(this);

                if (double.IsNaN(result))
                {
                    if (this.isHorizontal)
                    {
                        result = this.Top + this.StrokeThickness;
                    }
                    else
                    {
                        result = this.Top + this.LineLength;
                    }
                }

                return result;
            }
        }

        [FieldInformationAttribute("Top", Sort = 4)]
        public double Top
        {
            get
            {
                return Canvas.GetTop(this);
            }

            set
            {
                Canvas.SetTop(this, value);
            }
        }

        [FieldInformationAttribute("Left", Sort = 5)]
        public double Left
        {
            get
            {
                return Canvas.GetLeft(this);
            }

            set
            {
                Canvas.SetLeft(this, value);
            }
        }

        private void Initialize()
        {
            this.ReportLine = new System.Windows.Shapes.Line();

            this.StrokeThickness = 2;

            this.Stroke = Colors.Black;

            this.X1 = 0;

            this.X2 = this.lineLengthVal;

            this.Y1 = 0;

            this.Y2 = 0;

            this.Content = this.ReportLine;

            this.DataContext = this;
        }

        private static void OnStrokeRecieved(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ReportLineBase line = (ReportLineBase)d;

            Color col = (Color)e.NewValue;

            line.ReportLine.Stroke = new SolidColorBrush(col);
        }

        private void SetHorizontal()
        {
            this.X2 = this.LineLength;

            this.Y2 = 0;
        }

        private void SetVertical()
        {
            this.X2 = 0;

            this.Y2 = this.LineLength;
        }
    }
}
