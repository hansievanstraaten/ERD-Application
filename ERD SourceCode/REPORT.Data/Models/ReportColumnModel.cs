using ERD.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using WPF.Tools.Attributes;

namespace REPORT.Data.Models
{
    [ModelName("Data Column")]
    [Serializable()]
    public class ReportColumnModel : ModelBase
    {
        private bool inPrimaryKey;
        private bool isForeignkey;
        private string columnName;
        private string foreignKeyTable;
        private string foreignKeyColumn;
        private string foreignConstraintName;
        private string description;
        private string friendlyName;
        private SqlDbType? sqlDataType;
        private string dataType;
        private string tableName;

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

        public string TableName
        {
            get
            {
                return this.tableName;
            }

            set
            {
                this.tableName = value;
            }
        }

        [FieldInformation("Column Name", IsRequired = true, Sort = 0, DisableSpellChecker = true)]
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
        public string DataType
        {
            get
            {
                return this.dataType;
            }

            set
            {
                this.dataType = value;
            }
        }

        [FieldInformation("Friendly Name", Sort = 1)]
        public string FriendlyName
        {
            get
            {
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
    
        public double DropX { get; set; }

        public double DropY { get; set; }
    }
}
