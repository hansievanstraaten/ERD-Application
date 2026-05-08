using Avalonia;
using ERD.Viewer.Models.Attributes;
using ERD.Viewer.Models.ColumnModels;
using ERD.Viewer.Shared.Extensions;

namespace ERD.Viewer.Models.TableModels
{
    [ModelName("Table")]
    public class TableModel : ModelBase
    {
        private ColumnObjectModel[] columns;

        private ColumnObjectModel[] deletedColumns;

        private bool isDeleted;

        private string tableName;

        private string schemaName = "dbo";

        private string friendlyName;

        private string erdSegmentModelName;

        private string description;

        private Point canvasLocation;

        private string pluralName;

        public TableModel()
        {
        }

        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }

            set
            {
                isDeleted = value;

                base.OnPropertyChanged("IsDeleted");
            }
        }

        public bool IsNewTable
        {
            get;
            set;
        }

        [FieldInformation("Name", IsRequired = true, DisableSpellChecker = true, Sort = 1)]
        public string TableName
        {
            get
            {
                return tableName;
            }

            set
            {
                base.OnPropertyChanged("TableName", ref tableName, value);
            }
        }

        [FieldInformation("Schema", DisableSpellChecker = true, Sort = 21)]
        public string SchemaName
        {
            get
            {
                return schemaName;
            }

            set
            {
                base.OnPropertyChanged("SchemaName", ref schemaName, value);
            }
        }

        [FieldInformation("Friendly Name", Sort = 3)]
        public string FriendlyName
        {
            get
            {
                if (friendlyName.IsNullEmptyOrWhiteSpace())
                {
                    return TableName;
                }

                return friendlyName;
            }

            set
            {
                base.OnPropertyChanged("FriendlyName", ref friendlyName, value);

            }
        }

        [FieldInformation("Plural Name", DisableSpellChecker = true, Sort = 4)]
        public string PluralName
        {
            get
            {
                return pluralName;
            }

            set
            {
                pluralName = value;

                base.OnPropertyChanged("PluralName", ref pluralName, value);
            }
        }
        
        [FieldInformation("Description", Sort = 5)]
        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                base.OnPropertyChanged("Description", ref description, value);
            }
        }

        public string ErdSegmentModelName
        {
            get
            {
                return erdSegmentModelName;
            }

            set
            {
                base.OnPropertyChanged("ErdSegmentModelName", ref erdSegmentModelName, value);
            }
        }

        public string PrimaryKeyClusterConstraintName
        {
            get;
            set;
        }

        public Point CanvasLocation
        {
            get
            {
                return canvasLocation;
            }

            set
            {
                base.OnPropertyChangedStringCompair("CanvasLocation", ref canvasLocation, value);
            }
        }

        public ColumnObjectModel[] Columns
        {
            get
            {
                if (!columns.HasElements())
                {
                    columns = new ColumnObjectModel[] { };
                }

                return columns.OrderBy(c => c.Column_Id).ToArray();
            }

            set
            {
                columns = value.OrderBy(c => c.Column_Id).ToArray();

                base.OnPropertyChanged("Columns");
            }
        }

        public ColumnObjectModel[] DeltedColumns
        {
            get
            {
                if (!deletedColumns.HasElements())
                {
                    deletedColumns = new ColumnObjectModel[] { };
                }

                return deletedColumns;
            }

            set
            {
                deletedColumns = value;

                base.OnPropertyChanged("DeltedColumns");
            }
        }
    }
}
