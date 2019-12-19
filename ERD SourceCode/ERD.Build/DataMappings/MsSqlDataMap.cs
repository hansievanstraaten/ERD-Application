using ERD.Models;
using System.Data;

namespace ERD.Build.DataMappings
{
  internal static class MsSqlDataMap
  {
    internal static bool IsNullable(this ColumnObjectModel column)
    {
      if (!column.AllowNulls)
      {
        return false;
      }

      switch (column.SqlDataType)
      {
        case SqlDbType.Binary:
        case SqlDbType.Char:
        case SqlDbType.NChar:
        case SqlDbType.NText:
        case SqlDbType.NVarChar:
        case SqlDbType.Text:
        case SqlDbType.VarBinary:
        case SqlDbType.VarChar:
        case SqlDbType.Timestamp:
        case SqlDbType.Xml:
          return false;

        default:
          return true;

      }
    }

    internal static string GetMsSqlDataMap(SqlDbType sqlType)
    {
      switch (sqlType)
      {
        case SqlDbType.Char:
        case SqlDbType.NChar:
        case SqlDbType.VarChar:
        case SqlDbType.Text:
        case SqlDbType.Xml:
        case SqlDbType.NVarChar:
        case SqlDbType.NText:
          return "string";

        case SqlDbType.Bit:
          return "bool";

        case SqlDbType.Decimal:
        case SqlDbType.Variant:
        case SqlDbType.Money:
        case SqlDbType.SmallMoney:
          return "decimal";

        case SqlDbType.BigInt:

          return "Int64";

        case SqlDbType.TinyInt:
          return "byte";

        case SqlDbType.SmallInt:
          return "Int32";

        case SqlDbType.Int:
          return "int";

        case SqlDbType.Real:
          return "Single";

        case SqlDbType.Float:
          return "double";

        case SqlDbType.UniqueIdentifier:
          return "Guid";

        case SqlDbType.VarBinary:
        case SqlDbType.Binary:
        case SqlDbType.Timestamp:
        case SqlDbType.Image:
          return "byte[]";

        case SqlDbType.Date:
        case SqlDbType.DateTime2:
        case SqlDbType.DateTime:
        case SqlDbType.SmallDateTime:
        case SqlDbType.Udt:
          return "DateTime";

        case SqlDbType.DateTimeOffset:
          return "DateTimeOffset";


        case SqlDbType.Time:
          return "TimeSpan";

        case SqlDbType.Structured:
        default:
          return "Oeps! Not Mapped";
      }
    }
  }
}
