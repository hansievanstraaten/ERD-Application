using GeneralExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPF.Tools.Attributes;

namespace ERD.Models
{
    [ModelName("Canvas")]
    public class ErdCanvasModel
    {
        private bool isLocked;

        public ErdCanvasModel()
        {
            this.IncludeInContextBuild = new List<IncludeTableModel>();
        }

        public bool IsLocked 
        { 
            get
            {
                if (this.SegmentTables.Any(t => t.HasModelChanged)
                    || this.SegmentTables.Any(t => t.Columns.Any(c => c.HasModelChanged)))
                {
                    return true;
                }

                return this.isLocked;
            }
            
            set
            {
                if (!value && this.SegmentTables != null)
                {
                    foreach(TableModel table in this.SegmentTables)
                    {
                        table.HasModelChanged = false;

                        foreach(ColumnObjectModel column in table.Columns)
                        {
                            column.HasModelChanged = false;
                        }
                    }
                }

                this.isLocked = value;
            }
        }

        [FieldInformation("Tab Name", IsRequired = true)]
        public string ModelSegmentName
        {
            get;
            set;
        }

        [FieldInformation("Table Prefix")]
        public string TablePrefix
        {
            get;
            set;
        }

        public string ModelSegmentControlName
        {
            get
            {
                if (this.ModelSegmentName.IsNullEmptyOrWhiteSpace())
                {
                    return string.Empty;
                }

                return this.ModelSegmentName.MakeAlphaNumeric();
            }
        }

        public List<TableModel> SegmentTables
        {
            get;
            set;
        }

        public List<IncludeTableModel> IncludeInContextBuild
        {
            get;
            set;
        }
    }
}
