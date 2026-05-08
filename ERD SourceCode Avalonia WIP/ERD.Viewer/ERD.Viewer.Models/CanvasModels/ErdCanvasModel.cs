using ERD.Viewer.Models.Attributes;
using ERD.Viewer.Models.ColumnModels;
using ERD.Viewer.Models.TableModels;
using ERD.Viewer.Shared.Extensions;

namespace ERD.Viewer.Models.CanvasModels
{
    [ModelName("Canvas")]
    public class ErdCanvasModel
    {
        public delegate void ErdCanvasModelLockChangedEvent(object sender, bool lockStatus);

        public event ErdCanvasModelLockChangedEvent ErdCanvasModelLockChanged;

        private bool isLocked;

        public ErdCanvasModel()
        {
            IncludeInContextBuild = new List<IncludeTableModel>();
        }

        public bool IsLocked 
        { 
            get
            {
                if (SegmentTables.Any(t => t.HasModelChanged)
                    || SegmentTables.Any(t => t.Columns.Any(c => c.HasModelChanged)))
                {
                    return true;
                }

                return isLocked;
            }
            
            private set
            {
                if (!value && SegmentTables != null)
                {
                    foreach(TableModel table in SegmentTables)
                    {
                        table.HasModelChanged = false;

                        foreach(ColumnObjectModel column in table.Columns)
                        {
                            column.HasModelChanged = false;
                        }
                    }
                }

                isLocked = value;
            }
        }

        [FieldInformation("Tab Name", IsRequired = true)]
        public string ModelSegmentName
        {
            get;
            set;
        }

        [FieldInformation("Table Prefix", DisableSpellChecker = true)]
        public string TablePrefix
        {
            get;
            set;
        }

        public string ModelSegmentControlName
        {
            get
            {
                if (ModelSegmentName.IsNullEmptyOrWhiteSpace())
                {
                    return string.Empty;
                }

                return ModelSegmentName.MakeAlphaNumeric();
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
    
        public void SetLock(bool isLockedValue, bool raiseEvent)
        {
            IsLocked = isLockedValue;

            if (raiseEvent)
            {
                ErdCanvasModelLockChanged?.Invoke(this, isLockedValue);
            }
        }
    }
}
