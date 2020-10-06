using System.Globalization;
using System.Windows.Media;
using FlowDirection = System.Windows.FlowDirection;
using Label = System.Windows.Controls.Label;
using GeneralExtensions;

namespace WPF.Tools.CommonControls
{
    public class LableItem : Label
    {
        public double TextLenght
        {
            get
            {
                return this.StringRenderLength(this.Content.ParseToString());
            }
        }

        public double TextHeight
        {
            get
            {
                return this.StringRenderHeight(this.Content.ParseToString());
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

        public bool DisplayUnderscore { get; set; }

        public object Original { get; private set; }

        new public object Content
        {
            get
            {
                return base.Content;
            }

            set
            {
                this.Original = value;

                if (this.DisplayUnderscore && value != null)
                {
                    base.Content = value.ToString().Replace("_", "__");

                    return;
                }

                base.Content = value;
            }
        }
    }
}
