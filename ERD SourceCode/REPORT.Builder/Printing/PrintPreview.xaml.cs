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

            foreach(KeyValuePair<int, PrintCanvas> page in pages)
            {
                this.uxPageStack.Children.Add(page.Value);
            }
        }
    }
}
