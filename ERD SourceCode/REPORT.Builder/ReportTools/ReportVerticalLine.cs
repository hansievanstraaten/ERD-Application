using GeneralExtensions;
using System.Xml.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Builder.ReportTools
{
    [ModelName("Vertical Line")]
    public class ReportVerticalLine : ReportLineBase
    {
        public ReportVerticalLine()
        {
            base.IsHorizontal = false;

            base.LineLength = 2;
        }

        new public XElement ItemXml
        {
            get
            {
                XElement result = base.ItemXml;

                result.Add(new XAttribute("ObjectType", "ReportVerticalLine"));

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
