namespace ERD.Viewer.Tools.Converters
{
    public class ColourConverters
    {
        public static Avalonia.Media.Color GetFromHex(string hex)
        {
            return Avalonia.Media.Color.Parse(hex);
        }
    }
}
