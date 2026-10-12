namespace ERD.Git.Models
{
    public sealed class GitCommitInfo
    {
        public string Sha { get; init; } = string.Empty;
        public string ShortSha { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string AuthorEmail { get; init; } = string.Empty;
        public DateTimeOffset Date { get; init; }
    }
}
