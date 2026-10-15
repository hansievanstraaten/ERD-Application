namespace ERD.Git.Models.ConflictMapperModels
{
    public class MappedFileModel
    {
        public Dictionary<int, LineMapping> Lines { get; set; } = new Dictionary<int, LineMapping>();
    }
}
