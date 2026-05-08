using ERD.Viewer.Models.Attributes;
using ERD.Viewer.Shared.Extensions;
using System.Data;
using ERD.Viewer.DataScriptFactory.Extensions;

namespace ERD.Viewer.Models.ColumnModels
{
    [ModelName("Data Column")]
    public class ColumnObjectModel : ModelBase
    {
        private bool inPrimaryKey;
        private bool allowNulls = true;
        private bool isForeignkey;
        private bool isIdentity;
        private bool isDeleted;
        private int maxLength;
        private int precision;
        private int scale;
        private int column_Id;
        private int originalPosition;
        private string columnName = string.Empty;
        private string foreignKeyTable = string.Empty;
        private string foreignKeyColumn = string.Empty;
        private string foreignConstraintName = string.Empty;
        private string description = string.Empty;
        private string friendlyName = string.Empty;
        private SqlDbType? sqlDataType;

        [FieldInformation("In Primary Key", Sort = 3)]
        public bool InPrimaryKey
        {
            get
            {
                return inPrimaryKey;
            }

            set
            {
                base.OnPropertyChanged("InPrimaryKey", ref inPrimaryKey, value);

                base.OnPropertyChanged("KeyColumn");

                if (value)
                {
                    AllowNulls = false;

                    base.OnPropertyChanged("AllowNulls");
                }
            }
        }

        public bool IsForeignkey
        {
            get
            {
                return isForeignkey;
            }

            set
            {
                base.OnPropertyChanged("IsForeignkey", ref isForeignkey, value);

                base.OnPropertyChanged("ForeignKeyColumnValue");
            }
        }

        [FieldInformation("Allow Nulls", Sort = 4)]
        public bool AllowNulls
        {
            get
            {
                return allowNulls;
            }

            set
            {
                base.OnPropertyChanged("AllowNulls", ref allowNulls, value);
            }
        }

        public bool IsIdentity
        {
            get
            {
                return isIdentity;
            }

            set
            {
                base.OnPropertyChanged("IsIdentity", ref isIdentity, value);
            }
        }

        public bool IsVertualRelation
        {
            get;
            set;
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

        public int MaxLength
        {
            get
            {
                return maxLength;
            }

            set
            {
                base.OnPropertyChanged("MaxLength", ref maxLength, value);
            }
        }

        public int Precision
        {
            get
            {
                return precision;
            }

            set
            {
                base.OnPropertyChanged("Precision", ref precision, value);
            }
        }

        public int Scale
        {
            get
            {
                return scale;
            }

            set
            {
                base.OnPropertyChanged("Scale", ref scale, value);
            }
        }

        public int Column_Id
        {
            get
            {
                return column_Id;
            }

            set
            {
                base.OnPropertyChanged("Column_Id", ref column_Id, value);
            }
        }

        public int OriginalPosition
        {
            get
            {
                return originalPosition;
            }

            set
            {
                originalPosition = value;
            }
        }

        [FieldInformation("Column Name", IsRequired = true, Sort = 0, DisableSpellChecker = true)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = true)]
        [ValuesSourceAttribute("SystemColumnNames")]
        public string ColumnName
        {
            get
            {
                return columnName;
            }

            set
            {
                base.OnPropertyChanged("ColumnName", ref columnName, value);
            }
        }

        public string ForeignKeyColumnValue
        {
            get
            {
                if (IsForeignkey)
                {
                    return "FK";
                }

                return string.Empty;
            }

            set
            {

            }
        }

