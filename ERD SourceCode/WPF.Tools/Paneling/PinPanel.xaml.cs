using System;
using System.Threading.Tasks;
using System.Windows.Media;
using WPF.Tools.BaseClasses;
using GeneralExtensions;
using System.Windows;
using WPF.Tools.Functions;
using WPF.Tools.Dictionaries;
using WPF.Tools.TabControl;

namespace WPF.Tools.Paneling
{
  /// <summary>
  /// Interaction logic for PinPanel.xaml
  /// </summary>
  public partial class PinPanel : UserControlBase
  {
    public delegate void PinChangedEvent(object sender, bool isPined);
    
    public event PinChangedEvent PinChanged;

    private bool isPined;
    
    private VerticalTabDocLocationEnum docLocation;

    public PinPanel()
    {
      this.InitializeComponent();

      this.uxPinedTitle.Background = Brushes.DarkGray;
    }

    public bool IsPined
    {
      get
      {
        return this.isPined;
      }

      set
      {
        this.isPined = value;

        this.uxPinButton.ResourceImageName = value ?  "Pined" : "UnPined";
      }

    }

    public double TitleHight
    {
      get
      {
        return this.uxPinedTitle.TextHeight;
      }
    }

    new public string Title
    {
      get
      {
        return base.Title;
      }

      set
      {
        base.Title = value;

        this.uxPinedTitle.Content = value;
      }
    }

    new public object Content
    {
      get
      {
        return this.uxContent.Content;
      }

      set
      {
        this.uxContent.Content = value;

        this.uxPinButton.Visibility = value == null ? Visibility.Collapsed : Visibility.Visible;

        this.uxPinedTitle.Visibility = value == null ? Visibility.Collapsed : Visibility.Visible;
      }
    }

    public VerticalTabDocLocationEnum DocLocation
    {
      get
      {
        return this.docLocation;
      }

      set
      {
        this.docLocation = value;

        if (value == VerticalTabDocLocationEnum.Left)
        {
          // Default
          this.uxPinedTitle.HorizontalContentAlignment = HorizontalAlignment.Left;

          this.uxButtonStack.HorizontalAlignment = HorizontalAlignment.Right;
        }
        else
        {
          this.uxPinedTitle.HorizontalContentAlignment = HorizontalAlignment.Right;

          this.uxButtonStack.HorizontalAlignment = HorizontalAlignment.Left;
        }
      }
    }

    private void Pin_Cliked(object sender, System.Windows.RoutedEventArgs e)
    {
      try
      {
        if (this.IsPined)
        {
          this.SetAsUnPined();
        }
        else
        {
          this.SetAsPined();
        }

        this.PinChanged?.Invoke(this, this.IsPined);
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }
    
    private void SetAsPined()
    {
      this.IsPined = true;

      this.uxPinButton.ResourceImageName = "Pined";
    }

    private void SetAsUnPined()
    {
      this.IsPined = false;
      
      this.uxPinButton.ResourceImageName = "UnPined";
    }
  }
}
