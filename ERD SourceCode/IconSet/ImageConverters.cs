using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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

        public static ImageSource ConvertToImageSource(this Bitmap img)
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
        }

        public static Bitmap ConvertImageSourceToBitmap(this ImageSource source)
        {
            Bitmap result = null;

            InteropBitmap src = (InteropBitmap)source;

            int height = src.PixelHeight;

            int width = src.PixelWidth;

            int stride = width * ((src.Format.BitsPerPixel + 7) / 8);

            IntPtr prt = Marshal.AllocHGlobal(height * stride);

            try
            {
                src.CopyPixels(new Int32Rect(0, 0, width, height), prt, height * stride, stride);

                result = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, prt);

                long handle = (long)result.GetHbitmap();

                GC.AddMemoryPressure(handle > 0 ? handle : handle * -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    //Marshal.Release(prt);
                }
                catch
                {
                    // ignored
                }

                src = null;

                source = null;
            }

            return result;
        }

        public static string ConvertImageSourceToString(this ImageSource source)
        {
            if (source is not BitmapSource bitmapSource)
                return string.Empty;

            using var stream = new MemoryStream();

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);

            return Convert.ToBase64String(stream.ToArray());
        }

        public static ImageSource ConvertStringToImageSource(this string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value))
                    return IconSets.ResourceImageSource("DefaultImage", 25);

                byte[] bytes = Convert.FromBase64String(value);

                using var stream = new MemoryStream(bytes);

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
            catch
            {
                return IconSets.ResourceImageSource("DefaultImage", 25);
            }
        }
    }
}
