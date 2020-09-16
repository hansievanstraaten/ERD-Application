using REPORT.Builder.ReportTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace REPORT.Builder.Common
{
    internal class ObjectCreator
    {
        internal static UIElement CreateReportObject(XElement xmlObject)
        {
            string objectTypeName = xmlObject.Attribute("ObjectType").Value;

            switch (objectTypeName)
            {
                case "ReportLabel":

                    ReportLabel lbl = new ReportLabel();

                    lbl.ItemXml = xmlObject;

                    return lbl as UIElement;

                default:

                    return new ReportLabel() as UIElement;
            }
        }
    }
}
