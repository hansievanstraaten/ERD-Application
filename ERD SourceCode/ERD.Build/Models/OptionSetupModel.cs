using ERD.Build.BuildEnums;
using System.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace ERD.Build.Models
{
  [ModelNameAttribute("Setup")]
  public class OptionSetupModel : ModelsBase
  {
    private bool overrideIfExists;
    private string outputFileName;
    private string outputDirectory;
    private BuildTypeModel[] buildTypes;
    private RepeatOptionEnum repreatOption;
    private LanguageOptionEnum languageOption;

    public string OptionModelName { get; set;}

    [FieldInformationAttribute("File Name E.g. '[[CanvasName]].cs'", Sort = 1)]
    public string OutputFileName
    {
      get
      {
        return this.outputFileName;
      }

      set
      {
        this.outputFileName = value;

        base.OnPropertyChanged(() => this.OutputFileName);
      }
    }

    [FieldInformationAttribute("Output Directory", Sort = 2)]
    [BrowseButtonAttribute("OurputDirectoryKey", "", "Search")]
    public string OutputDirectory
    {
      get
      {
        return this.outputDirectory;
      }

      set
      {
        this.outputDirectory = value;

        base.OnPropertyChanged(() => this.OutputDirectory);
      }
    }

    [FieldInformationAttribute("Repeat Option", Sort = 3)]
    public RepeatOptionEnum RepeatOption
    {
      get
      {
        return this.repreatOption;
      }

      set
      {
        this.repreatOption = value;

        base.OnPropertyChanged(() => this.RepeatOption);
      }
    }

    [FieldInformationAttribute("Override File If Exists", Sort = 4)]
    public bool OverrideIfExists
    {
      get
      {
        return this.overrideIfExists;
      }

      set
      {
        this.overrideIfExists = value;

        base.OnPropertyChanged(() => this.OverrideIfExists);
      }
    }

    [FieldInformationAttribute("Language Option", Sort = 5, IsRequired = true)]
    public LanguageOptionEnum LanguageOption
    {
      get
      {
        return this.languageOption;
      }

      set
      {
        this.languageOption = value;

        base.OnPropertyChanged(() => this.LanguageOption);
      }
    }


    public BuildTypeModel[] BuildTypes
    {
      get
      {
        if (this.buildTypes == null)
        {
          this.buildTypes = new BuildTypeModel[] { };
        }

        return this.buildTypes.OrderBy(b => b.BuildTypeIndex).ToArray();
      }

      set
      {
        this.buildTypes = value;

        base.OnPropertyChanged(() => this.BuildTypes);
      }
    }

  }
}
