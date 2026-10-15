namespace ERD.Git.Models.GitModels
{
    public sealed class GitBranchInfo
    {
        public string Name { get; init; } = string.Empty;
        public string CanonicalName { get; init; } = string.Empty;
        public string RemoteName { get; init; } = string.Empty;
        public bool IsRemote { get; init; }
        public bool IsCurrent { get; init; }
        public bool IsTracking { get; init; }
        public string? TrackedBranch { get; init; }
        public string? CommitSha { get; init; }
        public int? AheadBy { get; init; }
        public int? BehindBy { get; init; }
    }
}
