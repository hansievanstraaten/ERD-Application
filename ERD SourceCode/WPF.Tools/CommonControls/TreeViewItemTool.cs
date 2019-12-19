using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GeneralExtensions;

namespace WPF.Tools.CommonControls
{
  public class TreeViewItemTool : TreeViewItem
  {
    public delegate void OnCheckChangedEvent(object sender, bool? checkState);

    public event OnCheckChangedEvent OnCheck_Changed;

    private bool isCheckBox;

    private bool isChecked;

    private  CheckBoxItem checkBox;
    
    private LableItem labelItem;

    private object itemContent;

    private Brush labelForeGround = Brushes.Black;

    public TreeViewItemTool()
    {
      this.Initialize();
    }

    public bool IsCheckBox
    {
      get => this.IsChecked ? this.IsChecked : this.isCheckBox;

      set
      {
        if (this.isCheckBox != value && value)
        {
          this.InitializeCheckBox();
        }
        else
        {
          this.Initialize();
        }

        this.isCheckBox = value;

        if (this.checkBox != null)
        {
          this.checkBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
      }
    }

    public bool IsChecked
    {
      get
      {
        if (this.checkBox == null)
        {
          return this.isChecked;
        }

        return this.checkBox.IsChecked.IsTrue();
      }

      set
      {
        this.isChecked = value;

        if (this.checkBox != null)
        {
          this.checkBox.IsChecked = value;
        }
      }
    }

    new public object Header
    {
      get
      {
        if (this.labelItem == null)
        {
          return this.itemContent;
        }

        return this.labelItem.Content;
      }

      set
      {
        this.itemContent = value;

        if (this.labelItem != null)
        {
          this.labelItem.Content = value;
        }
      }
    }

    new public Brush Foreground
    {
      get
      {
        return this.labelForeGround;
      }

      set
      {
        this.labelForeGround = value;

        if (this.labelItem != null)
        {
          this.labelItem.Foreground = value;
        }
      }
    }

    private void isChecked_Changeds(object sender, RoutedEventArgs e)
    {
      OnCheck_Changed?.Invoke(this, this.checkBox.IsChecked);
    }

    private void Initialize()
    {
      StackPanel pnl = new StackPanel();

      pnl.Orientation = Orientation.Horizontal;

      this.checkBox = null;

      this.labelItem = new LableItem { Content =  this.Header, Foreground = this.Foreground };

      pnl.Children.Add(this.labelItem);

      base.Header = pnl;
    }

    private void InitializeCheckBox()
    {
      StackPanel pnl = new StackPanel();

      pnl.Orientation = Orientation.Horizontal;

      this.checkBox = new CheckBoxItem { Margin = new Thickness(0,5,0,0) };

      this.checkBox.Visibility = this.IsCheckBox ? Visibility.Visible : Visibility.Collapsed;

      this.checkBox.IsChecked = this.IsChecked;

      this.checkBox.Checked += this.isChecked_Changeds;

      this.checkBox.Unchecked += this.isChecked_Changeds;

      pnl.Children.Add(this.checkBox);

      this.labelItem = new LableItem { Content = this.Header, VerticalContentAlignment = VerticalAlignment.Top };

      pnl.Children.Add(this.labelItem);

      base.Header = pnl;
    }
  }
}
