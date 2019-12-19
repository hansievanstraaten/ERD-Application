using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IconSet;
using WPF.Tools.CommonControls;
using WPF.Tools.Dictionaries;
using WPF.Tools.Specialized;

namespace WPF.Tools.TabControl
{
  internal class TabHeader : UserControl
  {
    public delegate void OnCloseClickEvent(object sender);

    public event OnCloseClickEvent OnCloseClick;

    public delegate void OnClickEvent(object sender);

    public event OnClickEvent OnClick;

    private bool showClose = false;

    private bool isOnCloseButton = false;

    private bool isActive = false;

    private bool isTabEnabled = true;

    private bool isVertical = false;

    private double rotationAngel = 90;

    private Grid mainGrid = new Grid();

    private readonly FlashableLabel HeadingLabel = new FlashableLabel();

    private readonly ActionButton CloseButton = new ActionButton();

    private Border uxBorder;

    public TabHeader()
    {
      this.Initialize();

      this.LoadDefault();

      this.InitializeBorder();
    }

    private void InitializeBorder()
    {
      this.uxBorder = new Border();

      this.uxBorder.CornerRadius = new CornerRadius(5, 5, 0, 0);

      this.AddChild(this.uxBorder);

      this.uxBorder.Child = this.mainGrid;
    }

    public string HeaderText
    {
      get
      {
        if (this.HeadingLabel == null)
        {
          return string.Empty;
        }

        return Convert.ToString(this.HeadingLabel.Content);
      }

      set
      {
        this.HeadingLabel.Content = TranslationDictionary.Translate(value);
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.isActive;
      }

      set
      {
        this.isActive = value;

        if (value)
        {
          this.HeadingLabel.Foreground = (SolidColorBrush) this.FindResource("SelectedForeground");

          this.uxBorder.Background = (SolidColorBrush) this.FindResource("SelectedBackground");

          this.uxBorder.BorderBrush = (LinearGradientBrush) this.FindResource("TabGradientBrush");

          return;
        }

        if (this.isTabEnabled)
        {
          this.HeadingLabel.Foreground = (SolidColorBrush) this.FindResource("SelectedBackground");

          this.uxBorder.Background = (LinearGradientBrush) this.FindResource("TabGradientBrush");
        }
        else
        {
          this.HeadingLabel.Foreground = (LinearGradientBrush)this.FindResource("DisabledForeground");

          this.uxBorder.Background = (LinearGradientBrush) this.FindResource("TabGradientBrush");
        }
      }
    }

    public bool IsVertical
    {
      get
      {
        return this.isVertical; //this.HeadingLabel.IsVertical;
      }


      set
      {
        this.PerformRotation(value);

        this.isVertical = value;
      }
    }

    public void LoadDefault()
    {
      this.MouseMove += This_MouseMove;

      this.MouseLeave += This_MouseLeave;

      this.PreviewMouseUp += This_PreviewMouseUp;

      this.LoadHeaderText();

      this.LoadCloseButton();
    }

    public bool CanClose
    {
      get;

      set;
    }

    public bool ShowCloseButton
    {
      get
      {
        return this.showClose;
      }

      set
      {
        this.showClose = value;

        this.CloseButton.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
      }
    }

    public bool IsTabEnabled
    {
      get
      {
        return this.isTabEnabled;
      }

      set
      {
        this.isTabEnabled = value;

        this.HeadingLabel.IsEnabled = value;

        this.HeadingLabel.Foreground = (LinearGradientBrush)this.FindResource("DisabledForeground");
      }
    }

    public double RotationAngle
    {
      get
      {
        return this.rotationAngel;
      }

      set
      {
        this.rotationAngel = value;
      }
    }

    public void StartAnimation()
    {
      this.HeadingLabel.StartAnimation();
    }

    private void LoadHeaderText()
    {
      this.HeadingLabel.Name = "uxHeader";

      this.HeadingLabel.Content = "Not Set";

      this.HeadingLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

      this.HeadingLabel.VerticalAlignment = System.Windows.VerticalAlignment.Top;

      //this.HeadingLabel.Height = 25;

      this.HeadingLabel.Foreground = (SolidColorBrush) this.FindResource("SelectedBackground");

      Grid.SetColumn(this.HeadingLabel, 0);

      Grid.SetRow(this.HeadingLabel, 0);

      this.mainGrid.Children.Add(this.HeadingLabel);
    }

    private void LoadCloseButton()
    {
      this.CloseButton.Content = "X";

      this.CloseButton.Height = 19;

      this.CloseButton.Width = 20;

      this.CloseButton.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

      this.CloseButton.VerticalAlignment = System.Windows.VerticalAlignment.Top;

      this.CloseButton.Style = (Style) FindResource(ToolBar.ButtonStyleKey);

      this.CloseButton.Visibility = System.Windows.Visibility.Hidden;

      this.CloseButton.ToolTip = "Close";

      this.CloseButton.MouseEnter += CloseButton_MouseEnter;

      this.CloseButton.MouseLeave += CloseButton_MouseLeave;

      this.CloseButton.Click += CloseButton_Click;

      Grid.SetColumn(this.CloseButton, 1); // NOTE: set the column to 2 to re-implement the link button

      Grid.SetRow(this.CloseButton, 0);

      this.mainGrid.Children.Add(this.CloseButton);
    }

    private void This_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.CanClose)
      {
        return;
      }

      this.ShowCloseButton = false;
    }

    private void This_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.CanClose)
      {

        return;
      }

      this.ShowCloseButton = true;
    }

    private void This_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (this.isOnCloseButton)
      {
        return;
      }

      if (this.OnClick != null)
      {
        this.OnClick(this);
      }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.OnCloseClick != null)
      {
        this.OnCloseClick(this);
      }
    }

    private void CloseButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.isOnCloseButton = false;

      this.CloseButton.Foreground = Brushes.Black;
    }

    private void CloseButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (!this.ShowCloseButton)
      {
        return;
      }

      this.isOnCloseButton = true;

      this.CloseButton.Foreground = Brushes.Red;
    }

    private void Initialize()
    {
      this.BorderThickness = new Thickness(0.5, 0.5, 0.5, 0);

      ColumnDefinition col0 = new ColumnDefinition();

      ColumnDefinition col2 = new ColumnDefinition();

      col2.Width = new GridLength(0, GridUnitType.Auto);

      RowDefinition row = new RowDefinition();

      this.mainGrid.ColumnDefinitions.Add(col0);

      this.mainGrid.ColumnDefinitions.Add(col2);

      this.mainGrid.RowDefinitions.Add(row);
    }

    private void PerformRotation(bool newValue)
    {
      if (this.IsVertical == newValue)
      {
        // Nothing to do
        return;
      }

      if (newValue)
      {
        // Rotate to vertical posistion
        RotateTransform rotateV = new RotateTransform(this.rotationAngel);

        this.LayoutTransform = rotateV;

        return;
      }

      // Rotate Horizontal
      RotateTransform rotateH = new RotateTransform(0);

      this.LayoutTransform = rotateH;
    }
  }
}
