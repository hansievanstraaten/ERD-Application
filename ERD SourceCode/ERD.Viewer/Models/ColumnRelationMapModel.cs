using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERD.Viewer.Enumiration;
using WPF.Tools.Attributes;

namespace ERD.Viewer.Models
{
  [ModelName("Foreign Key")]
  public class ColumnRelationMapModel : ModelBase
  {
    private string foreignConstraintName;
    private string parenttable;
    private string parentColumn;
    private string childtable;
    private string childColumn;

    [FieldInformation("Constraint Name", DisableSpellChecker = true)]
    public string ForeignConstraintName
    {
      get
      {
        return this.foreignConstraintName;
      }

      set
      {
        base.OnPropertyChanged("ForeignConstraintName", ref this.foreignConstraintName, value);
      }
    }

    [FieldInformation("Parent Table", DisableSpellChecker = true)]
    public string ParentTable
    {
      get
      {
        return this.parenttable;
      }

      set
      {
        base.OnPropertyChanged("ParentTable", ref this.parenttable, value);
      }
    }
    
    [FieldInformation("Parent Column", DisableSpellChecker = true)]
    public string ParentColumn
    {
      get
      {
        return this.parentColumn;
      }

      set
      {
        base.OnPropertyChanged("ParentColumn", ref this.parentColumn, value);
      }
    }

    [FieldInformation("Child Table", DisableSpellChecker = true)]
    public string ChildTable
    {
      get
      {
        return this.childtable;
      }

      set
      {
        base.OnPropertyChanged("ChildTable", ref this.childtable, value);
      }
    }

    [FieldInformation("Child Column", DisableSpellChecker = true)]
    public string ChildColumn
    {
      get
      {
        return this.childColumn;
      }

      set
      {
        base.OnPropertyChanged("ChildColumn", ref this.childColumn, value);
      }
    }
    
    public RelationTypesEnum RelationTypes {get; set;}
  }
}
