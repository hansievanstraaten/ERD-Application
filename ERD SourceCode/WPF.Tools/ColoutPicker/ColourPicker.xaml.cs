using GeneralExtensions;
using System;
using System.Windows;
using System.Windows.Media;
using WPF.Tools.BaseClasses;
using WPF.Tools.CommonControls;
using WPF.Tools.Dictionaries;

namespace WPF.Tools.ColoutPicker
{
  /// <summary>
  /// Interaction logic for ColourPicker.xaml
  /// </summary>
  public partial class ColourPicker : WindowBase
  {
    public ColourPicker()
    {
      this.InitializeComponent();

      this.AutoSize = true;
    }

    public Brush SelectedColour { get; private set; }

    private void Colour_Clicked(object sender, System.Windows.RoutedEventArgs e)
    {
      try
      {
        ActionButton button = (ActionButton)sender;

        this.SelectedColour = button.Background;

        this.DialogResult = true;

        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }
    
    private void Cancel_Clicked(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
