using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum PorInvoiceStatusEnum
  {
    [Description("Captured")]
    Captured,

    [Description("Partially Payed")]
    PartiallyPayed,

    [Description("Payed")]
    Payed,

    [Description("Completed")]
    Completed,

    [Description("Deleted")]
    Deleted
  }
}
