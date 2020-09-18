using System.Drawing;
using System.Windows.Media;

namespace IconSet
{
    public class ImageFiles
    {
        public static ImageSource LoadFromFile(string path)
        {
            Bitmap bmp = new Bitmap(path);

            return ImageConverters.ConvertToImageSource(bmp);
        }
    }
}
