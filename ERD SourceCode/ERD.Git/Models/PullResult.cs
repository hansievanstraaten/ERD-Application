namespace ERD.Git.Models
{
    public sealed class PullResult
    {
        public string? CommitId { get; init; }
        
        public string Status { get; init; } = string.Empty;
        
        public List<string?> ConflictedFiles { get; set; } = new List<string?>();

        public bool HasConflicts => ConflictedFiles.Count > 0;
    }
}
