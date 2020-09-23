using ERD.Base;
using ERD.Common;
using REPORT.Builder.Common.DatabaseOptions;
using REPORT.Builder.ReportTools;
using REPORT.Data.Models;
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
                case "ReportDataObject":

                    ReportDataObject column = new ReportDataObject();

                    column.ItemXml = xmlObject;

                    return column as UIElement;

                case "ReportLabel":

                    ReportLabel lbl = new ReportLabel();

                    lbl.ItemXml = xmlObject;

                    return lbl as UIElement;

                case "ReportBorder":

                    ReportBorder border = new ReportBorder();

                    border.ItemXml = xmlObject;

                    return border as UIElement;

                case "CurrentDate":

                    CurrentDate date = new CurrentDate();

                    date.ItemXml = xmlObject;

                    return date as UIElement;

                case "ReportImage":

                    ReportImage image = new ReportImage();

                    image.ItemXml = xmlObject;

                    return image as UIElement;

                case "ReportHorizontalLine":

                    ReportHorizontalLine horizontalLine = new ReportHorizontalLine();

                    horizontalLine.ItemXml = xmlObject;

                    return horizontalLine as UIElement;

                case "ReportVerticalLine":

                    ReportVerticalLine verticalLine = new ReportVerticalLine();

                    verticalLine.ItemXml = xmlObject;

                    return verticalLine as UIElement;

                case "ReportPageBreak":

                    ReportPageBreak pageBreak = new ReportPageBreak();

                    pageBreak.ItemXml = xmlObject;

                    return pageBreak as UIElement;

                default:

                    return new ReportLabel() as UIElement;
            }
        }
    
        internal static string GetCanvasSQL(ReportColumnModel[] columns)
        {
            DatabaseTypeEnum buildType = Connections.Instance.DatabaseModel.DatabaseType;

            IDataToSQL sql = null;

            switch(buildType)
            {
                case DatabaseTypeEnum.SQL:
                default:

                    sql = new MsSQL();

                    return sql.BuildSelectQuery(columns);
            }
        }
    }
}
