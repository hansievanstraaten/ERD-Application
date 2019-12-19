using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;

namespace WPF.Tools.ToolModels
{
  [ModelNameAttribute("Select")]
  public class SelectViewModel : ModelsBase
  {
    private object selectedItem;

    [FieldInformationAttribute("Options")]
    [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
    [ValuesSourceAttribute("LookupValues")]
    public object SelectedItem
    {
      get
      {
        return this.selectedItem;
      }

      set
      {
        this.selectedItem = value;

        base.OnPropertyChanged(() => this.SelectedItem);
      }
    }

    public DataItemModel[] LookupValues
    {
      get;

      set;
    }
  }
}
