using WPF.Tools.Attributes;

namespace ERD.Models
{
	[ModelName("Canvas")]
	public class ErdCanvasEditModel
	{
        [FieldInformation("Tab Name", IsRequired = true, IsReadOnly = true)]
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

    }
}
