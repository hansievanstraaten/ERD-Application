using ERD.Base;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;

namespace ERD.Models
{
    [ModelName("Database Setup")]
    public class DatabaseModel : UserNameAndPasswordModel
    {
        private string serverName;
        private string portName;
        private string databaseName;
        private bool trustedConnection;
        private bool sslMode;
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

        [FieldInformation("Server Name/Host", IsRequired = true, Sort = 1)]
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

        [FieldInformation("Port", IsRequired = false, Sort = 2, IsVisible = false)]
        public string PortName
        {
            get
            {
                return this.portName;
            }

            set
            {
                base.OnPropertyChanged("PortName", ref this.portName, value);
            }
        }

        [FieldInformation("Database Name", IsRequired = true, Sort = 3)]
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

        [FieldInformation("User Name", IsRequired = true, Sort = 4)]
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

        [FieldInformation("Password", IsRequired = true, Sort = 5)]
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

        [FieldInformation("Trusted Connection", Sort = 6)]
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

        [FieldInformation("SSL Mode", Sort = 7, IsVisible = false)]
        public bool SSLMode
        {
            get
            {
                return this.sslMode;
            }

            set
            {
                base.OnPropertyChanged("SSLMode", ref this.sslMode, value);
            }
        }
    }
}
