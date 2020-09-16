using GeneralExtensions;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Label")]
    public class ReportLabel : LabelBase
    {
        public ReportLabel()
        {
            base.Loaded += this.ReportLabel_Loaded;

            base.Content = "Label";
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("Caption", this.Caption));
                result.Add(new XAttribute("ObjectType", "ReportLabel"));

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

        public bool IsDesignMode
        {
            get;

            set;
        }

        [FieldInformation("Caption", Sort = 0)]
        public string Caption
        {
            get
            {
                return base.Content.ParseToString();
            }

            set
            {
                base.Content = value;
            }
        }

        private void ReportLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsDesignMode)
            {
                this.Background = Brushes.WhiteSmoke;
            }
            else
            {
                this.Background = Brushes.Transparent;
            }
        }
    }
}
