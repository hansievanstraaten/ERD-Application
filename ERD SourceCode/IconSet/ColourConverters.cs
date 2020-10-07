//using System.Drawing;
using System.Windows.Media;

namespace IconSet
{
    public static class ColourConverters
    {
        public static Color GetFromHex(string hex)
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }

        public static Brush GetBrushfromHex(string hex)
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom(hex));
        }

        public static string GetHexFromBrush(Brush brush)
        {
            return ((SolidColorBrush)brush).Color.ToString();
        }

        public static Brush InvertFromHex(string hex)
        {
            Color color = GetFromHex(hex);

            Color resultColor = color.InvertColor();

            return GetBrushfromHex(resultColor.ToString());

        }

        public static Color InvertColor(this Color c) => Color.FromArgb(c.A.Invert(), c.R.Invert(), c.G.Invert(), c.B.Invert());

        private static byte Invert(this byte b)
        {
            unchecked
            {
                return (byte)(b + 128);
            }
        }
    }
}
