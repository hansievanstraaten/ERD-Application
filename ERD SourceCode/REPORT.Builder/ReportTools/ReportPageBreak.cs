using GeneralExtensions;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Page Break")]
    public class ReportPageBreak : ReportLineBase
    {
        public ReportPageBreak()
        {
            base.IsHorizontal = true;

            base.LineLength = 16;

            base.Stroke = Brushes.DarkGray;

            base.StrokeThickness = 4;

            //base.Left = 0;
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "ReportPageBreak"));

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

        new public double StrokeThickness
        {
            get
            {
                return base.StrokeThickness;
            }

            set
            {
                base.StrokeThickness = value;
            }
        }

        new public Brush Stroke
        {
            get
            {
                return base.Stroke;
            }

            set
            {
                base.Stroke = value;
            }
        }

        new public bool IsHorizontal
        {
            get
            {
                return base.IsHorizontal;
            }

            set
            {
                base.IsHorizontal = value;
            }
        }

        new public double LineLength
        {
            get
            {
                return base.LineLength;
            }

            set
            {
                base.LineLength = value;
            }
        }

        new public double Top
        {
            get
            {
                return base.Top;
            }

            set
            {
                base.Top = value;
            }
        }

        new public double Left
        {
            get
            {
                return base.Left;
            }

            set
            {
                base.Left = value;
            }
        }

    }
}
