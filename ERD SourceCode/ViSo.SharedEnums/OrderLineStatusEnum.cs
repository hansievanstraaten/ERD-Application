using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum OrderLineStatusEnum
  {
    [Description("Open")]
    Open = 1,
    
    [Description("Ready for Invoice")]
    ReadyForInvoice = 2,

    [Description("Canceled")]
    Canceled = 3,

    [Description("Back Order")]
    BackOrder = 4,

    [Description("PartialCompleted")]
    PartialCompleted = 5
  }
}
