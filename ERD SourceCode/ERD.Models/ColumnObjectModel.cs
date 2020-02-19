using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WPF.Tools.Attributes;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace ERD.Models
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
        private string columnName;
        private string foreignKeyTable;
        private string foreignKeyColumn;
        private string foreignConstraintName;
        private string description;
        private string friendlyName;
        private SqlDbType? sqlDataType;

        [FieldInformation("In Primary Key", Sort = 3)]
        public bool InPrimaryKey
        {
            get
            {
                return this.inPrimaryKey;
            }

            set
            {
                base.OnPropertyChanged("InPrimaryKey", ref this.inPrimaryKey, value);

                base.OnPropertyChanged("KeyColumn");

                if (value)
                {
                    this.AllowNulls = false;

                    base.OnPropertyChanged("AllowNulls");
                }
            }
        }

        public bool IsForeignkey
        {
            get
            {
                return this.isForeignkey;
            }

            set
            {
                base.OnPropertyChanged("IsForeignkey", ref this.isForeignkey, value);

                base.OnPropertyChanged("ForeignKeyColumnValue");
            }
        }

        [FieldInformation("Allow Nulls", Sort = 4)]
        public bool AllowNulls
        {
            get
            {
                return this.allowNulls;
            }

            set
            {
                base.OnPropertyChanged("AllowNulls", ref this.allowNulls, value);
            }
        }

        public bool IsIdentity
        {
            get
            {
                return this.isIdentity;
            }

            set
            {
                base.OnPropertyChanged("IsIdentity", ref this.isIdentity, value);
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
                return this.isDeleted;
            }

            set
            {
                this.isDeleted = value;

                base.OnPropertyChanged("IsDeleted");
            }
        }

        public int MaxLength
        {
            get
            {
                return this.maxLength;
            }

            set
            {
                base.OnPropertyChanged("MaxLength", ref this.maxLength, value);
            }
        }

        public int Precision
        {
            get
            {
                return this.precision;
            }

            set
            {
                base.OnPropertyChanged("Precision", ref this.precision, value);
            }
        }

        public int Scale
        {
            get
            {
                return this.scale;
            }

            set
            {
                base.OnPropertyChanged("Scale", ref this.scale, value);
            }
        }

        public int Column_Id
        {
            get
            {
                return this.column_Id;
            }

            set
            {
                base.OnPropertyChanged("Column_Id", ref this.column_Id, value);
            }
        }

        public int OriginalPosition
        {
            get
            {
                return this.originalPosition;
            }

            set
            {
                this.originalPosition = value;
            }
        }

        [FieldInformation("Column Name", IsRequired = true, Sort = 0, DisableSpellChecker = true)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = true)]
        [ValuesSourceAttribute("SystemColumnNames")]
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }

            set
            {
                base.OnPropertyChanged("ColumnName", ref this.columnName, value);
            }
        }

        public string ForeignKeyColumnValue
        {
            get
            {
                if (this.IsForeignkey)
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
                if (this.ColumnName.IsNullEmptyOrWhiteSpace())
                {
                    return "\\...";
                }

                if (this.InPrimaryKey)
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
                return this.description;
            }

            set
            {
                base.OnPropertyChanged("Description", ref this.description, value);
            }
        }

        [FieldInformation("Data Type", IsRequired = true, Sort = 2)]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = true)]
        [ValuesSourceAttribute("DataTypesValues")]
        public string DataType
        {
            get
            {
                if (this.IsIdentity)
                {
                    return $"{this.SqlDataType?.SQLFormat(this.MaxLength, this.Precision, this.Scale)} IDENTITY";
                }

                return this.SqlDataType?.SQLFormat(this.MaxLength, this.Precision, this.Scale);
            }

            set
            {
                string[] valueSplit = value.Split(new char[] {'(', ',', ')'}, StringSplitOptions.RemoveEmptyEntries);

                SqlDbType result = SqlDbType.VarChar;

                Enum.TryParse<SqlDbType>(valueSplit[0].Replace(" IDENTITY", string.Empty), true, out result);

                this.SqlDataType = result;

                //if (valueSplit.Length == 2 && valueSplit[1].ToUpper() != "MAX")
                if (valueSplit.Length == 2 && valueSplit[1].IsNumberic())
                {
                    this.MaxLength = valueSplit[1].ToInt32();
                }
                else if (valueSplit.Length == 2 && valueSplit[1].ToUpper() == "MAX")
                {
                    this.MaxLength = -1;
                }
                else if (valueSplit.Length == 3)
                {
                    this.Precision = valueSplit[1].ToInt32();

                    this.Scale = valueSplit[2].ToInt32();
                }

                this.IsIdentity = value.EndsWith("IDENTITY");

                base.OnPropertyChanged("DataType");
            }
        }

        [FieldInformation("Friendly Name", Sort = 1)]
        public string FriendlyName
        {
            get
            {
                //if (this.friendlyName.IsNullEmptyOrWhiteSpace())
                //{
                //  return this.ColumnName;
                //}

                return this.friendlyName;
            }

            set
            {
                base.OnPropertyChanged("FriendlyName", ref this.friendlyName, value);
            }
        }

        public string ForeignKeyTable
        {
            get
            {
                return this.foreignKeyTable;
            }

            set
            {
                base.OnPropertyChanged("ForeignKeyTable", ref this.foreignKeyTable, value);
            }
        }

        public string ForeignKeyColumn
        {
            get
            {
                return this.foreignKeyColumn;
            }

            set
            {
                base.OnPropertyChanged("ForeignKeyColumn", ref this.foreignKeyColumn, value);
            }
        }

        public string ForeignConstraintName
        {
            get
            {
                return this.foreignConstraintName;
            }

            set
            {
                base.OnPropertyChanged("ForeignConstraintName", ref this.foreignConstraintName, value);
            }
        }

        public SqlDbType? SqlDataType
        {
            get
            {
                return this.sqlDataType;
            }

            set
            {
                base.OnPropertyChangedStringCompair("SqlDataType", ref this.sqlDataType, value);

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

        public DataItemModel[] SystemColumnNames
        {
            get
            {
                return this.InvokeMethod("ERD.Common.IntegrityInvoker,ERD.Common", "GetSystemColumns", null).To<List<DataItemModel>>().ToArray();
            }
        }
    }
}
