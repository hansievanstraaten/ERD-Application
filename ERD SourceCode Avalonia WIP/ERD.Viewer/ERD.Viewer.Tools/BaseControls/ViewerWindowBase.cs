using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;

namespace ERD.Viewer.Tools.BaseControls
{
    public class ViewerWindowBase : Window
    {
        private readonly WindowNotificationManager notifications;
        public ViewerWindowBase()
        {
            this.notifications = new WindowNotificationManager(this)
            {
                Position = NotificationPosition.TopCenter,
                MaxItems = 3
            };
        }

        protected WindowNotificationManager Notifications => this.notifications;

        public void ShowError(Exception err)
        {
            this.notifications.Show(new Notification("Error", err?.Message ?? "Unknown error", NotificationType.Error));
        }

        public IClassicDesktopStyleApplicationLifetime Dispatcher
        {
            get
            { 
                return Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime
                    ?? throw new InvalidOperationException("Application lifetime is not a classic desktop style.");
            }
        }
    }
}
