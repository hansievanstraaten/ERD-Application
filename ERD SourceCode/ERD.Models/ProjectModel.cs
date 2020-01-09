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

    [FieldInformation("Project Name", IsRequired = true)]
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

    [FieldInformation("File Directory", IsRequired = true)]
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
    
    [FieldInformation("Allow Database Relations")]
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

    [FieldInformation("Allow Virtual Relations")]
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

    [FieldInformation("Keep Columns Unique")]
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
  }
}
