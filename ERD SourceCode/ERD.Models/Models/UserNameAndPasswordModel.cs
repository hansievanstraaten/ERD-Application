using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;

namespace ERD.Viewer.Models
{
  [ModelName("Username And Password")]
  public class UserNameAndPasswordModel : ModelBase
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
