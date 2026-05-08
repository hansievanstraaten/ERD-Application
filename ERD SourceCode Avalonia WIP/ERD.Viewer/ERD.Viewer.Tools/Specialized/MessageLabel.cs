using ERD.Viewer.Shared.Functions;
using ERD.Viewer.Tools.SharedControls;

namespace ERD.Viewer.Tools.Specialized
{
    public class MessageLabel : LableItem
    {
        private int executionCount = 0;

        public MessageLabel()
        {
            DisplaySeconds = 5;

            MaximumQueueSize = 10;
        }

        public int DisplaySeconds { get; set; }

        public int MaximumQueueSize { get; set; }

        new public object Content
        {
            get
            {
                return base.Content;
            }

            set
            {
                ++executionCount;

                WaitTime();

                base.Content = value;

                InvalidateVisual();
            }
        }

        private async void WaitTime()
        {
            if (executionCount > MaximumQueueSize)
            {
                --executionCount;

                return;
            }

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Sleep.ThreadWaitSeconds(DisplaySeconds);
                });

                if (executionCount <= 1)
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
                executionCount--;
            }
        }
    }
}
