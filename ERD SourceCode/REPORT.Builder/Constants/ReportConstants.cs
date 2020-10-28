using ERD.Base;
using ERD.Common;
using GeneralExtensions;

namespace REPORT.Builder.Constants
{
    internal class ReportConstants
    {
        internal static string ReportSetupFileName
        {
            get
            {
                return $"{General.ProjectModel.ModelName}.ReportSystemSetup.{FileTypes.erpt.ParseToString()}";
            }
        }

        internal static string None = "<None>";
    }
}
