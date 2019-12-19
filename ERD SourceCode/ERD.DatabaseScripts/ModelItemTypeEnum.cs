using System;

namespace ERD.DatabaseScripts
{
  internal enum ModelItemTypeEnum
  {
    TextBox = 1,
    CheckBox = 2,
    ComboBox = 3,
    DatePicker = 4,
    EnumBox = 5,
    SecureString = 6
  }

  internal static class ModelItemType
  {
    public static ModelItemTypeEnum GetItemType(Type itemType)
    {
      Type baseType = itemType.BaseType;

      switch (itemType.Name)
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
