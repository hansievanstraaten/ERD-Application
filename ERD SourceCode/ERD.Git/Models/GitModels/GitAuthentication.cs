namespace ERD.Git.Models.GitModels
{
    public sealed class GitAuthentication
    {
        public string Username { get; init; } = string.Empty;

        /// <summary>
        /// Password or Personal Access Token.
        /// </summary>
        public string Password { get; init; } = string.Empty;
    }
}
