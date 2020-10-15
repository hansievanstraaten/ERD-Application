using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
    public static class StringBuilderExstentions
    {
        public static int IndexOf(this StringBuilder builder, string value)
        {
            if (builder == null 
                || builder.Length == 0 
                || value.IsNullEmptyOrWhiteSpace())
            {
                return -1;
            }

            return builder.ToString().IndexOf(value);
        }

        public static int IndexOf(this StringBuilder builder, string value, int startIndex)
        {
            if (builder == null
                || builder.Length == 0
                || value.IsNullEmptyOrWhiteSpace())
            {
                return -1;
            }

            return builder.ToString().IndexOf(value, startIndex);
        }

        public static string Substring(this StringBuilder builder, int startIndex)
        {
            if (builder == null
                || builder.Length == 0
                || startIndex < 0)
            {
                return string.Empty;
            }

            return builder.ToString().Substring(startIndex);
        }

        public static string Substring(this StringBuilder builder, int startIndex, int length)
        {
            if (builder == null
                || builder.Length == 0
                || startIndex < 0
                || length <= 0)
            {
                return string.Empty;
            }

            return builder.ToString().Substring(startIndex, length);
        }
    }
}
