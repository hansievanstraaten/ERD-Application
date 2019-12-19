using System.Threading.Tasks;
using WPF.Tools.CommonControls;
using WPF.Tools.Functions;

namespace WPF.Tools.Specialized
{
  public class MessageLabel : LableItem
  {
    private int executionCount = 0;

    public MessageLabel()
    {
      this.DisplaySeconds = 5;

      this.MaximumQueueSize = 10;
    }

    public int DisplaySeconds { get; set; }

    public int MaximumQueueSize { get; set;}
    
    new public object Content
    {
      get
      {
        return base.Content;
      }

      set
      {
        ++this.executionCount;

        this.WaitTime();
        
        base.Content = value;

        this.InvalidateVisual();
      }
    }

    private async void WaitTime()
    {
      if (this.executionCount > this.MaximumQueueSize)
      {
        --this.executionCount;

        return;
      }

      try
      {
        await Task.Factory.StartNew(() =>
        {
          Sleep.ThreadWaitSeconds(this.DisplaySeconds);
        });
        
        if (this.executionCount <= 1)
        {
          base.Content = null;
        }
      }
      catch
      {
        base.Content = null;
      }
      finally
      {
        this.executionCount--;
      }
    }
  }
}
