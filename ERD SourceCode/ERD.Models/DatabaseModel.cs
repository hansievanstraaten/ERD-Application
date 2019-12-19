using ERD.Base;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;

namespace ERD.Models
{
  [ModelName("Database Setup")]
  public class DatabaseModel : UserNameAndPasswordModel
  {
    private string serverName;
    private string databaseName;
    private bool trustedConnection;
    private DatabaseTypeEnum databaseType;

    [FieldInformation("Database Type", IsRequired = true, Sort = 0)]
    public DatabaseTypeEnum DatabaseType
    {
      get
      {
        return this.databaseType;
      }

      set
      {
        if (this.databaseType != value)
        {
          base.HasModelChanged = true;
        }

        this.databaseType = value;

        base.OnPropertyChanged("DatabaseType");
      }
    }

    [FieldInformation("Server Name", IsRequired = true, Sort = 1)]
    public string ServerName
    {
      get
      {
        return this.serverName;
      }

      set
      {
        base.OnPropertyChanged("ServerName", ref this.serverName, value);
      }
    }

    [FieldInformation("Database Name", IsRequired = true, Sort = 2)]
    public string DatabaseName
    {
      get
      {
        return this.databaseName;
      }

      set
      {
        base.OnPropertyChanged("DatabaseName", ref this.databaseName, value);
      }
    }

    [FieldInformation("User Name", IsRequired = true, Sort = 3)]
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

    [FieldInformation("Password", IsRequired = true, Sort = 4)]
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

    [FieldInformation("Trusted Connection", Sort = 5)]
    public bool TrustedConnection
    {
      get
      {
        return this.trustedConnection;
      }

      set
      {
        base.OnPropertyChanged("TrustedConnection", ref this.trustedConnection, value);
      }
    }
  }
}
