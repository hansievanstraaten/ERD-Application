using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum InvMovementTypeEnum
  {
    [Description("Ordered")]
    Ordered = 1,

    [Description("Back Order")]
    BackOrder = 2,

    [Description("Reserved")]
    Reserved = 3,

    [Description("Released")]
    Released = 4,

    [Description("Order Canceld")]
    OrderCanceled = 5,

    [Description("Backorder Release")]
    BackorderRelease = 6
  }
}
