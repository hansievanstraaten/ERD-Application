using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace ERD.Git.Models.GitModels
{
    [ModelNameAttribute("Clone Repository")]
    public sealed class GitCloneRequestModel : ModelsBase
    {
        [FieldInformation("Git Repository URL", IsRequired = true, Sort = 0)]
        public string RemoteUrl { get; set; } = string.Empty;

        
        [FieldInformation("Repository Local Path", IsRequired = true, Sort = 1)]
        [BrowseButtonAttribute("OurputDirectoryKey", "", "Search")]
        public string LocalPath { get; set; } = string.Empty;


        [FieldInformation("User Name", IsRequired = false, Sort = 2)]
        public string? Username { get; set; } = string.Empty;


        [FieldInformation("Password or Personal Access Token.", IsRequired = false, Sort = 3)]
        [BrowseButtonAttribute("Password", "", "QuestionMark")]
        public string? Password { get; set; } = string.Empty;
    }
}
