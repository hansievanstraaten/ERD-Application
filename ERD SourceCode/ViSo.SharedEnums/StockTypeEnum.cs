using System.ComponentModel;
using System.Linq;

namespace ViSo.SharedEnums
{
  public enum StockTypeEnum
  {
    [Description("Stockable")]
    StockAble = 1,

    [Description("Non-Stockable")]
    NonStock = 2,

    [Description("Service")]
    Service = 3,

    [Description("Consumables")]
    Consumables = 4
  }

  public static class StockTypeCheck
  {
    private static StockTypeEnum[] inventoryItems = new StockTypeEnum[] 
      {
        StockTypeEnum.StockAble,
        StockTypeEnum.NonStock
      };

    public static bool IsInventoryItem(StockTypeEnum type)
    {
      return StockTypeCheck.inventoryItems.Contains(type);
    }

    public static bool IsInventoryItem(int typeValue)
    {
      StockTypeEnum type = (StockTypeEnum) typeValue;

      return StockTypeCheck.IsInventoryItem(type);
    }
  }
}
