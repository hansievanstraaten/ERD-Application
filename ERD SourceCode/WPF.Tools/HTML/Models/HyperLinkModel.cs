using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace WPF.Tools.HTML.Models
{
  [ModelNameAttribute("Hyperlink")]
  public class HyperLinkModel : ModelsBase
  {
    private string displayName;
    private string url;

    [FieldInformationAttribute("Display Text", IsRequired = true)]
    public string DisplayName 
    {
      get
      {
        return this.displayName;
      }

      set
      {
        this.displayName = value;

        base.OnPropertyChanged("DisplayName");
      }
    }

    [FieldInformationAttribute("Url", IsRequired = true)]
    public string Url
    {
      get
      {
        return this.url;
      }

      set
      {
        this.url = value;

        base.OnPropertyChanged("Url");
      }
    }
  }
}
