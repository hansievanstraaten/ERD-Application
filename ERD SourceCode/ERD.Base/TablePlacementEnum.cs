using System.Windows;

namespace ERD.Base
{
  public enum TablePlacementEnum
  {
    ParentAtOp,
    ParentAtLeft,
    ParentAtBottom,
    ParentAtRight
  }

  public static class ParentPlacementCalculator
  {
    public static TablePlacementEnum CalculatePlacement(
      Point parent, 
      Point child, 
      double parentRight, 
      double childRight,
      double parentBottom,
      double childBottom)
    {
      if (parent.Y <= childBottom && parentBottom >= child.Y) // Check Top and bottoms
      { // Table to a side
        if (parent.X > child.X)
        {
          return TablePlacementEnum.ParentAtRight;
        }

        return TablePlacementEnum.ParentAtLeft;
      }

      if (parent.Y > childBottom)
      { 
        return TablePlacementEnum.ParentAtBottom;
      }
      
      return TablePlacementEnum.ParentAtOp;
    }
  }
}
