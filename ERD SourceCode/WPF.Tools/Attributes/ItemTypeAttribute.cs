using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Tools.ModelViewer;

namespace WPF.Tools.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ItemTypeAttribute : Attribute
  {
    private ModelItemTypeEnum modelRenderType;

    private bool isComboboxEditable;

    public ItemTypeAttribute(ModelItemTypeEnum modelType, bool isComboboxEdit = true)
    {
      this.modelRenderType = modelType;

      this.isComboboxEditable = isComboboxEdit;
    }

    public ModelItemTypeEnum ModelType
    {
      get
      {
        return this.modelRenderType;
      }

      set
      {
        this.modelRenderType = value;
      }
    }

    public bool IsComboboxEditable
    {
      get
      {
        return this.isComboboxEditable;
      }

      set
      {
        this.isComboboxEditable = value;
      }
    }
  }
}
