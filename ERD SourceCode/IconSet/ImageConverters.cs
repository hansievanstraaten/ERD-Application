using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IconSet
{
  public static class ImageConverters
  {
    public static System.Drawing.Size ScaleImage(double width, double height, double scale)
    {
      System.Drawing.Size result = new System.Drawing.Size();

      double ratioX = scale / width;

      double ratioY = scale / height;

      double ratio = Math.Min(ratioX, ratioY);

      result.Width = (int)(width * ratio);

      result.Height = (int)(height * ratio);

      return result;
    }

    public static ImageSource ConvertToImageSource(Bitmap img)
    {
      if (img == null)
      {
        return null;
      }

      ImageSource source = null;

      try
      {
        IntPtr handle = img.GetHbitmap();

        source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        return source;
      }
      catch
      {
        return null;
      }
      finally
      {
        source = null;
      }
      //}
    }
  }
}
