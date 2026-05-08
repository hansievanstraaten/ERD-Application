using System.Text;

namespace ERD.Viewer.Shared.Extensions
{
    public static class StringBuilderExtensions
    {
        public static bool IsNullEmptyOrWhiteSpace(this StringBuilder value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return true;
            }

            return false;
        }

    }
}
