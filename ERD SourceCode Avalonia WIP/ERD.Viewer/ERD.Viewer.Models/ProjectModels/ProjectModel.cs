using ERD.Viewer.Models.Attributes;

namespace ERD.Viewer.Models.ProjectModels
{
    [ModelName("Project Setup")]
    public class ProjectModel : ModelBase
    {
        private string modelName = string.Empty;
        private string fileDirectory = string.Empty;
        private bool allowRelation;
        private bool allowVertualRelation;
        private bool keepColumnsUnique;
        private bool lockCanvasOnEditing;

        [FieldInformation("Project Name", IsRequired = true, Sort = 1)]
        public string ModelName
        {
            get
            {
                return modelName;
            }

            set
            {
                base.OnPropertyChanged("ModelName", ref modelName, value);
            }
        }

        [FieldInformation("File Directory", IsRequired = true, Sort = 2)]
        [BrowseButton("DirectoryBrowse", "...", "SearchFolder")]
        public string FileDirectory
        {
            get
            {
                return fileDirectory;
            }

            set
            {
                base.OnPropertyChanged("FileDirectory", ref fileDirectory, value);
            }
        }

        [FieldInformation("Allow Database Relations", Sort = 3)]
        public bool AllowRelations
        {
            get
            {
                return allowRelation;
            }

            set
            {
                base.OnPropertyChanged("AllowRelations", ref allowRelation, value);
            }
        }

        [FieldInformation("Allow Virtual Relations", Sort = 4)]
        public bool AllowVertualRelations
        {
            get
            {
                return allowVertualRelation;
            }

            set
            {
                base.OnPropertyChanged("AllowVertualRelations", ref allowVertualRelation, value);
            }
        }

        [FieldInformation("Keep Columns Unique", Sort = 5)]
        public bool KeepColumnsUnique
        {
            get
            {
                return keepColumnsUnique;
            }

            set
            {
                base.OnPropertyChanged("KeepColumnsUnique", ref keepColumnsUnique, value);
            }
        }

        [FieldInformation("Lock Canvas on Editing", Sort = 6)]
        public bool LockCanvasOnEditing
        {
            get
            {
                return lockCanvasOnEditing;
            }

            set
            {
                base.OnPropertyChanged("LockCanvasOnEditing", ref lockCanvasOnEditing, value);
            }
        }
    }
}
