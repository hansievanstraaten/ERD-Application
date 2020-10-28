using System.ComponentModel;

namespace ViSo.SharedEnums.ReportEnums
{
    public enum StorageTypeEnum
    {
        [Description("SQLite")]
        SQLite = 1,

        [Description("MS SQL")]
        MsSql = 2
    }
}
