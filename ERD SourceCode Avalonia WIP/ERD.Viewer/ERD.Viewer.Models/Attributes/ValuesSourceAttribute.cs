namespace ERD.Viewer.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValuesSourceAttribute : Attribute
    {
        private string propertyName;

        public ValuesSourceAttribute(string property)
        {
            propertyName = property;
        }

        public string PropertyName
        {
            get
            {
                return propertyName;
            }

            set
            {
                propertyName = value;
            }
        }
    }
}
