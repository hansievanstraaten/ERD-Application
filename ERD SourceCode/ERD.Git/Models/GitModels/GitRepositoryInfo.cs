namespace ERD.Git.Models.GitModels
{
    public sealed class GitRepositoryInfo
    {
        public string Path { get; init; } = string.Empty;
        public string? CurrentBranch { get; init; }
        public string? HeadCommit { get; init; }
        public bool IsDirty { get; init; }
        public bool IsBare { get; init; }
    }
}
