using GeneralExtensions;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Linq;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Builder.Printing
{
    internal static class XmlHelper
    {
        internal static bool IsDataObject(this XElement reportObject)
        {
            return reportObject.Attribute("ObjectType").Value == "ReportDataObject";
        }

        internal static bool IsEmptyDataSuppressed(this XElement reportObject)
		{
            return reportObject.Attribute("SuppressIfNoData").Value.ToBool();
        }

        internal static bool IsPageBreak(this XElement reportObject)
        {
            return reportObject.Attribute("ObjectType").Value == "ReportPageBreak";
        }

        internal static int GetRowSectionIndex(this XElement row)
        {
            return row.Attribute("SectionIndex").Value.ToInt32();
        }

        internal static int GetSectionGroupIndex(this XElement section)
        {
            return section.Attribute("SectionGroupIndex").Value.ToInt32();
        }

        internal static string GetObjectColumn(this XElement reportObject)
        {
            return reportObject.Attribute("ObjectColumn").Value;
        }

        /// <summary>
        /// Gets the ReportSettings from the Report Document XML
        /// </summary>
        /// <param name="document">The Report Document to get the xml fomr</param>
        /// <param name="reportType">The ReportTypeEnum to get the XML for</param>
        /// <returns>Returns the FirstOrDefault XElement</returns>
        internal static XElement GetReportSettings(this XDocument document,  ReportTypeEnum reportType)
        {
            return document.Root
                .Descendants("ReportSettings")
                .FirstOrDefault(rs => rs.Attribute("ReportTypeEnum").Value.ToInt32() == (int)reportType);
        }

        internal static Dictionary<int, XElement> GetDataReportSections(this XDocument document)
        {
            XElement reportSettings = document.GetReportSettings(ReportTypeEnum.ReportContent);

            if (reportSettings == null)
            {
                return new Dictionary<int, XElement>();
            }

            return reportSettings
                .Descendants("ReportSection")
                .ToDictionary(atr => atr.Attribute("SectionIndex").Value.ToInt32());
        }

        /// <summary>
        /// Gets the ReportSection from the ReportSettings XML
        /// </summary>
        /// <param name="element">The report ReportSettings the get the result XML from</param>
        /// <param name="sectionIndex">The int section index to retreive</param>
        /// <returns>Returns the ReportSection at the selected section index </returns>
        internal static XElement GetReportSection(this XElement element, int sectionIndex, out SectionTypeEnum sectionTypeEnum)
        {
            XElement result = element
                .Descendants("ReportSection")
                .FirstOrDefault(si => si.Attribute("SectionIndex").Value.ToInt32() == sectionIndex);

            sectionTypeEnum = (SectionTypeEnum)result.Attribute("SectionType").Value.ToInt32();

            return result;
        }

        /// <summary>
        /// Gets the Canvas XML from the ReportSection
        /// </summary>
        /// <param name="element">The ReportSection to get the Canvas XML from</param>
        /// <returns>Returns the CanvasXml</returns>
        internal static XElement GetCanvasXml(this XElement element)
        {
            return element
                .Element("CanvasXml");
        }

        internal static List<XElement> GetReportObjects(this XElement element)
        {
            return element
                .Descendants("ReportObject")
                .ToList();
        }

        internal static List<int> GetSectionForeignSectionIndexes(this XElement section)
        {
            XElement canvas = section.GetCanvasXml();

            List<int> result = new List<int>();

            foreach(XElement item in canvas.Element("ForeignSectionIndexes").Elements())
            {
                result.Add(item.Value.ToInt32());
            }

            return result;
        }

    }
}
