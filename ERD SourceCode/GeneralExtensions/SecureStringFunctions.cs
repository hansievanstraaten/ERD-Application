using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class SecureStringFunctions
  {
    public static string UnSecureSecureString(this SecureString value)
    {
      if (value == null || value.Length == 0)
      {
        return string.Empty;
      }

      IntPtr binaryString = Marshal.SecureStringToBSTR(value);

      try
      {
        return Marshal.PtrToStringBSTR(binaryString);
      }
      finally
      {
        Marshal.FreeBSTR(binaryString);
      }
    }

    public static string Encrypt(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return string.Empty;
      }

      return value.ZipFile().ConvertBytesToString();
    }

    public static string Decrypt(this string value)
    {
      if (value.IsNullEmptyOrWhiteSpace())
      {
        return string.Empty;
      }

      return value.ConvertStringToBytes().UnzipFile().ParseToString();
    }
  }
}
