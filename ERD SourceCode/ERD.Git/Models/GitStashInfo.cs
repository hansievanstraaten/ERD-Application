namespace ERD.Git.Models
{
    public sealed class GitStashInfo
    {
        public int Index { get; init; }
        public string Message { get; init; } = string.Empty;
    }
}
