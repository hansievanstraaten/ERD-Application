namespace ERD.Viewer.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldInformationAttribute : Attribute
    {
        private int sortOrder = 0;

        private int maxTextLength = 0;

        private bool isReadOnly = false;

        private bool visible = true;

        private bool isRequired = false;

        private bool disableSpellCheck = false;

        private string caption;

        public FieldInformationAttribute(
          string fieldCaption,
          bool defaultVisible = true,
          int sort = 0,
          int maxLength = 0,
          bool readOnly = false,
          bool isRequiredField = false,
          bool disableSpellChecker = false)
        {
            caption = fieldCaption;

            visible = defaultVisible;

            sortOrder = sort;

            maxTextLength = maxLength;

            isReadOnly = readOnly;

            isRequired = isRequiredField;

            disableSpellCheck = disableSpellChecker;
        }

        public int Sort
        {
            get
            {
                return sortOrder;
            }

            set
            {
                sortOrder = value;
            }
        }

        public int MaxTextLength
        {
            get
            {
                return maxTextLength;
            }

            set
            {
                maxTextLength = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }

            set
            {
                isReadOnly = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }

        public bool IsRequired
        {
            get
            {
                return isRequired;
            }

            set
            {
                isRequired = value;
            }
        }

        public bool DisableSpellChecker
        {
            get
            {
                return disableSpellCheck;
            }

            set
            {
                disableSpellCheck = value;
            }
        }

        public string FieldCaption
        {
            get
            {
                return caption;
            }

            set
            {
                caption = value;
            }
        }
    }
}
