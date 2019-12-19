using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum InvoiceStatusEnum
  {
    [Description("Created")]
    Created = 1,

    [Description("Printed")]
    Printed = 2,

    [Description("Delivered")]
    Delivered = 3,

    [Description("Payed")]
    Payed = 4,

    [Description("Canceled")]
    Canceled = 5
  }
}
