using System;
using System.Threading.Tasks;
using ERD.Viewer.Tools.Services;

namespace ERD.Viewer.Tools.Utilities
{
    /// <summary>
    /// Helpers for running tasks with automatic exception routing to the application's ExceptionRouter.
    /// Use either TaskUtilities.Run(...) instead of Task.Run(...) or call .WithExceptionRouting() on any Task.
    /// </summary>
    public static class TaskUtilities
    {
        public static Task Run(Action action)
        {
            var t = Task.Run(action);
            AttachExceptionRouting(t);
            return t;
        }

        public static Task Run(Func<Task> func)
        {
            var t = Task.Run(func);
            AttachExceptionRouting(t);
            return t;
        }

        public static Task<T> Run<T>(Func<T> func)
        {
            var t = Task.Run(func);
            AttachExceptionRouting(t);
            return t;
        }

        public static Task<T> Run<T>(Func<Task<T>> func)
        {
            var t = Task.Run(func);
            AttachExceptionRouting(t);
            return t;
        }

        public static Task WithExceptionRouting(this Task task)
        {
            AttachExceptionRouting(task);
            return task;
        }

        public static Task<T> WithExceptionRouting<T>(this Task<T> task)
        {
            AttachExceptionRouting(task);
            return task;
        }

        private static void AttachExceptionRouting(Task task)
        {
            if (task == null) return;

            // Attach a continuation that will route unhandled exceptions to the ExceptionRouter.
            task.ContinueWith(t =>
            {
                var ex = t.Exception?.Flatten().InnerException ?? t.Exception;
                if (ex != null)
                {
                    // Fire-and-forget routing
                    _ = ExceptionRouter.RouteAsync(ex);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
