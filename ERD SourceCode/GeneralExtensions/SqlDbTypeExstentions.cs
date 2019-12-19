using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class SqlDbTypeExstentions
  {
    public static string SQLFormat(this SqlDbType dataType, int maxLength, int precision, int scale)
    {
      switch (dataType)
      {
          case SqlDbType.Binary:
          case SqlDbType.Char: 
          case SqlDbType.DateTime2:
          case SqlDbType.DateTimeOffset:
          case SqlDbType.NChar:
          case SqlDbType.NVarChar:
          case SqlDbType.VarBinary:
          case SqlDbType.VarChar:
          return $"{dataType.ToString()}({(maxLength < 0 ? "MAX" : maxLength.ToString())})";

          case SqlDbType.Variant:
            return $"numeric({precision}, {scale})";

          case SqlDbType.Decimal:
            return $"{dataType.ToString()}({precision}, {scale})";

          default:
            return dataType.ToString();
      }
    }
  }
}
