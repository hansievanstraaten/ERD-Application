using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum PurchaseOrderLineStatusEnum
  {
    [Description("Line Added")]
    LineAdded = 1,

    [Description("Deleted")]
    Deleted = 2,

    [Description("Ordered")]
    Ordered = 3,

    [Description("Ordered")]
    BackOrdered = 4,

    [Description("Completed")]
    Completed = 5,

    [Description("Payed")]
    Payed = 6
  }
}
