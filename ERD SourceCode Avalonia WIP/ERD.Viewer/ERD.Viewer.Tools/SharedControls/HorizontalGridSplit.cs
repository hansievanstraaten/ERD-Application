using ERD.Viewer.Tools.BaseControls;
using Avalonia.Input;

namespace ERD.Viewer.Tools.SharedControls
{
    public class HorizontalGridSplit : GridSplitterBase
    {
        public HorizontalGridSplit()
        {
            this.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

            this.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

            this.ResizeDirection = Avalonia.Controls.GridResizeDirection.Rows;

            this.Height = 5;

            this.Cursor = new Cursor(StandardCursorType.SizeNorthSouth);

            this.MinWidth = 20;
        }
    }
}