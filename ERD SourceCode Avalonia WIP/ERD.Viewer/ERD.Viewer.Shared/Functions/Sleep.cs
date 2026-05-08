using System.Threading;

namespace ERD.Viewer.Shared.Functions
{
  public static class Sleep
  {
    private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

    public static void ThreadWait(int miliseconds)
    {
            manualResetEvent.WaitOne(miliseconds);
    }

    public static void ThreadWaitSeconds(int seconds)
    {
            ThreadWait(seconds * 1000);
    }
  }
}
