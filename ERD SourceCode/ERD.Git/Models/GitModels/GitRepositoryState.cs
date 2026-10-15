namespace ERD.Git.Models.GitModels
{
    public sealed class GitRepositoryState
    {
        public string CurrentOperation { get; init; } = string.Empty;

        public bool IsOperationInProgress { get; init; }

        public IReadOnlyList<string> ConflictedFiles { get; init; } = new List<string>();

        public bool HasConflicts => ConflictedFiles.Count > 0;
    }
}
