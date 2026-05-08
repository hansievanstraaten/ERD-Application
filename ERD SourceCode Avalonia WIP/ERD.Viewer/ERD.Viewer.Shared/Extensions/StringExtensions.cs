using System.Text.RegularExpressions;

namespace ERD.Viewer.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string value)
        {
            decimal resultValue = 0;

            return decimal.TryParse(value, out resultValue);
        }

        public static int ToInt32(this string value)
        {
            if (value.IsNullEmptyOrWhiteSpace())
            {
                return 0;
            }

            return Convert.ToInt32(value);
        }

        public static string MakeAlphaNumeric(this string value)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9-_]");

            return rgx.Replace(value, string.Empty);
        }
    }
}
