using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum PurchaseOrderStatusEnum
  {
    [Description("New Order")]
    NewOrder = 0,

    [Description("Order Captured")]
    Captured = 1,

    [Description("Ordered")]
    Ordered = 2,

    [Description("Payed")]
    Payed = 3,

    [Description("Waiting For Back Orders")]
    WaitingForBackOrders = 4,

    [Description("Completed")]
    Completed = 5,

    [Description("Canceled")]
    Caneled = 6
  }
}
