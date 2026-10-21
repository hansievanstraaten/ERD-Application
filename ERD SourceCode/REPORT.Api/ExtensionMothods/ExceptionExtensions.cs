using System.Text;

namespace REPORT.Api.ExtensionMothods
{
    public static class ExceptionExtensions
    {
        public static string MessageFull(this Exception error)
        {
            if (error.InnerException == null)
            {
                return error.Message;
            }

            StringBuilder result = new StringBuilder();

            result.AppendLine(error.Message);
            result.AppendLine(error.InnerException.MessageFull());

            return result.ToString();
        }

        public static string StackTraceFull(this Exception error)
        {
            if (error.InnerException == null)
            {
                return error.StackTrace!;
            }

            StringBuilder result = new StringBuilder();

            result.AppendLine(error.StackTrace);
            result.AppendLine(error.InnerException.StackTraceFull());

            return result.ToString();
        }

        public static string FullErrorDescription(this Exception err)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(MessageFull(err));
            result.AppendLine(StackTraceFull(err));

            return result.ToString();
        }
    }
}
