using GeneralExtensions;
using IconSet;
//using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Image")]
    public class ReportImage : Image
    {
        private string imagePath;

        public ReportImage()
        {
            this.Source = IconSets.ResourceImageSource("DefaultImage", 25);
        }

        public XElement ItemXml
        {
            get
            {
                XElement result = new XElement("ReportObject");

                result.Add(new XAttribute("ObjectType", "ReportImage"));
                result.Add(new XAttribute("Top", this.Top));
                result.Add(new XAttribute("Left", this.Left));
                result.Add(new XAttribute("Width", this.Width));
                result.Add(new XAttribute("Height", this.Height));
                result.Add(new XAttribute("ImageString", this.ImageString));

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

        [FieldInformation("Select Image", Sort = 1)]
        [BrowseButton("ReportImageKey", "Search", "Search")]
        public string ImagePath
        {
            get
            {
                return this.imagePath;
            }

            set
            {
                this.imagePath = value;

                if (value.IsNullEmptyOrWhiteSpace())
                {
                    return;
                }

                base.Source = ImageFiles.LoadFromFile(value);
            }
        }

        [FieldInformation("Width", Sort = 2)]
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

        [FieldInformation("Height", Sort = 3)]
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
    
        public string ImageString
        {
            get
            {
                return base.Source.ConvertImageSourceToBitmap().ZipFile().ConvertBytesToString();
            }

            set
            {
                base.Source = (value.ConvertStringToBytes().UnzipFile() as System.Drawing.Bitmap).ConvertToImageSource();
            }
        }
    }
}
