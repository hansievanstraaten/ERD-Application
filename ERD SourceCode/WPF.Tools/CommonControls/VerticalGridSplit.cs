using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.BaseClasses;

namespace WPF.Tools.CommonControls
{
  public class VerticalGridSplit : GridSplitterBase
  {
    public VerticalGridSplit()
    {
      if (DesignerProperties.GetIsInDesignMode(this))
      {
        return;
      }

      this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

      this.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

      this.ResizeDirection = System.Windows.Controls.GridResizeDirection.Columns;

      this.Width = 3;
    }
  }
}
