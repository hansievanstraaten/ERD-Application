using GeneralExtensions;
using System.Collections.Generic;

namespace Report.Web.Presenters.Exstentions
{
	public static class StringExstentions
	{
		public static Dictionary<string, string> SplitClientQueryString(this string clientQueryString)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();

			if (clientQueryString.IsNullEmptyOrWhiteSpace())
			{
				return result;
			}

			foreach(string queryItem in clientQueryString.Split('&'))
			{
				string[] itemSplit = queryItem.Split('=');

				result.Add(itemSplit[0], itemSplit[1]);
			}

			return result;
		}
	}
}
