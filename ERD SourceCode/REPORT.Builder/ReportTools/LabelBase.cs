using GeneralExtensions;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.CommonControls;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Builder.ReportTools
{
    public class LabelBase : LableItem
    {
        public XElement ItemXml
        {
            get
            {
                XElement result = new XElement("ReportObject");

                result.Add(new XAttribute("Top", this.Top));
                result.Add(new XAttribute("Left", this.Left));
                result.Add(new XAttribute("Background", this.Background));                
                result.Add(new XAttribute("Foreground", this.Foreground));                
                result.Add(new XAttribute("FontSize", this.FontSize));                
                result.Add(new XAttribute("FontFamily", this.FontFamily));
                
                //result.Add(new XElement("Top"), this.Top);
                //result.Add(new XElement("Top"), this.Top);
                //result.Add(new XElement("Top"), this.Top);

                return result;
            }

            set
            {
                foreach(XElement element in value.Elements())
                {
                    this.SetPropertyValue(element.Name.LocalName, element.Value);
                }
            }
        }

        [FieldInformation("Top", Sort = 101)]
        public double Top
        {
            get
            {
                return Canvas.GetTop(this);
            }

            set
            {
                Canvas.SetTop(this, (value < 0 ? 0 : value));
            }
        }

        [FieldInformation("Left", Sort = 102)]
        public double Left
        {
            get
            {
                return Canvas.GetLeft(this);
            }

            set
            {
                Canvas.SetLeft(this, (value < 0 ? 0 : value));
            }
        }

        [FieldInformation("Background", Sort = 103)]
        new public Brush Background
        {
            get
            {
                return base.Background;
            }

            set
            {
                base.Background = value;
            }
        }

        [FieldInformation("Foreground", Sort = 104)]
        new public Brush Foreground
        {
            get
            {
                return base.Foreground;
            }

            set
            {
                base.Foreground = value;
            }
        }

        [FieldInformation("FontSize", Sort = 105)]
        new public double FontSize
        {
            get
            {
                return base.FontSize;
            }

            set
            {
                base.FontSize = value;

                //base.FontFamily
            }
        }

        [FieldInformation("FontFamily", Sort = 106)]
        [ItemType(ModelItemTypeEnum.ComboBox, false)]
        [ValuesSource("FontFamalies")]
        new public FontFamily FontFamily
        {
            get
            {
                return base.FontFamily;
            }

            set
            {
                base.FontFamily = value;
            }
        }

        public DataItemModel[] FontFamalies
        {
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();

                foreach (FontFamily font in Fonts.SystemFontFamilies)
                {
                    result.Add(new DataItemModel { DisplayValue = font.ToString(), ItemKey = font });
                }

                return result.ToArray();
            }
        }

    }
}
