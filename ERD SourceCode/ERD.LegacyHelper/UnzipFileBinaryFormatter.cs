using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace ERD.LegacyHelper
{
    internal class UnzipFileBinaryFormatter
    {
        internal int UnzipFile(string[] args)
        {
            byte[] bytes = Convert.FromBase64String(args[0]);

            object obj;
            using (MemoryStream ms = new MemoryStream(bytes))
            using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
            {
#pragma warning disable SYSLIB0011
                BinaryFormatter bf = new BinaryFormatter();
                obj = bf.Deserialize(gz);
#pragma warning restore SYSLIB0011
            }

            Console.Write(obj?.ToString() ?? string.Empty);
            return 0;
        }
    }
}
