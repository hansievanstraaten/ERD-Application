using GeneralExtensions;
using System;
using System.Windows;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Current Date")]
    public class CurrentDate : LabelBase
    {
        public CurrentDate()
        {
            this.DataContext = this;

            base.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "CurrentDate"));

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

    }
}
