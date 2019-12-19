using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.BaseClasses;

namespace WPF.Tools.CommonControls
{
  public class HorizontalGridSplit : GridSplitterBase
  {
    public HorizontalGridSplit()
    {
      if (DesignerProperties.GetIsInDesignMode(this))
      {
        return;
      }

      this.VerticalAlignment = System.Windows.VerticalAlignment.Center;

      this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

      this.ResizeDirection = System.Windows.Controls.GridResizeDirection.Rows;

      this.Height = 5;
    }
  }
}