        public string KeyColumn
        {
            get
            {
                if (ColumnName.IsNullEmptyOrWhiteSpace())
                {
                    return "\\...";
                }

                if (InPrimaryKey)
                {
                    return "PK";
                }

                return string.Empty;
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

        [FieldInformation("Data Type", IsRequired = true, Sort = 2)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = true)]
        [ValuesSourceAttribute("DataTypesValues")]
        public string? DataType
        {
            get
            {
                if (IsIdentity)
                {
                    return $"{SqlDataType?.SQLFormat(MaxLength, Precision, Scale)} IDENTITY";
                }

                return SqlDataType?.SQLFormat(MaxLength, Precision, Scale);
            }

            set
            {
                string[] valueSplit = value.Split(new char[] {'(', ',', ')'}, StringSplitOptions.RemoveEmptyEntries);

                SqlDbType result = SqlDbType.VarChar;

                Enum.TryParse(valueSplit[0].Replace(" IDENTITY", string.Empty), true, out result);

                SqlDataType = result;

                //if (valueSplit.Length == 2 && valueSplit[1].ToUpper() != "MAX")
                if (valueSplit.Length == 2 && valueSplit[1].IsNumeric())
                {
                    MaxLength = valueSplit[1].ToInt32();
                }
                else if (valueSplit.Length == 2 && valueSplit[1].ToUpper() == "MAX")
                {
                    MaxLength = -1;
                }
                else if (valueSplit.Length == 3)
                {
                    Precision = valueSplit[1].ToInt32();

                    Scale = valueSplit[2].ToInt32();
                }

                IsIdentity = value.EndsWith("IDENTITY");

                base.OnPropertyChanged("DataType");
            }
        }

        [FieldInformation("Friendly Name", Sort = 1)]
        public string FriendlyName
        {
            get
            {
                return friendlyName;
            }

            set
            {
                base.OnPropertyChanged("FriendlyName", ref friendlyName, value);
            }
        }

        public string ForeignKeyTable
        {
            get
            {
                return foreignKeyTable;
            }

            set
            {
                base.OnPropertyChanged("ForeignKeyTable", ref foreignKeyTable, value);
            }
        }

        public string ForeignKeyColumn
        {
            get
            {
                return foreignKeyColumn;
            }

            set
            {
                base.OnPropertyChanged("ForeignKeyColumn", ref foreignKeyColumn, value);
            }
        }

        public string ForeignConstraintName
        {
            get
            {
                return foreignConstraintName;
            }

            set
            {
                base.OnPropertyChanged("ForeignConstraintName", ref foreignConstraintName, value);
            }
        }

        public SqlDbType? SqlDataType
        {
            get
            {
                return sqlDataType;
            }

            set
            {
                base.OnPropertyChangedStringCompair("SqlDataType", ref sqlDataType, value);

                base.OnPropertyChanged("DataType");
            }
        }

        public DataItemModel[] DataTypesValues
        {
            get
            {
                List<DataItemModel> result = new List<DataItemModel>();

                foreach (SqlDbType dataType in Enum.GetValues(typeof(SqlDbType)))
                {
                    switch (dataType)
                    {
                        case SqlDbType.Int:
                        case SqlDbType.BigInt:

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()}", ItemKey = dataType});

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()} IDENTITY", ItemKey = dataType});

                            break;

                        case SqlDbType.VarChar:
                        case SqlDbType.VarBinary:
                        case SqlDbType.NVarChar:

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()}(255)", ItemKey = dataType});

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()}(MAX)", ItemKey = dataType});

                            break;

                        case SqlDbType.Decimal:
                        case SqlDbType.Money:

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()}(18, 6)", ItemKey = dataType});

                            break;

                        case SqlDbType.Binary:
                        case SqlDbType.Char:

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()}(50)", ItemKey = dataType});

                            break;

                        case SqlDbType.DateTime2:
                        case SqlDbType.DateTimeOffset:
                        case SqlDbType.Time:

                            result.Add(new DataItemModel {DisplayValue = $"{dataType.ToString()}(7)", ItemKey = dataType});

                            break;

                        default:
                            result.Add(new DataItemModel {DisplayValue = dataType.ToString(), ItemKey = dataType});

                            break;
                    }
                }

                return result.OrderBy(e => e.DisplayValue).ToArray();
            }
        }

        public DataItemModel[]? SystemColumnNames
        {
            get
            {
                return this.InvokeMethod("ERD.Common.IntegrityInvoker,ERD.Common", "GetSystemColumns", null)?.To<List<DataItemModel>>()?.ToArray();
            }
        }
    }
}
