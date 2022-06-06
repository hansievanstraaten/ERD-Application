using ERD.Common;
using ERD.Models;
using System.Collections.Generic;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace ERD.DataExport.Models
{
    [ModelName("Options")]
    public  class ExportOptions : ModelBase
    {
        private string source;

        private string description;

        private string outputDirectory;

        private char dataDelimiter = '|';

        private int topX = 0;

        private bool mergeIdentityValues;
        
        private bool placeDataInSQL;
        
        private bool mergeUpdate = true;
        
        private bool mergeInsert = true;

        private bool mergeDelete = false;

        [FieldInformation("Source", IsRequired = true, Sort = 1, IsReadOnly = true)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("SourceItems")]
        public string Source 
        { 
            get
            {
                return this.source;
            }
            
            set
            {
                this.source = value;

                base.OnPropertyChanged("Source");
            }
        }

        [FieldInformation("Destination", IsRequired = true, Sort = 2)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("DestinationItems")]
        public string Destination 
        { 
            get
            {
                return this.description;
            }
            
            set
            {
                this.description = value;

                base.OnPropertyChanged("Destination");
            }
        }

        [FieldInformation("Output Directory", IsRequired = true, Sort = 3, DisableSpellChecker = true)]
        [BrowseButtonAttribute("OutputDirectoryKey", "", "Search")]
        public string OutputDirectory
        {
            get
            {
                return this.outputDirectory;
            }

            set
            {
                this.outputDirectory = value;

                base.OnPropertyChanged("OutputDirectory");
            }
        }

        [FieldInformation("Data rows max limit", Sort = 4)]
        public int TopX 
        {
            get
            {
                return topX;
            }

            set
            {
                this.topX = value;

                base.OnPropertyChanged("TopX");
            }
        }

        [FieldInformation("Data Delimiter", IsRequired = true, Sort = 4)]
        public char DataDelimiter
        {
            get
            {
                return this.dataDelimiter;
            }

            set
            {
                this.dataDelimiter = value;

                base.OnPropertyChanged("DataDelimiter");
            }
        }

        [FieldInformation("Merge Identity Values", Sort = 5)]
        [ItemTypeAttribute(ModelItemTypeEnum.CheckBox)]
        public bool MergeIdentityValues 
        { 
            get
            {
                return this.mergeIdentityValues;
            }
            
            set
            {
                this.mergeIdentityValues = value;

                base.OnPropertyChanged("MergeIdentityValues");
            }
        }

        [FieldInformation("Place Data In SQL", Sort = 6)]
        [ItemTypeAttribute(ModelItemTypeEnum.CheckBox)]
        public bool PlaceDataInSQL
        {
            get
            {
                return this.placeDataInSQL;
            }

            set
            {
                this.placeDataInSQL = value;

                base.OnPropertyChanged("PlaceDataInSQL");
            }
        }

        [FieldInformation("Update existing rows in target.", Sort = 7)]
        [ItemTypeAttribute(ModelItemTypeEnum.CheckBox)]
        public bool MergeUpdate
        {
            get
            {
                return this.mergeUpdate;
            }

            set
            {
                this.mergeUpdate = value;

                base.OnPropertyChanged("MergeUpdate");
            }
        }

        [FieldInformation("Insert new rows in target.", Sort = 8)]
        [ItemTypeAttribute(ModelItemTypeEnum.CheckBox)]
        public bool MergeInsert
        {
            get
            {
                return this.mergeInsert;
            }

            set
            {
                this.mergeInsert = value;

                base.OnPropertyChanged("MergeInsert");
            }
        }

        [FieldInformation("Delete unmatched row from target.", Sort = 9)]
        [ItemTypeAttribute(ModelItemTypeEnum.CheckBox)]
        public bool MergeDelete
        {
            get
            {
                return this.mergeDelete;
            }

            set
            {
                this.mergeDelete = value;

                base.OnPropertyChanged("MergeDelete");
            }
        }

        public DataItemModel[] SourceItems 
        { 
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();

                result.Add(new DataItemModel { DisplayValue = Connections.Instance.DefaultConnectionName, ItemKey = Connections.Instance.DefaultConnectionName });

                foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
                {
                    result.Add(new DataItemModel { DisplayValue = connectionKey.Value.ConnectionName, ItemKey = connectionKey.Value.ConnectionName });
                }

                return result.ToArray();
            }
        }

        public DataItemModel[] DestinationItems
        {
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();

                result.Add(new DataItemModel { DisplayValue = "<File Output Only>", ItemKey = "FileOutputOnly" });

                if (Connections.Instance.DefaultConnectionName != this.Source)
                {
                    result.Add(new DataItemModel { DisplayValue = Connections.Instance.DefaultConnectionName, ItemKey = Connections.Instance.DefaultConnectionName });
                }

                foreach (KeyValuePair<string, AltDatabaseModel> connectionKey in Connections.Instance.AlternativeModels)
                {
                    if (connectionKey.Value.ConnectionName == this.Source)
                    {
                        continue;
                    }

                    result.Add(new DataItemModel { DisplayValue = connectionKey.Value.ConnectionName, ItemKey = connectionKey.Value.ConnectionName });
                }

                return result.ToArray();
            }
        }
    }
}
