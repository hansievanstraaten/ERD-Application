using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

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

            return System.Convert.ToBase64String(value);
        }

        public static object UnzipFile(this byte[] source)
        {
            object result;

            using (MemoryStream queryStream = new MemoryStream(source))
            {
                using (GZipStream zipStream = new GZipStream(queryStream, CompressionMode.Decompress))
                {
                    // TODO: The following fix uses System.Text.Json for deserialization,
                    // but it will not work if the original data was not serialized with System.Text.Json. Manual migration of the serialization
                    // format may be required.
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            zipStream.CopyTo(ms);
                            ms.Position = 0;
                            // You must know the type to deserialize to. Replace typeof(object) with the actual type if possible.
                            result = JsonSerializer.Deserialize(ms.ToArray(), typeof(object));
                        }
                    }
                    catch
                    {
                        // Support for Backward compatibility
                        return UnzipFileBinaryFormatter(source);
                    }
                }
            }

            return result;
        }

        private static object UnzipFileBinaryFormatter(byte[] source)
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
