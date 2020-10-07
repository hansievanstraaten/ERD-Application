using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.CommonControls;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Label")]
    public class LabelBase : TextBlockItem
    {
        public LabelBase()
        {
            this.ElemntId = Guid.NewGuid();
        }

        public XElement ItemXml
        {
            get
            {
                XElement result = new XElement("ReportObject");

                result.Add(new XAttribute("Top", this.Top));
                result.Add(new XAttribute("Left", this.Left));
                result.Add(new XAttribute("Foreground", this.Foreground));
                result.Add(new XAttribute("FontSize", this.FontSize));
                result.Add(new XAttribute("FontFamily", this.FontFamily));
                result.Add(new XAttribute("FontWeight", this.FontWeight));
                result.Add(new XAttribute("HorizontalAlignment", this.HorizontalAlignment));
                result.Add(new XAttribute("VerticalAlignment", this.VerticalAlignment));
                result.Add(new XAttribute("TextWrapping", this.TextWrapping));
                result.Add(new XAttribute("Width", this.Width));
                result.Add(new XAttribute("Height", this.Height));

                if (this.Background != null)
                {
                    result.Add(new XAttribute("Background", this.Background));
                }

                return result;
            }

            set
            {
                foreach(XAttribute element in value.Attributes())
                {
                    this.SetPropertyValue(element.Name.LocalName, element.Value);
                }
            }
        }

        public Guid ElemntId { get; private set; }

        public bool IsDesignMode
        {
            get;

            set;
        }

        //[FieldInformation("Top", Sort = 101)]
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

        //[FieldInformation("Left", Sort = 102)]
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
        [ItemTypeAttribute(ModelItemTypeEnum.ColorBox)]
        [BrowseButtonAttribute("Background", "Pick Color", "Browse")]
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
        [ItemTypeAttribute(ModelItemTypeEnum.ColorBox)]
        [BrowseButtonAttribute("Foreground", "Pick Color", "Browse")]
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

        [FieldInformation("Font Size", Sort = 105)]
        new public double FontSize
        {
            get
            {
                return base.FontSize;
            }

            set
            {
                base.FontSize = value;
            }
        }

        [FieldInformation("Font Family", Sort = 106)]
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

                //base.FontWeight
            }
        }

        [FieldInformation("Font Weight", Sort = 107)]
        [ItemType(ModelItemTypeEnum.ComboBox, false)]
        [ValuesSource("FontWeightsList")]
        new public FontWeight FontWeight
        {
            get
            {
                return base.FontWeight;
            }

            set
            {
                base.FontWeight = value;
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

        public DataItemModel[] FontWeightsList
        {
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();

                result.Add(new DataItemModel { DisplayValue = FontWeights.Black.ToString(), ItemKey = FontWeights.Black });
                result.Add(new DataItemModel { DisplayValue = FontWeights.Bold.ToString(), ItemKey = FontWeights.Bold });
                result.Add(new DataItemModel { DisplayValue = FontWeights.DemiBold.ToString(), ItemKey = FontWeights.DemiBold});
                result.Add(new DataItemModel { DisplayValue = FontWeights.ExtraBlack.ToString(), ItemKey = FontWeights.ExtraBlack });
                result.Add(new DataItemModel { DisplayValue = FontWeights.ExtraBold.ToString(), ItemKey = FontWeights.ExtraBold });
                result.Add(new DataItemModel { DisplayValue = FontWeights.ExtraLight.ToString(), ItemKey = FontWeights.ExtraLight });
                result.Add(new DataItemModel { DisplayValue = FontWeights.Heavy.ToString(), ItemKey = FontWeights.Heavy});
                result.Add(new DataItemModel { DisplayValue = FontWeights.Light.ToString(), ItemKey = FontWeights.Light});
                result.Add(new DataItemModel { DisplayValue = FontWeights.Medium.ToString(), ItemKey = FontWeights.Medium});
                result.Add(new DataItemModel { DisplayValue = FontWeights.Normal.ToString(), ItemKey = FontWeights.Normal});
                result.Add(new DataItemModel { DisplayValue = FontWeights.Regular.ToString(), ItemKey = FontWeights.Regular});
                result.Add(new DataItemModel { DisplayValue = FontWeights.SemiBold.ToString(), ItemKey = FontWeights.SemiBold});
                result.Add(new DataItemModel { DisplayValue = FontWeights.Thin.ToString(), ItemKey = FontWeights.Thin });

                return result.ToArray();
            }
        }

        [FieldInformation("Horizontal Alignment", Sort = 108)]
        new public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return base.HorizontalAlignment;
            }

            set
            {
                base.HorizontalAlignment = value;
            }
        }

        [FieldInformation("Vertical Alignment", Sort = 109)]
        new public VerticalAlignment VerticalAlignment
        {
            get
            {
                return base.VerticalAlignment;
            }

            set
            {
                base.VerticalAlignment = value;
            }
        }

        [FieldInformation("Text Wrapping", Sort = 110)]
        new public TextWrapping TextWrapping
        { 
            get
            {
                return base.TextWrapping; 
            }

            set
            {
                base.TextWrapping = value;
            }
        }

        [FieldInformation("Width", Sort = 111)]
        new public double Width
        {
            get
            {
                return base.Width;
            }

            set
            {
                base.Width = value;
            }
        }

        [FieldInformation("Height", Sort = 112)]
        new public double Height
        {
            get
            {
                return base.Height;
            }

            set
            {
                base.Height = value;
            }
        }
    }
}
