using ERD.Base;
using ERD.Common;
using REPORT.Builder.Common.DatabaseOptions;
using REPORT.Builder.ReportTools;
using REPORT.Data.Models;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace REPORT.Builder.Common
{
    internal class ObjectCreator
    {
        internal static UIElement CreateReportObject(XElement xmlObject, bool isDesignMode)
        {
            string objectTypeName = xmlObject.Attribute("ObjectType").Value;

            switch (objectTypeName)
            {
                case "ReportDataObject":

                    ReportDataObject column = new ReportDataObject();

                    column.IsDesignMode = isDesignMode;

                    column.ItemXml = xmlObject;

                    return column as UIElement;

                case "ReportLabel":

                    ReportLabel lbl = new ReportLabel();

                    lbl.IsDesignMode = isDesignMode;

                    lbl.ItemXml = xmlObject;

                    return lbl as UIElement;

                case "ReportBorder":

                    ReportBorder border = new ReportBorder();

                    border.IsDesignMode = isDesignMode;

                    border.ItemXml = xmlObject;

                    return border as UIElement;

                case "CurrentDate":

                    CurrentDate date = new CurrentDate();

                    date.IsDesignMode = isDesignMode;

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
    
        internal static string GetCanvasSQL(
            ReportColumnModel[] columns, 
            List<WhereParameterModel> whereParameterModel, 
            List<ReportXMLPrintParameterModel> reportFilters,
            Dictionary<string, ReportSQLReplaceHeaderModel> replacementColumns,
            string orderByString)
        {
            DatabaseTypeEnum buildType = Connections.Instance.DatabaseModel.DatabaseType;

            IDataToSQL sql = null;

            switch(buildType)
            {
                case DatabaseTypeEnum.SQL:
                default:

                    sql = new MsSQL();

                    return sql.BuildSelectQuery(columns, whereParameterModel, reportFilters, replacementColumns, orderByString);
            }
        }

        internal static string UpdateStatements(UpdateStatementModel updateStatements, out List<string> columnValues)
		{
            DatabaseTypeEnum buildType = Connections.Instance.DatabaseModel.DatabaseType;

            IDataToSQL sql = null;

            switch (buildType)
            {
                case DatabaseTypeEnum.SQL:
                default:

                    sql = new MsSQL();

                    return sql.UpdateStatements(updateStatements, out columnValues);
            }
        }
    }
}
