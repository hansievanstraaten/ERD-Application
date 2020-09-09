using System.ComponentModel;

namespace REPORT.Builder.ReportEnums
{
    public enum StorageTypeEnum
    {
        [Description("File System")]
        FileSystem = 1,

        [Description("Database (Recommended)")]
        DatabaseSystem = 2
    }
}
