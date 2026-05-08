namespace ERD.Viewer.Models.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ItemTypeAttribute : Attribute
  {
    private ModelItemTypeEnum modelRenderType;

    private bool isComboboxEditable;

    public ItemTypeAttribute(ModelItemTypeEnum modelType, bool isComboboxEdit = true)
    {
            modelRenderType = modelType;

            isComboboxEditable = isComboboxEdit;
    }

    public ModelItemTypeEnum ModelType
    {
      get
      {
        return modelRenderType;
      }

      set
      {
                modelRenderType = value;
      }
    }

    public bool IsComboboxEditable
    {
      get
      {
        return isComboboxEditable;
      }

      set
      {
                isComboboxEditable = value;
      }
    }
  }
}
