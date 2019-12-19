using GeneralExtensions;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.Tools.CommonControls;
using WPF.Tools.Exstention;

namespace WPF.Tools.Stacks
{
  /// <summary>
  /// Interaction logic for SearchStackPanel.xaml
  /// </summary>
  public partial class SearchStackPanel : UserControl
  {
    public SearchStackPanel()
    {
      this.InitializeComponent();
    }

    public string WatermarkText
    {
      get
      {
        return this.uxSearchBox.WatermarkText;
      }

      set
      {
        this.uxSearchBox.WatermarkText = value;
      }
    }

    public Brush WatermarkForeground
    {
      get
      {
        return this.uxSearchBox.WatermarkForeground;
      }

      set
      {
        this.uxSearchBox.WatermarkForeground = value;
      }
    }

    public void Add(LableItem item)
    {
      this.uxLablesStack.Children.Add(item);
    }

    public void AddRange(LableItem[] items)
    {
      foreach (LableItem item in items)
      {
        this.Add(item);
      }
    }

    public void Clear()
    {
      this.uxLablesStack.Children.Clear();
    }

    private void SearchText_Changed(object sender, RoutedEventArgs e)
    {
      try
      {
        UIElement[] tables = this.uxLablesStack.FindVisualControls(typeof(LableItem));

        if (this.uxSearchBox.Text.IsNullEmptyOrWhiteSpace())
        {
          foreach (UIElement item in this.uxLablesStack.FindVisualControls(typeof(LableItem)))
          {
            item.Visibility = Visibility.Visible;
          }

          return;
        }

        string searchText = this.uxSearchBox.Text.ToLower();

        foreach (UIElement item in this.uxLablesStack.FindVisualControls(typeof(LableItem)))
        {
          item.Visibility = ((LableItem)item).Original.ParseToString().ToLower().Contains(searchText) ? Visibility.Visible : Visibility.Collapsed;
        }
      }
      catch
      {
        // DO NOTHING
      }
    }
  }
}
