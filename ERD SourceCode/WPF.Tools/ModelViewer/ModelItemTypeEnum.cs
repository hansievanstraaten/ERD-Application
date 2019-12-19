using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Tools.ModelViewer
{
  public enum ModelItemTypeEnum
  {
    TextBox = 1,
    CheckBox = 2,
    ComboBox = 3,
    DatePicker = 4,
    EnumBox = 5,
    SecureString = 6
  }

  public static class ModelItemType
  {
    public static ModelItemTypeEnum GetItemType(Type itemType)
    {
      string typeName = itemType.Name;

      if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        typeName = itemType.GenericTypeArguments[0].Name;
      }

      Type baseType = itemType.BaseType;

      switch (typeName)
      {
        case nameof(Boolean):
          return ModelItemTypeEnum.CheckBox;

          case nameof(Enum):
            return ModelItemTypeEnum.ComboBox;

        case nameof(DateTime):
          return ModelItemTypeEnum.DatePicker;

        case nameof(System.Security.SecureString):
          return ModelItemTypeEnum.SecureString;
          
          default:
            if (baseType.Name == nameof(Enum))
            {
              return ModelItemTypeEnum.EnumBox;
            }

            return ModelItemTypeEnum.TextBox;
      }
    }
  }
}
