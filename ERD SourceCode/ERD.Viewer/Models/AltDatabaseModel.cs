using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;

namespace ERD.Viewer.Models
{
  [ModelName("Alternative Database Setup", true)]
  public class AltDatabaseModel : DatabaseModel
  {
    private string connectionName;

    [FieldInformation("Name", IsRequired = true, Sort = 0)]
    public string ConnectionName
    {
      get
      {
        return this.connectionName;
      }

      set
      {
        base.OnPropertyChanged("ConnectionName", ref this.connectionName, value);
      }
    }


    [FieldInformation("User Name", Sort = 3)]
    new public string UserName
    {
      get
      {
        return base.UserName;
      }

      set
      {
        base.UserName = value;
      }
    }
    

    [FieldInformation("Password", Sort = 4)]
    [ItemTypeAttribute(ModelItemTypeEnum.SecureString)]
    new public string Password
    {
      get
      {
        return base.Password;
      }

      set
      {
        base.Password = value;
      }
    }

  }
}
