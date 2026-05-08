using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using ERD.Viewer.Tools.Services;
using System;
using System.Threading.Tasks;

namespace ERD.Viewer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            Styles.Add(new StyleInclude(new Uri("avares://ERD.Viewer.Tools/"))
            {
                Source = new Uri("avares://ERD.Viewer.Tools/Styles/WindowStyles.axaml")
            });

            Styles.Add(new StyleInclude(new Uri("avares://ERD.Viewer.Tools/"))
            {
                Source = new Uri("avares://ERD.Viewer.Tools/Styles/ApplicationColours.axaml")
            });
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // register global handlers early so we catch exceptions during startup
            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
                if (ev.ExceptionObject is Exception ex)
                {
                    _ = ExceptionRouter.RouteAsync(ex);
                }
            };

            TaskScheduler.UnobservedTaskException += (s, ev) =>
            {
                _ = ExceptionRouter.RouteAsync(ev.Exception);
                ev.SetObserved();
            };

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}