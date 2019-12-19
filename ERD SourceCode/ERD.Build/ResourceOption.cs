using ERD.Build.Properties;
using System.Collections.Generic;
using GeneralExtensions;
using System;
using WPF.Tools.ToolModels;

namespace ERD.Build
{
  public class ResourceOption
  {
    public List<DataItemModel> ScriptParameterOptions()
    {
      string[] resultItems = Resources.BuildParameters
                                      .Replace("\n", string.Empty)
                                      .Replace("\r", string.Empty)
                                      .Split(',', StringSplitOptions.RemoveEmptyEntries);

      List<DataItemModel> result = new List<DataItemModel>();

      foreach (string item in resultItems)
      {
        string[] itemSplit = item.Split('=');

        result.Add(new DataItemModel {DisplayValue = itemSplit[1], ItemKey = itemSplit[0]});
      }

      return result;
    }
  }
}
