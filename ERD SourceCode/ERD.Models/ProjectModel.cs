using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.Attributes;

namespace ERD.Models
{
  [ModelName("Project Setup")]
  public class ProjectModel : ModelBase
  {
    private string modelName;
    private string fileDirectory;
    private bool allowRelation;
    private bool allowVertualRelation;
    private bool keepColumnsUnique;
    private bool lockCanvasOnEditing;

    [FieldInformation("Project Name", IsRequired = true, Sort = 1)]
    public string ModelName
    {
      get
      {
        return this.modelName;
      }

      set
      {
        base.OnPropertyChanged("ModelName", ref this.modelName, value);
      }
    }

    [FieldInformation("File Directory", IsRequired = true, Sort = 2)]
    [BrowseButtonAttribute("DirectoryBrowse", "...", "SearchFolder")]
    public string FileDirectory 
    {
      get
      {
        return this.fileDirectory;
      }

      set
      {
        base.OnPropertyChanged("FileDirectory", ref this.fileDirectory, value);
      }
    }
    
    [FieldInformation("Allow Database Relations", Sort = 3)]
    public bool AllowRelations
    {
      get
      {
        return this.allowRelation;
      }

      set
      {
        base.OnPropertyChanged("AllowRelations", ref this.allowRelation, value);
      }
    }

    [FieldInformation("Allow Virtual Relations", Sort = 4)]
    public bool AllowVertualRelations
    {
      get
      {
        return this.allowVertualRelation;
      }

      set
      {
        base.OnPropertyChanged("AllowVertualRelations", ref this.allowVertualRelation, value);
      }
    }

    [FieldInformation("Keep Columns Unique", Sort = 5)]
    public bool KeepColumnsUnique
    {
      get
      {
        return this.keepColumnsUnique;
      }

      set
      {
        base.OnPropertyChanged("KeepColumnsUnique", ref this.keepColumnsUnique, value);
      }
    }

    [FieldInformation("Lock Canvas on Editing", Sort = 6)]
    public bool LockCanvasOnEditing
    {
      get
      {
        return this.lockCanvasOnEditing;
      }

      set
      {
        base.OnPropertyChanged("LockCanvasOnEditing", ref this.lockCanvasOnEditing, value);
      }
    }
  }
}
