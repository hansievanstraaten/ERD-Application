using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;

namespace ERD.Models
{
  [ModelName("Username And Password")]
  public class UserNameAndPasswordModel : ModelsBase
  {
    private string userName;
    private string password;

    [FieldInformation("User Name", IsRequired = true, Sort = 3)]
    public string UserName
    {
      get
      {
        return this.userName;
      }

      set
      {
        base.OnPropertyChanged("UserName", ref this.userName, value);
      }
    }


    [FieldInformation("Password", IsRequired = true, Sort = 4)]
    [ItemTypeAttribute(ModelItemTypeEnum.SecureString)]
    public string Password
    {
      get
      {
        return this.password;
      }

      set
      {
        base.OnPropertyChanged("Password", ref this.password, value);
      }
    }
  }
}
