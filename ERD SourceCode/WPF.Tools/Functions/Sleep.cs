using System.Threading;

namespace WPF.Tools.Functions
{
  public static class Sleep
  {
    private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

    public static void ThreadWait(int miliseconds)
    {
      Sleep.manualResetEvent.WaitOne(miliseconds);
    }

    public static void ThreadWaitSeconds(int seconds)
    {
      Sleep.ThreadWait((seconds * 1000));
    }
  }
}
