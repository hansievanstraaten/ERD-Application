namespace WPF.Tools.ToolModels
{
  public class DataItemModel
  {
    public DataItemModel()
    {
      
    }

    public DataItemModel(string itemKey, string displayValue)
    {
      this.ItemKey = itemKey;

      this.DisplayValue = displayValue;
    }

    public string DisplayValue { get; set; }

    public object ItemKey { get; set; }

    public object Tag { get; set; }
  }
}
