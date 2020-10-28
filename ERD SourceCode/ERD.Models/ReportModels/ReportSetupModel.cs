using ViSo.SharedEnums.ReportEnums;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace ERD.Models.ReportModels
{
    [ModelName("Reports Setup")]
    public class ReportSetupModel : ModelsBase
    {
        private StorageTypeEnum storageType;
        private string fileDirectory;
        private DatabaseModel dataBaseSource;

        public ReportSetupModel()
        {
            this.DataBaseSource = new DatabaseModel();
        }

        [FieldInformation("Database Type", IsRequired = true)]
        public StorageTypeEnum StorageType
        {
            get
            {
                return this.storageType;
            }

            set
            {
                this.storageType = value;

                base.OnPropertyChanged(() => this.StorageType);
            }
        }

        [FieldInformation("DB File Location")]
        [BrowseButton("SaveDirectory", "Open Folder", "Search")]
        public string FileDirectory
        {
            get
            {
                return this.fileDirectory;
            }

            set
            {
                this.fileDirectory = value;

                base.OnPropertyChanged(() => this.FileDirectory);
            }
        }

        //[FieldInformation("Database Setup")]
        public DatabaseModel DataBaseSource
        {
            get
            {
                return this.dataBaseSource;
            }

            set
            {
                this.dataBaseSource = value;

                base.OnPropertyChanged(() => this.DataBaseSource);
            }
        }
    }
}
