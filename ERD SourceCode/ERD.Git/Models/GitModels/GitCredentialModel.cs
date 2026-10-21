using WPF.Tools.Attributes;

namespace ERD.Git.Models.GitModels
{
    public class GitCredentialModel
    {
        [FieldInformation("User Name", IsRequired = false, Sort = 2)]
        public string? Username { get; set; } = string.Empty;


        [FieldInformation("Password or Personal Access Token.", IsRequired = false, Sort = 3)]
        [BrowseButtonAttribute("Password", "", "QuestionMark")]
        public string? Password { get; set; } = string.Empty;
    }
}
