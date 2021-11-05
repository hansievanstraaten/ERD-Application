using ERD.Models;
using System.Linq;
using System.Text;

namespace ERD.Viewer.Database
{
  public static class TableQueryBuilder
  {
    public static string BuildSelectTop(TableModel table, int topValue = 100)
    {
      if (table.Columns.Count() == 0)
      {
        return string.Empty;
      }

      StringBuilder result = new StringBuilder();

      for(int x = 0; x < table.Columns.Length; x++)
      {
        if (x == 0)
        {
          result.AppendLine($"SELECT TOP {topValue} [{table.Columns[x].ColumnName}], ");
        }
        else
        {
          result.AppendLine($"                           [{table.Columns[x].ColumnName}], ");
        }
      }

      int removeIndex = result.Length - 4;

      result.Remove(removeIndex, 4);

      result.AppendLine();

      result.AppendLine($" FROM [{table.SchemaName}].[{table.TableName}]");

      return result.ToString();
    }
  }
}
