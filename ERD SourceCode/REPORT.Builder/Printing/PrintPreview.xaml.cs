using System.Collections.Generic;
using WPF.Tools.BaseClasses;

namespace REPORT.Builder.Printing
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : UserControlBase
    {

        public PrintPreview(Dictionary<int, PrintCanvas> pages)
        {
            this.InitializeComponent();

            foreach(KeyValuePair<int, PrintCanvas> pageKeyPair in pages)
            {
                pageKeyPair.Value.Margin = new System.Windows.Thickness(5);

                this.uxPageStack.Children.Add(pageKeyPair.Value);
            }
        }
    }
}
