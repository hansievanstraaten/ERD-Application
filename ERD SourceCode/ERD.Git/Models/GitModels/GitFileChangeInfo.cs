namespace ERD.Git.Models.GitModels
{
    public class GitFileChangeInfo
    {
        public string FilePath { get; set; } = string.Empty;
        
        public string ChangeType { get; set; } = string.Empty;
        
        public int LinesAdded { get; set; }
        
        public int LinesDeleted { get; set; }

        public string ChangeIndicator
        {
            get
            {
                if (this.LinesAdded > 0 && this.LinesDeleted > 0)
                {
                    return "~";
                }
                else if (this.LinesAdded > 0)
                {
                    return "A";
                }
                else if (this.LinesDeleted > 0)
                {
                    return "D";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string? PreviousContent { get; internal set; }

        public string? CommitedContent { get; internal set; }
    }
}
