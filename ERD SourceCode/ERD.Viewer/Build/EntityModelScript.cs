using ERD.Models;
using ERD.Models.BuildModels.EntityFrameworkModels;
using GeneralExtensions;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace ERD.Viewer.Build
{
  public static class EntityModelScript
  {
    public static EntityFrameworkSetup Setup {get; set;}

    public static string ScriptServerModelBase(TableModel table)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine("using System.ComponentModel.DataAnnotations;");

      result.AppendLine(Setup.ModelClassBaseUsing);

      result.AppendLine();

      result.AppendLine($"namespace {Setup.ModelClassNamespace}");

      result.AppendLine("{");

      result.AppendLine($"    {EntityModelScript.GetModelClassBaseString(table)}");
      result.AppendLine("    {");

      //private {1} _{2};

      foreach (ColumnObjectModel column in table.Columns)
      {
        
      }

      foreach (ColumnObjectModel column in table.Columns)
      {
        result.AppendLine($"{EntityModelScript.GetModelPropertyString(column)}");

        result.AppendLine();
      }

      result.AppendLine("    }");

      result.Append("}");

      return result.ToString();
    }

    public static string ScriptServerModel(TableModel table)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
      result.AppendLine();

      result.AppendLine($"namespace {Setup.ModelClassNamespace}");

      result.AppendLine("{");

      result.AppendLine($"    {EntityModelScript.GetModelClassString(table)}");
      result.AppendLine("    {");

      result.AppendLine("        // NOTE: Add the [NotMapped] Attribute to all properties in this class to ensure that Entity Framework does not fall over");
      result.AppendLine("    }");

      result.Append("}");

      return result.ToString();
    }

    public static string ScriptServerModelMapping(TableModel table)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(Setup.MappingClassUsing);

      result.AppendLine();

      result.AppendLine($"namespace {Setup.MappingClassNamespace}");

      result.AppendLine("{");

      result.AppendLine($"    public class {EntityModelScript.GetClassName(table)}Mapping : EntityTypeConfiguration<{EntityModelScript.GetClassName(table)}>");
      result.AppendLine("    {");

      result.AppendLine($"        public {EntityModelScript.GetClassName(table)}Mapping()");
      result.AppendLine("        {");

      result.AppendLine($"            ToTable(\"{table.TableName}\");");
      result.AppendLine();

      ColumnObjectModel[] pkColumns = table.Columns.Where(pk => pk.InPrimaryKey).ToArray();

      if (pkColumns.HasElements() && pkColumns.Length == 1)
      {
        result.AppendLine($"            HasKey(k => k.{EntityModelScript.GetColumnName(pkColumns[0])});");
      }
      else if (pkColumns.HasElements())
      {
        result.Append("            HasKey(k => new { ");

        for (int x = 0; x < pkColumns.Length; ++x)
        {
          if (x == (pkColumns.Length - 1))
          {
            result.Append($"k.{EntityModelScript.GetColumnName(pkColumns[x])} ");
          }
          else
          {
            result.Append($"k.{EntityModelScript.GetColumnName(pkColumns[x])}, ");
          }
        }

        result.AppendLine(" });");
      }

      result.AppendLine();

      for (int x = 0; x < table.Columns.Length; x++)
      {
        ColumnObjectModel column = table.Columns[x];

        string lambda = $"col{x}";

        result.AppendLine($"            Property({lambda} => {lambda}.{EntityModelScript.GetColumnName(column)}).HasColumnName(\"{column.ColumnName}\");");
      }

      result.AppendLine("        }");

      result.AppendLine("    }");

      result.Append("}");

      return result.ToString();
    }
    
    public static string GetClassName(TableModel table)
    {
      return EntityModelScript.Setup.UseFriendlyNames && !table.FriendlyName.IsNullEmptyOrWhiteSpace() ?
        table.FriendlyName.Replace(' ', '_').MakeAlphaNumeric() : table.TableName.Replace(' ', '_').MakeAlphaNumeric();
    }

    public static string GetClassName(IncludeTableModel table)
    {
      return EntityModelScript.Setup.UseFriendlyNames && !table.FriendlyName.IsNullEmptyOrWhiteSpace() ?
        table.FriendlyName.Replace(' ', '_').MakeAlphaNumeric() : table.TableName.Replace(' ', '_').MakeAlphaNumeric();
    }

    public static string GetColumnName(ColumnObjectModel column)
    {
      return EntityModelScript.Setup.UseFriendlyNames && !column.FriendlyName.IsNullEmptyOrWhiteSpace() ?
        column.FriendlyName.Replace(' ', '_').MakeAlphaNumeric() : column.ColumnName.Replace(' ', '_').MakeAlphaNumeric();
    }

    public static string[] GetColumnDotNetDescriptor(ColumnObjectModel column)
    {
      return new string[]
      {
        EntityModelScript.GetSqlDataMap(column.SqlDataType.Value),
        EntityModelScript.GetColumnName(column)
      };
    }

    private static string GetModelClassBaseString(TableModel table)
    {
      return string.Format(EntityModelScript.Setup.ModelClassBaseString, EntityModelScript.GetClassName(table));
    }

    private static string GetModelClassString(TableModel table)
    {
      return string.Format(EntityModelScript.Setup.ModelClassString, EntityModelScript.GetClassName(table));
    }

    //private static string GetModelFieldString(ColumnObjectModel column)
    //{
      
    //}

    private static string GetModelPropertyString(ColumnObjectModel column)
    {
      string sqlDataType = EntityModelScript.IsNullable(column) ?
        $"{EntityModelScript.GetSqlDataMap(column.SqlDataType.Value)}{(column.InPrimaryKey ? string.Empty : "?")}" : 
        EntityModelScript.GetSqlDataMap(column.SqlDataType.Value);

      string inPrimaryKey = column.InPrimaryKey ? "(Primary Key) " : string.Empty;

      string isForeignKey = column.IsForeignkey ? $"(Foreign Key from: {column.ForeignKeyTable}) " : string.Empty;

      string description = $"{inPrimaryKey}{isForeignKey}{column.Description}";

      if (column.SqlDataType.Value == SqlDbType.Timestamp)
      {
        string resultSting = EntityModelScript.Setup.ModelPropertyString
          .Replace("{0}", description)
          .Replace("{1}", sqlDataType)
          .Replace("{2}", EntityModelScript.GetColumnName(column));

        int insertIndex = resultSting.IndexOf("</summary>") + 10;

        resultSting =  resultSting.Insert(insertIndex, $"{Environment.NewLine}        [Timestamp]");

        return resultSting.ToString();
      }

      // NOTE: We need to do a replace due to the brackets in the text here
      return EntityModelScript.Setup.ModelPropertyString
        .Replace("{0}", description)
        .Replace("{1}", sqlDataType)
        .Replace("{2}", EntityModelScript.GetColumnName(column));
    }

    private static bool IsNullable(ColumnObjectModel column)
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

    private static string GetSqlDataMap(SqlDbType sqlType)
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


//Func<Object, SqlDbType> getSqlType = val => new SqlParameter("Test", val).SqlDbType;
//Func<Type, SqlDbType> getSqlType2 = type => new SqlParameter("Test", type.IsValueType?Activator.CreateInstance(type):null).SqlDbType;

////returns nvarchar...
//Object obj = "valueToTest";
//getSqlType(obj).Dump();
//getSqlType2(typeof(String)).Dump();

////returns int...
//obj = 4;
//getSqlType(obj).Dump();
//getSqlType2(typeof(Int32)).Dump();

////returns bigint...
//obj = Int64.MaxValue;
//getSqlType(obj).Dump();
//getSqlType2(typeof(Int64)).Dump();
