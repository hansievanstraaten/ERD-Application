using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WPF.Tools.CommonControls
{
    public class TextBlockItem : TextBlock
    {
        public double TextLenght
        {
            get
            {
                return this.StringRenderLength(this.Text);
            }
        }

        public double TextHeight
        {
            get
            {
                return this.StringRenderHeight(this.Text);
            }
        }

        public double StringRenderLength(string textValue)
        {
            FormattedText formattedText = new FormattedText(
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

        public double StringRenderHeight(string textValue)
        {
            FormattedText formattedText = new FormattedText(
              textValue,
              CultureInfo.CurrentCulture,
              FlowDirection.LeftToRight,
              new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
              this.FontSize,
              Brushes.Black,
              new NumberSubstitution(),
              1);

            return formattedText.Height;
        }
    }
}
