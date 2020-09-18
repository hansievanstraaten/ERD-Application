﻿
using GeneralExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Border")]
    public class ReportBorder : Border
    {
        //private bool itemSelected;

        public ReportBorder()
        {
            this.DataContext = this;

            this.BorderThickness = new Thickness(2);

            this.BorderBrush = Brushes.Black;

            this.Background = Brushes.Transparent;

            base.Width = 100;

            base.Height = 40;
        }

        public XElement ItemXml
        {
            get
            {
                XElement result = new XElement("ReportObject");

                result.Add(new XAttribute("ObjectType", "ReportBorder"));
                result.Add(new XAttribute("Width", this.Width));
                result.Add(new XAttribute("Height", this.Height));
                result.Add(new XAttribute("BorderThickness", this.BorderThickness));
                result.Add(new XAttribute("BorderBrush", this.BorderBrush));
                result.Add(new XAttribute("Top", this.Top));
                result.Add(new XAttribute("Left", this.Left));

                if (this.Background != null)
                {
                    result.Add(new XAttribute("Background", this.Background));
                }

                return result;
            }

            set
            {
                foreach (XAttribute element in value.Attributes())
                {
                    this.SetPropertyValue(element.Name.LocalName, element.Value);
                }
            }
        }

        //public bool ItemSelected
        //{
        //    get
        //    {
        //        return this.itemSelected;
        //    }
            
        //    set
        //    {
        //        this.itemSelected = value;
        //    }
        //}

        public bool IsDesignMode
        {
            get;

            set;
        }

        [FieldInformation("Width", Sort = 1)]
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

        [FieldInformation("Height", Sort = 2)]
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

        [FieldInformation("Border Thickness", Sort = 3)]
        new public Thickness BorderThickness
        {
            get
            {
                return base.BorderThickness;
            }

            set
            {
                base.BorderThickness = value;
            }
        }

        [FieldInformation("Border Brush", Sort = 4)]
        new public Brush BorderBrush
        {
            get
            {
                return base.BorderBrush;
            }

            set
            {
                base.BorderBrush = value;
            }
        }

        [FieldInformation("Background", Sort = 5)]
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

        //[FieldInformation("Top", Sort = 5)]
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

        //[FieldInformation("Left", Sort = 6)]
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
    }
}
