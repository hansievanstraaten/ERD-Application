namespace ERD.Git.Models.GitModels
{
    public sealed class GitStashInfo
    {
        public int Index { get; init; }
        public string Message { get; init; } = string.Empty;
    }
}
