using WPF.Tools.BaseClasses;
using WPF.Tools.HTML.Models;

namespace WPF.Tools.HTML
{
  /// <summary>
  /// Interaction logic for HyperlinkEdit.xaml
  /// </summary>
  public partial class HyperlinkEdit : WindowBase
  {
    public HyperlinkEdit()
    {
      this.InitializeComponent();

      this.Hyperlink = new HyperLinkModel();

      this.uxViewer.Items.Add(this.Hyperlink);

      this.AutoSize = true;
    }

    public HyperLinkModel Hyperlink
    {
      get;

      set;
    }

    private void Ok_Cliked(object sender, System.Windows.RoutedEventArgs e)
    {
      if (this.uxViewer.HasValidationError)
      {
        return;
      }

      this.DialogResult = true;

      this.Close();
    }

    private void Canceled_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
