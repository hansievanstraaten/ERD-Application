using System.Security.Principal;
using WPF.Tools.Attributes;

namespace ERD.Git.Models
{
    public class GitIdentity
    {
        string defaultEmail = "NoMail@gmail.com";

        [FieldInformation("User Name", IsRequired = true, IsReadOnly = true, Sort = 1)]
        public string Name
        {
            get
            { 
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                return identity?.Name ?? Environment.UserName;
            }
        }

        [FieldInformation("Email", IsRequired = true, Sort = 2)]
        public string Email 
        {
            get
            {
                return this.defaultEmail;
            }
            
            set
            {
                this.defaultEmail = value;
            }
        }
    }
}
