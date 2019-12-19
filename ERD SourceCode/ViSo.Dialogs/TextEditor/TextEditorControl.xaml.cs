using WPF.Tools.BaseClasses;

namespace ViSo.Dialogs.TextEditor
{
  /// <summary>
  /// Interaction logic for TextEditorControl.xaml
  /// </summary>
  internal partial class TextEditorControl : UserControlBase
  {
    public TextEditorControl(string text)
    {
      this.InitializeComponent();

      this.Text = text;
    }

    public string Text
    {
      get
      {
        return this.uxText.Text;
      }

      set
      {
        this.uxText.Text = value;
      }
    }
  }
}
