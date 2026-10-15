namespace ERD.Git.Models.GitModels
{
    public sealed class PullResult
    {
        public string? CommitId { get; init; }
        
        public string Status { get; init; } = string.Empty;
        
        public IReadOnlyList<string> ConflictedFiles { get; init; } = new List<string>();

        public bool HasConflicts => ConflictedFiles.Count > 0;
    }
}
