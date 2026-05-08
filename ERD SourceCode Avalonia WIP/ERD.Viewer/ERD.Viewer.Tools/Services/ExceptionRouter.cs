using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ERD.Viewer.Tools.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERD.Viewer.Tools.Services
{
    /// <summary>
    /// Routes exceptions through registered handlers. If no handler handles the exception
    /// it will attempt to show the error on the active window implementing ViewerWindowBase.
    /// </summary>
    public static class ExceptionRouter
    {
        private static readonly Func<Exception, Task<bool>>[] EmptyHandlers = Array.Empty<Func<Exception, Task<bool>>>();
        private static readonly object sync = new object();
        private static Func<Exception, Task<bool>>[] handlers = EmptyHandlers;

        public static void AddHandler(Func<Exception, Task<bool>> handler)
        {
            if (handler == null)
            {
                return;
            }

            lock (sync)
            {
                List<Func<Exception, Task<bool>>>? list = handlers.ToList();

                list.Add(handler);

                handlers = list.ToArray();
            }
        }

        public static async Task RouteAsync(Exception err)
        {
            if (err == null)
            {
                return;
            }

            Func<Exception, Task<bool>>[]? snapshot = handlers;

            foreach (Func<Exception, Task<bool>> handle in snapshot)
            {
                try
                {
                    if (await handle(err).ConfigureAwait(false))
                    {
                        return;
                    }
                }
                catch
                {
                    // swallow handler exceptions to keep pipeline running
                }
            }

            // fallback: try to show on active window
            try
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    IClassicDesktopStyleApplicationLifetime? lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                    ViewerWindowBase? active = lifetime?.Windows?.FirstOrDefault(w => w.IsActive) as ViewerWindowBase;
                    if (active != null)
                    {
                        active.ShowError(err);
                    }
                    else
                    {
                        // no active window - try main window
                        ViewerWindowBase? main = lifetime?.MainWindow as ViewerWindowBase;
                        main?.ShowError(err);
                    }
                }).GetTask().ConfigureAwait(false);
            }
            catch
            {
                // last resort: nothing we can do safely
            }
        }
    }
}
