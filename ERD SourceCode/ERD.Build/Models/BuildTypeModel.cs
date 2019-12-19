using ERD.Build.BuildEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace ERD.Build.Models
{
  [ModelNameAttribute("Build Type", AllowHeaderCollapse = true)]
  public class BuildTypeModel : ModelsBase
  {
    private int buildTypeIndex;
    private string buildTypeName;
    private RepeatTypeEnum repeatType;
    private string code;

    public int BuildTypeIndex
    {
      get
      {
        return this.buildTypeIndex;
      }

      set
      {
        this.buildTypeIndex = value;

        base.OnPropertyChanged(() => this.BuildTypeIndex);
      }
    }

    public string BuildTypeName
    {
      get
      {
        return this.buildTypeName;
      }

      set
      {
        this.buildTypeName = value;

        base.OnPropertyChanged(() => this.BuildTypeName);
      }
    }
    
    [FieldInformationAttribute("Repeat", Sort = 1)]
    public RepeatTypeEnum RepeatType
    {
      get
      {
        return this.repeatType;
      }

      set
      {
        this.repeatType = value;

        base.OnPropertyChanged(() => this.RepeatType);
      }
    }



    [FieldInformationAttribute("Code", Sort = 5)]
    [BrowseButtonAttribute("EditCodeKey", "", "Edit")]
    public string Code
    {
      get
      {
        return this.code;
      }

      set
      {
        this.code = value;

        base.OnPropertyChanged(() => this.Code);
      }
    }

  }
}
