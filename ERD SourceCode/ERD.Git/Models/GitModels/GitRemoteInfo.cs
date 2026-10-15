namespace ERD.Git.Models.GitModels
{
    public sealed class GitRemoteInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
        public string PushUrl { get; init; } = string.Empty;
    }
}
