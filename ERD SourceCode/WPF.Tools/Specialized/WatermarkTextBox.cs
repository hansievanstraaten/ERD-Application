using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Tools.BaseClasses;
using GeneralExtensions;
using WPF.Tools.CommonControls;
using System.Windows.Media;

namespace WPF.Tools.Specialized
{
  public class WatermarkTextBox : ControlBase
  {
    public static RoutedEvent OnTextChangedEvent = EventManager.RegisterRoutedEvent("PART_OnTextChangeEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WatermarkTextBox));

    public delegate void ActionKeyPresedEvent(object sender, KeyEventArgs e);

    public event ActionKeyPresedEvent ActionKeyPresed;


    private TextBlock block;

    private TextBoxItem box;

    private Grid mainGrid;

    public event RoutedEventHandler OnTextChanged
    {
      add
      {
        this.AddHandler(OnTextChangedEvent, value);
      }

      remove
      {
        this.RemoveHandler(OnTextChangedEvent, value);
      }
    }

    public WatermarkTextBox()
    {
      this.Initialize();
    }

    public bool IsReadOnly
    {
      get
      {
        return this.box.IsReadOnly;
      }

      set
      {
        this.box.IsReadOnly = value;
      }
    }

    public bool IsMultiline
    {
      get
      {
        return this.box.IsMultiline;
      }

      set
      {
        this.box.IsMultiline = true;
      }
    }

    public bool IsSpellCheckerDisabled
    {
      get
      {
        if (this.box.SpellCheck == null)
        {
          return false;
        }

        return this.box.SpellCheck.IsEnabled;
      }

      set
      {
        this.box.SpellCheck.IsEnabled = !value;
      }
    }

    public bool RaiseActionKeyEvent
    {
      get
      {
        return this.box.RaiseActionKeyEvent;
      }

      set
      {
        this.box.RaiseActionKeyEvent = value;
      }
    }

    public string Text
    {
      get
      {
        return this.box.Text;
      }

      set
      {
        this.box.Text = value;

        if (value.IsNullEmptyOrWhiteSpace())
        {
          this.block.Visibility = Visibility.Visible;
        }
        else
        {
          this.block.Visibility = Visibility.Collapsed;
        }
      }
    }

    public string WatermarkText
    {
      get
      {
        return this.block.Text;
      }

      set
      {
        this.block.Text = value;
      }
    }

    public Brush WatermarkForeground
    {
      get
      {
        return this.block.Foreground;
      }

      set
      {
        this.block.Foreground = value;
      }
    }

    public TextWrapping TextWrapping
    {
      get
      {
        return this.box.TextWrapping;
      }

      set
      {
        this.box.TextWrapping = value;
      }
    }

    public Key[] ActionKeys
    {
      get
      {
        return this.box.ActionKeys;
      }

      set
      {
        this.box.ActionKeys = value;
      }
    }

    public void SetCaret(int index)
    {
      this.box.CaretIndex = index;
    }

    new public void Focus()
    {
      base.Focus();

      Keyboard.Focus(this.box);

      this.SetCaret(0);
    }
    
    private void Initialize()
    {
      NameScope.SetNameScope(this, new NameScope());

      ResourceDictionary resource = new ResourceDictionary();

      Uri resourcePath = new Uri("/WPF.Tools;component/Templates/ControlsTemplate.xaml", UriKind.Relative);
      
      resource.Source = resourcePath;

      this.Resources.MergedDictionaries.Add(resource);

      this.Template = this.FindResource("WatermarkText") as ControlTemplate;

      this.ApplyTemplate();

      this.box = (TextBoxItem) this.Template.FindName("uxUsetText", this);

      this.block = (TextBlock) this.Template.FindName("uxWaterMark", this);

      this.mainGrid = (Grid) this.Template.FindName("uxMainGrid", this);
      
      this.box.TextChanged += this.box_TextChanged;

      this.box.GotFocus += this.Box_GotFocus;

      this.box.LostFocus += this.Box_LostFocus;

      this.box.ActionKeyPresed += this.BoxActionKey_Pressed;

      this.HorizontalAlignment = HorizontalAlignment.Stretch;

      this.VerticalAlignment = VerticalAlignment.Stretch;
    }

    private void BoxActionKey_Pressed(object sender, KeyEventArgs e)
    {
      if (this.RaiseActionKeyEvent && this.ActionKeyPresed != null)
      {
        this.ActionKeyPresed(this, e);
      }
    }

    private void Box_LostFocus(object sender, RoutedEventArgs e)
    {
      if (this.Text.IsNullEmptyOrWhiteSpace())
      {
        this.block.Visibility = Visibility.Visible;
      }
    }

    private void Box_GotFocus(object sender, RoutedEventArgs e)
    {
      this.block.Visibility = Visibility.Collapsed;
    }
    
    private void box_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (this.Text.IsNullEmptyOrWhiteSpace())
      {
        this.block.Visibility = Visibility.Visible;
      }
      else
      {
        this.block.Visibility = Visibility.Collapsed;
      }

      RoutedEventArgs args = new RoutedEventArgs(WatermarkTextBox.OnTextChangedEvent, this);

      this.RaiseEvent(args);
    }
  }
}
