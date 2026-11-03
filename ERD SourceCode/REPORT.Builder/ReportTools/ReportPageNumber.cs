using GeneralExtensions;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Page Number")]
    public class ReportPageNumber : LabelBase
    {
        public ReportPageNumber()
        {
            this.Text = "Page X";
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "ReportPageNumber"));

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
    }
}
