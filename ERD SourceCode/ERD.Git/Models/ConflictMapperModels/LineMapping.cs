using ERD.Git.Enums;

namespace ERD.Git.Models.ConflictMapperModels
{
    public class LineMapping
    {
        private bool? isSelected = null;

        public int LineIndex { get; set; }

        public string? Text { get; set; }

        public DiffItem? Diff { get; set; }

        public bool IsSelected
        {
            get
            {
                if (this.isSelected.HasValue)
                {
                    return this.isSelected.Value;
                }

                if (this.Diff == null)
                {
                    return false;
                }

                return true;
            }

            set
            {
                this.isSelected = value;
            }
        }
    }
}
