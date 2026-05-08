namespace ERD.Viewer.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelNameAttribute : Attribute
    {
        private bool allowCollapse;

        private string name;

        public ModelNameAttribute(string modelName, bool allowHeaderCollapse = false)
        {
            name = modelName;

            allowCollapse = allowHeaderCollapse;
        }

        public bool AllowHeaderCollapse
        {
            get
            {
                return allowCollapse;
            }

            set
            {
                allowCollapse = value;
            }
        }

        public string ModelName
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
    }
}
