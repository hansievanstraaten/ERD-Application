using GeneralExtensions;
using IconSet.Properties;
using System.Drawing;
using System.Windows.Media;

namespace IconSet
{
  public static class IconSets
  {
    public static Bitmap ResourceBitmap(string imageName)
    {
      if (imageName == null || imageName.IsNullEmptyOrWhiteSpace())
      {
        return null;
      }

      Bitmap img = null;

      try
      {
        img = (Bitmap)Resources.ResourceManager.GetObject(imageName);

        if (img == null)
        {
          return null;
        }
      }
      catch
      {
        return null;
      }

      return img; 
    }
    
    public static ImageSource ResourceImageSource(string imageName, double scale)
    {
      using (Bitmap img = IconSets.ResourceBitmap(imageName))
      {
        if(img == null)
        {
          return null;
        }

        Size smallSize = ImageConverters.ScaleImage(img.Width, img.Height, scale);

        using (Bitmap resized = new Bitmap(img, smallSize))
        {
          return ImageConverters.ConvertToImageSource(resized);
        }
      }
    }

  }
}
