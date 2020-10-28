using ERD.Build.Properties;
using System.Collections.Generic;
using GeneralExtensions;
using System;
using WPF.Tools.ToolModels;
using System.Text;

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
                string[] itemSplit = item.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (itemSplit.Length == 2)
				{
                    result.Add(new DataItemModel { DisplayValue = itemSplit[1], ItemKey = itemSplit[0] });

                    continue;
				}

                StringBuilder displayText = new StringBuilder();

                for(int x = 1; x < itemSplit.Length; ++x)
				{
                    displayText.Append($"{itemSplit[x]} == ");
				}

                displayText.Remove(displayText.Length - 4, 4);

                result.Add(new DataItemModel { DisplayValue = displayText.ToString(), ItemKey = itemSplit[0] });

            }

            return result;
        }
    }
}
