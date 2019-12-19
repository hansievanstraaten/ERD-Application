using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GeneralExtensions;

namespace WPF.Tools.CommonControls
{
  public class TextBoxItem : TextBox
  {
    public delegate void ActionKeyPresedEvent(object sender, KeyEventArgs e);

    public event ActionKeyPresedEvent ActionKeyPresed;

    private bool raiseActionKeyEvent;

    private bool isMultiline;

    private Key[] actionKeys = new Key[] { Key.Enter, Key.Tab };

    public TextBoxItem()
    {
      this.SpellCheck.IsEnabled = true;
    }

    public bool RaiseActionKeyEvent
    {
      get
      {
        return this.raiseActionKeyEvent;
      }

      set
      {
        this.raiseActionKeyEvent = value;
      }
    }

    public bool IsMultiline
    {
      get
      {
        return this.isMultiline;
      }

      set
      {
        this.isMultiline = value;

        this.TextWrapping = value ? TextWrapping.Wrap : TextWrapping.NoWrap;

        this.AcceptsReturn = value;

        this.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;

        if (value)
        {
          this.ActionKeys.Remove(Key.Enter);
        }
      }
    }

    public double TextLenght
    {
      get
      {
        return this.StringRenderLength(this.Text);
      }
    }

    public Key[] ActionKeys
    {
      get
      {
        return this.actionKeys;
      }

      set
      {
        this.actionKeys = value;
      }
    }

    public double StringRenderLength(string textValue)
    {
      var formattedText = new FormattedText(
        textValue,
        CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
        this.FontSize,
        Brushes.Black,
        new NumberSubstitution(),
        1);

      return formattedText.Width;
    }
    
    protected override void OnPreviewKeyUp(KeyEventArgs e)
    {
      base.OnPreviewKeyUp(e);

      if (this.ActionKeys.Contains(e.Key) &&
          (this.raiseActionKeyEvent && this.ActionKeyPresed != null))
      {
          this.ActionKeyPresed(this, e);
      }
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);

      this.SelectAll();
    }
  }
}
