using ERD.Base;
using ERD.Common;
using GeneralExtensions;

namespace REPORT.Data.Common
{
	public class Constants
	{
        public static string ReportSetupFileName
        {
            get
            {
                return $"{General.ProjectModel.ModelName}.ReportSystemSetup.{FileTypes.erpt.ParseToString()}";
            }
        }

        public static string None = "<None>";

        public static string SqlGetDate = "GetDate()";
    }
}
