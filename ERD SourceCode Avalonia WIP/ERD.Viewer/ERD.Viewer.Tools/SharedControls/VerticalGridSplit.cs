using ERD.Viewer.Tools.BaseControls;
using Avalonia.Input;

namespace ERD.Viewer.Tools.SharedControls
{
    public class VerticalGridSplit : GridSplitterBase
    {
        public VerticalGridSplit()
        {
            this.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

            this.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

            this.ResizeDirection = Avalonia.Controls.GridResizeDirection.Columns;

            this.Width = 3;

            this.Cursor = new Cursor(StandardCursorType.SizeWestEast);

            this.MinHeight = 20;
        }
    }
}
