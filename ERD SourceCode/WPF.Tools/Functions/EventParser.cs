using System;
using System.Windows.Threading;

namespace WPF.Tools.Functions
{
  public class ParseQueryEventArguments
  {
    public string Title { get; set;}

    public object[] Arguments { get; set;}
  }

  public class ParseMessageEventArguments
  {
    public string Title { get; set;}

    public string Message { get; set;}

    public object[] Arguments { get; set; }
  }

  public static class EventParser
  {
    public delegate object ParseQueryObjectEvent(object sender, ParseQueryEventArguments e);

    public delegate void ParseMessageObjectEvent(object sender, ParseMessageEventArguments e);

    public delegate void ParseErrorObjectEvent(object sender, Exception err);

    public static event ParseQueryObjectEvent ParseQueryObject;

    public static event ParseMessageObjectEvent ParseMessageObject;

    public static event ParseErrorObjectEvent ParseErrorObject;

    public static object ParseQuery(object sender, string title)
    {
      return EventParser.ParseQuery(sender, new ParseQueryEventArguments { Title = title});
    }

    public static object ParseQuery(object sender, ParseQueryEventArguments e)
    {
      return ParseQueryObject?.Invoke(sender, e ?? new ParseQueryEventArguments { Arguments = new object[] { } });
    }

    public static void ParseMessage(object sender, Dispatcher dispatcher, string title, string message)
    {
      dispatcher.Invoke(() =>
      {
        EventParser.ParseMessage(sender, new ParseMessageEventArguments { Title = title, Message = message });
      });
    }

    public static void ParseMessage(object sender, string title, string message)
    {
      EventParser.ParseMessage(sender, new ParseMessageEventArguments { Title = title, Message = message });
    }

    public static void ParseMessage(object sender, ParseMessageEventArguments e)
    {
      ParseMessageObject?.Invoke(sender, e ?? new ParseMessageEventArguments { Arguments = new object[] { } });
    }

    public static void ParseError(object sender, Exception err)
    {
      ParseErrorObject?.Invoke(sender, err);
    }
  }
}
