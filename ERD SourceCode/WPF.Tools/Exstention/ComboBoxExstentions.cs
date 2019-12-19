using GeneralExtensions;
using System;
using WPF.Tools.CommonControls;
using WPF.Tools.Dictionaries;
using WPF.Tools.ToolModels;

namespace WPF.Tools.Exstention
{
  public static class ComboBoxExstentions
  {
    public static void AddEnum(this ComboBoxTool destination, Type enumType)
    {
      foreach (var item in Enum.GetValues(enumType))
      {
        DataItemModel result = new DataItemModel
        {
          DisplayValue = TranslationDictionary.Translate(item.GetDescriptionAttribute()),
          ItemKey = item
        };

        destination.Items.Add(result);
      }
    }
    
  }
}
