using GeneralExtensions;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPF.Tools.BaseClasses;
using WPF.Tools.ColoutPicker;
using WPF.Tools.Dictionaries;
using WPF.Tools.ToolModels;

namespace WPF.Tools.HTML
{
  /// <summary>
  /// Interaction logic for HTMLEditor.xaml
  /// </summary>
  public partial class HTMLEditor : UserControlBase
  {
    private double[] fontSizes = new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 24 };

    private FontWeight boldnes = FontWeights.Normal;

    public HTMLEditor()
    {
      this.InitializeComponent();

      this.LoadFontObjects();
    }

    public string FlowDocumentText
    {
      get
      {
        return XamlWriter.Save(this.uxHtmlText.Document);
      }

      set
      {
        if (value.IsNullEmptyOrWhiteSpace())
        {
          this.uxHtmlText.Document.Blocks.Clear();

          return;
        }

        this.uxHtmlText.Document = (FlowDocument)XamlReader.Parse(value);
      }
    }

    private void Copy_Clicked(object sender, RoutedEventArgs e)
    {
      this.uxHtmlText.Copy();
    }

    private void Paste_Clicked(object sender, RoutedEventArgs e)
    {
      this.uxHtmlText.Paste();
    }

    private void ForeColour_Clicked(object sender, RoutedEventArgs e)
    {
      ColourPicker picker = new ColourPicker();

      if (picker.ShowDialog().IsFalse())
      {
        return;
      }

      this.uxHtmlText.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, picker.SelectedColour);
    }

    private void BackColour_Clicked(object sender, RoutedEventArgs e)
    {
      ColourPicker picker = new ColourPicker();

      if (picker.ShowDialog().IsFalse())
      {
        return;
      }
      
      this.uxHtmlText.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, picker.SelectedColour);
    }

    private void Undo_Clicked(object sender, RoutedEventArgs e)
    {
      this.uxHtmlText.Undo();
    }

    private void Redo_Clicked(object sender, RoutedEventArgs e)
    {
      this.uxHtmlText.Redo();
    }

    private void FontSize_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      DataItemModel itemModel = (DataItemModel) this.uxFontSize.SelectedItem;

      this.uxHtmlText.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, itemModel.ItemKey.ToDouble());
    }

    private void FontFamaly_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      DataItemModel itemModel = (DataItemModel)this.uxFontFamaly.SelectedItem;

      this.uxHtmlText.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, (FontFamily)itemModel.ItemKey);
    }

    private void FontStyleBold_Clicked(object sender, RoutedEventArgs e)
    {
      this.boldnes = this.boldnes == FontWeights.Normal ? FontWeights.Bold : FontWeights.Normal;

      this.uxHtmlText.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, this.boldnes);
    }

    private void FontStyleItalic_Clicked(object sender, RoutedEventArgs e)
    {
      this.uxHtmlText.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
    }

    private void FontStyleUnderLine_Clicked(object sender, RoutedEventArgs e)
    {
      this.uxHtmlText.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
    }
    
    private void AddImage_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        OpenFileDialog dlg = new OpenFileDialog();

        if (dlg.ShowDialog().IsFalse())
        {
          return;
        }

        Uri uri = new Uri(dlg.FileName, UriKind.Relative);
        
        BitmapImage bitmapImg = new BitmapImage(uri);

        Image image = new Image();
        
        image.Stretch = Stretch.Fill;
        
        image.Width = 50;
        
        image.Height = 50;
        
        image.Source = bitmapImg;

        BlockUIContainer container = new BlockUIContainer(image);

        TextPointer textPointer = this.uxHtmlText.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);
        Floater floater = new Floater(container, textPointer);
        
        floater.HorizontalAlignment = HorizontalAlignment.Center;

        floater.Width = image.Width;
        //this.uxHtmlText.Document.Blocks.Add(container);

        image.Loaded += delegate
        {
          AdornerLayer al = AdornerLayer.GetAdornerLayer(image);

          if (al != null)
          {
            al.Add(new ResizingAdorner(image));
          }
        };

        //BitmapImage bitmap = new BitmapImage(new Uri(dlg.FileName));

        //Image image = new Image();

        //image.Source = bitmap;

        //TextPointer textPointer = this.uxHtmlText.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);

        //Floater floater = new Floater(new BlockUIContainer(image), textPointer);

        //floater.HorizontalAlignment = HorizontalAlignment.Center;

        //floater.Width = bitmap.Width;
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }
    
    private void Hyperlink_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        HyperlinkEdit linker = new HyperlinkEdit();

        if (linker.ShowDialog().IsFalse())
        {
          return;
        }

        Run run = new Run(linker.Hyperlink.DisplayName);

        TextPointer textPoint = this.uxHtmlText.CaretPosition.GetInsertionPosition(LogicalDirection.Forward);

        Hyperlink hyperLink = new Hyperlink(run, textPoint);

        hyperLink.NavigateUri = new Uri(linker.Hyperlink.Url);

        hyperLink.IsEnabled = true;
        
        hyperLink.RequestNavigate += (hyperlinkSender, args) => Process.Start(args.Uri.ToString());
      }
      catch (Exception err)
      {
        MessageBox.Show(TranslationDictionary.Translate(err.InnerExceptionMessage()));
      }
    }
    
    private void LoadFontObjects()
    {
      foreach (double size in this.fontSizes)
      {
        this.uxFontSize.Items.Add(new DataItemModel { DisplayValue = size.ToString(), ItemKey = size });
      }

      foreach (FontFamily item in Fonts.SystemFontFamilies)
      {
        this.uxFontFamaly.Items.Add(new DataItemModel { DisplayValue = item.ParseToString(), ItemKey = item });
      }
    }
  }
}
