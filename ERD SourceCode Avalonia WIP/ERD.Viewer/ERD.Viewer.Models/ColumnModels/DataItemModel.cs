namespace ERD.Viewer.Models.ColumnModels
{
    public class DataItemModel
    {
        public DataItemModel()
        {
            this.DisplayValue = string.Empty;

            this.ItemKey = null;

            this.Tag = null;
        }

        public DataItemModel(string itemKey, string displayValue)
        {
            this.DisplayValue = displayValue;

            this.ItemKey = itemKey;

            this.Tag = null;
        }

        public string DisplayValue { get; set; }

        public object? ItemKey { get; set; }

        public object? Tag { get; set; }
    }
}
