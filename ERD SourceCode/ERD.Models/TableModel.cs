using GeneralExtensions;
using System.Windows;
using WPF.Tools.Attributes;
using System.Linq;

namespace ERD.Models
{
  [ModelName("Table")]
  public class TableModel : ModelBase
  {
    private ColumnObjectModel[] columns;

    private ColumnObjectModel[] deletedColumns;
    
    private bool isDeleted;

    private string tableName;

    private string friendlyName;

    private string erdSegmentModelName;
    
    private string description;

    private Point canvasLocation;

    public TableModel()
    {
    }

    public bool IsDeleted
    {
      get
      {
        return this.isDeleted;
      }

      set
      {
        this.isDeleted = value;

        base.OnPropertyChanged("IsDeleted");
      }
    }

    public bool IsNewTable {get; set;}

    [FieldInformation("Name", IsRequired = true, DisableSpellChecker = true)]
    public string TableName
    {
      get
      {
        return this.tableName;
      }

      set
      {
        base.OnPropertyChanged("TableName", ref this.tableName, value);

      }
    }

    [FieldInformation("Friendly Name")]
    public string FriendlyName
    {
      get
      {
        if (this.friendlyName.IsNullEmptyOrWhiteSpace())
        {
          return this.TableName;
        }

        return this.friendlyName;
      }

      set
      {
        base.OnPropertyChanged("FriendlyName", ref this.friendlyName, value);

      }
    }

    [FieldInformation("Description")]
    public string Description
    {
      get
      {
        return this.description;
      }

      set
      {
        base.OnPropertyChanged("Description", ref this.description, value);
      }
    }

    public string ErdSegmentModelName
    {
      get
      {
        return this.erdSegmentModelName;
      }

      set
      {
        base.OnPropertyChanged("ErdSegmentModelName", ref this.erdSegmentModelName, value);
      }
    }

    public string PrimaryKeyClusterConstraintName {get; set;}

    public Point CanvasLocation
    {
      get
      {
        return this.canvasLocation;
      }

      set
      {
        base.OnPropertyChangedStringCompair("CanvasLocation", ref this.canvasLocation, value);
      }
    }

    public ColumnObjectModel[] Columns
    {
      get
      {
        if (!this.columns.HasElements())
        {
          this.columns = new ColumnObjectModel[] { };
        }
        
        return this.columns.OrderBy(c => c.Column_Id).ToArray();
      }

      set
      {
        this.columns = value.OrderBy(c => c.Column_Id).ToArray();
        
        base.OnPropertyChanged("Columns");
      }
    }

    public ColumnObjectModel[] DeltedColumns
    {
      get
      {
        if (!this.deletedColumns.HasElements())
        {
          this.deletedColumns = new ColumnObjectModel[] { };
        }

        return this.deletedColumns;
      }

      set
      {
        this.deletedColumns = value;

        base.OnPropertyChanged("DeltedColumns");
      }
    }
  }
}
