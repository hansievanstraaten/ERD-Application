using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace ERD.Viewer.Tools.BaseControls
{
    public abstract class GridSplitterBase : GridSplitter
    {
        public GridSplitterBase()
        {
            SetStyes();

            this.IsTabStop = false;

            this.UseLayoutRounding = true;

            // Provide a minimal template so the splitter renders even when a theme
            // does not supply a template for GridSplitter.
            // The template simply draws a Border filled with the Background brush.
            this.Template = new FuncControlTemplate<GridSplitterBase>((parent, scope) =>
            {
                return new Border
                {
                    Background = parent.Background
                };
            });
        }

        private void SetStyes()
        {
            object? resource = Application.Current?.FindResource("GridSplitter");

            IBrush? brush = resource as IBrush;

            if (brush == null)
            {
                brush = Brushes.Gray;
            }

            this.Background = brush;
        }
    }
}
