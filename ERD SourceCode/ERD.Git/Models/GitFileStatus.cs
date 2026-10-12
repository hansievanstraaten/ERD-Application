namespace ERD.Git.Models
{
    public sealed class GitFileStatus
    {
        public string Path { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
    }
}
