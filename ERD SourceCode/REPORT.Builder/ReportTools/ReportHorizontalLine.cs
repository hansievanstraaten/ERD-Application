using GeneralExtensions;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Horizontal Line")]
    public class ReportHorizontalLine : ReportLineBase
    {
        public ReportHorizontalLine()
        {
            base.IsHorizontal = true;

            base.LineLength = 2;
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "ReportHorizontalLine"));

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
