using WPF.Tools.Attributes;

namespace ERD.Git.Models.GitModels
{
    public class GitCommitRequest : GitIdentity
    {
        [FieldInformation("Comment", IsRequired = true, Sort = 0)]
        public string CommitMessage { get; set; } = string.Empty;
    }
}
