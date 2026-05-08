namespace ERD.Models
{
    public class ForeignKeyObjectModel : ModelBase
    {
        private int originalPosition;
        private string localColumnName;
        private string foreignKeyTable;
        private string foreignKeyColumn;
        private string foreignConstraintName;
        
        public string LocalColumnName 
        {
            get
            {
                return this.localColumnName;
            }
            
            set
            {
                base.OnPropertyChanged("LocalColumnName", ref this.localColumnName, value);
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
                //if (string.IsNullOrWhiteSpace(value) == false
                //    && value.Length > 63)
                //{

                //}

                base.OnPropertyChanged("ForeignConstraintName", ref this.foreignConstraintName, value);
            }
        }

        public bool IsVertualRelation
        {
            get;
            set;
        }
    }
}
