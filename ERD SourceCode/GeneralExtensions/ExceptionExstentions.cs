using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralExtensions
{
  public static class ExceptionExstentions
  {
    public static string InnerExceptionMessage(this Exception err)
    {
      if (err.InnerException == null)
      {
        return err.Message;
      }

      return err.InnerException.InnerExceptionMessage();
    }

    public static string ExstendedMessage(this Exception err)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(err.Message);

      if (err.InnerException != null)
      {
        result.AppendLine(err.InnerException.ExstendedMessage());
      }

      return result.ToString();
    }

    public static string ExstendedStackTrace(this Exception err)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(err.StackTrace);

      if (err.InnerException != null)
      {
        result.AppendLine(err.InnerException.ExstendedStackTrace());
      }

      return result.ToString();
    }

    public static string ExstendedSource(this Exception err)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(err.Source);

      if (err.InnerException != null)
      {
        result.AppendLine(err.InnerException.ExstendedSource());
      }

      return result.ToString();
    }

    public static string GetFullExceptionMessage(this Exception err)
    {
      StringBuilder result = new StringBuilder();

      result.AppendLine(err.ExstendedMessage());

      result.AppendLine();

      result.AppendLine("Stack Trace:");

      result.AppendLine(err.ExstendedStackTrace());

      return result.ToString();
    }
  }
}
