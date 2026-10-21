using System.ComponentModel;

namespace ViSo.SharedEnums
{
    public enum OperatorsEnum
    {
        [Description("=")]
        Equals,
        [Description("!=")]
        NotEquals,
        [Description(">")]
        GreaterThan,
        [Description("<")]
        LessThan,
        [Description(">=")]
        GreaterThanOrEqual,
        [Description("<=")]
        LessThanOrEqual,
        [Description("Contains")]
        Contains,
        [Description("StartsWith")]
        StartsWith,
        [Description("EndsWith")]
        EndsWith
    }
}
