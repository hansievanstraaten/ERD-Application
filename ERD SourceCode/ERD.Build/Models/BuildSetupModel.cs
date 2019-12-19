using System.Collections.Generic;

namespace ERD.Build.Models
{
  public class BuildSetupModel
  {
    public BuildSetupModel()
    {
      this.BuildOptions = new List<OptionSetupModel>();
    }
    
    public List<OptionSetupModel> BuildOptions;
  }
}
