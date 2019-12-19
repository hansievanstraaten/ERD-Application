using System.Collections.Generic;
using WPF.Tools.Attributes;
using GeneralExtensions;

namespace ERD.Viewer.Models
{
  [ModelName("Canvas")]
  public class ErdCanvasModel
  {
    public ErdCanvasModel()
    {
      this.IncludeInContextBuild = new List<IncludeTableModel>();
    }

    [FieldInformation("Tab Name", IsRequired = true)]
    public string ModelSegmentName {get; set;}

    [FieldInformation("Table Prefix")]
    public string TablePrefix {get; set;}

    public string ModelSegmentControlName
    {
      get
      {
        if (this.ModelSegmentName.IsNullEmptyOrWhiteSpace())
        {
          return string.Empty;
        }

        return this.ModelSegmentName.MakeAlphaNumeric();
      }
    }

    public List<TableModel> SegmentTables {get; set;}

    public List<IncludeTableModel> IncludeInContextBuild { get; set; }
  }
}
