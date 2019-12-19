using System.ComponentModel;

namespace ViSo.SharedEnums
{
  public enum AddressTypeEnum
  {
    [Description("Home Address")]
    Home = 1,

    [Description("Work Address")]
    Work = 2,

    [Description("PO Box Address")]
    PoBox = 3,

    [Description("Delivery Address")]
    Delivery = 4
  }
}
