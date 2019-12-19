using System;

namespace ERD.DatabaseScripts
{
  public class DataConverters
  {
    public long ConvertTimeStamp(byte[] timeStamp)
    {
      if (BitConverter.IsLittleEndian)
      {
        Array.Reverse(timeStamp);
      }

      return BitConverter.ToInt64(timeStamp, 0);
    }
  }
}
