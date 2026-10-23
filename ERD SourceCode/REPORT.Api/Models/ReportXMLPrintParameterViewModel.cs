using GeneralExtensions;
using ViSo.SharedEnums;
using WPF.Tools.Attributes;
using WPF.Tools.ToolModels;

namespace REPORT.Api.Models
{
    public class ReportXMLPrintParameterViewModel
    {
        public int ReportXMLVersion { get; set; }

        public Int64 MasterReport_Id { get; set; }
                
        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public bool IsActive { get; set; }

        public string FilterCaption { get; set; }

        public string FilterValue { get; set; }

        public OperatorsEnum Operator { get; set; }

        public string DefaultValue { get; set; }

        public bool? IsRequired { get; set; }

        public DataItemModel[] OperatorValues { get; set; }
    }
}
