using ERD.Git.Enums;

namespace ERD.Git.Models.ConflictMapperModels
{
    public class DiffItem
    {
        public int Index { get; set; }

        public string? Value { get; set; }

        public FileLocationEnum FileLocation { get; set; }
    }
}
