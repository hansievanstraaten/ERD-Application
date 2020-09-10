using System.ComponentModel;

namespace ViSo.SharedEnums.ReportEnums
{
    public enum StorageTypeEnum
    {
        [Description("File System")]
        FileSystem = 1,

        [Description("Database (Recommended)")]
        DatabaseSystem = 2
    }
}
