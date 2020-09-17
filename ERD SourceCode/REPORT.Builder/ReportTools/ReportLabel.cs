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
            this.DataContext = this;

            base.Loaded += this.ReportLabel_Loaded;

            base.Text = "Label";
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
        
        [FieldInformation("Text", Sort = 0)]
        public string Caption
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
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
