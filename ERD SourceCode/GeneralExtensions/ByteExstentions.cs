using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace GeneralExtensions
{
  public static class ByteExstentions
  {
    public static string ConvertBytesToString(this byte[] value)
    {
      if (value == null)
      {
        return string.Empty;
      }

      return Convert.ToBase64String(value);
    }

    public static object UnzipFile(this byte[] source)
    {
      object result;

      using (MemoryStream queryStream = new MemoryStream(source))
      {
        using (GZipStream zipStream = new GZipStream(queryStream, CompressionMode.Decompress))
        {
          BinaryFormatter formatter = new BinaryFormatter();

          result = formatter.Deserialize(zipStream);
        }
      }

      return result;
    }
  }
}
