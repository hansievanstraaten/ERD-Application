using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERD.Viewer.Database
{
  internal class DataConverters
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
