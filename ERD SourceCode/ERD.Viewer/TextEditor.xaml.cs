using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer
{
  /// <summary>
  /// Interaction logic for TextEditor.xaml
  /// </summary>
  public partial class TextEditor : WindowBase
  {
    public TextEditor(string title, string text)
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

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;

      this.Close();
    }
  }
}
