using Avalonia.Media;
using ERD.Viewer.Shared.Extensions;
using System.Globalization;
using Label = Avalonia.Controls.Label;

namespace ERD.Viewer.Tools.SharedControls
{
    public class LableItem : Label
    {
        public double TextLenght
        {
            get
            {
                return this.StringRenderLength(Content.ParseToString());
            }
        }

        public double TextHeight
        {
            get
            {
                return this.StringRenderHeight(Content.ParseToString());
            }
        }

        public double StringRenderLength(string textValue)
        {
            if (textValue.IsNullEmptyOrWhiteSpace())
            {
                return 0;
            }

            int charCount = textValue.ToCharArray().Length;

            FormattedText formattedText = new FormattedText(
                textValue,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                Brushes.Black);

            return formattedText.Width + charCount;
        }

        public double StringRenderHeight(string textValue)
        {
            FormattedText formattedText = new FormattedText(
                textValue,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                Brushes.Black);

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
                Original = value;

                if (DisplayUnderscore && value != null)
                {
                    base.Content = value.ToString().Replace("_", "__");

                    return;
                }

                base.Content = value;
            }
        }
    }
}
