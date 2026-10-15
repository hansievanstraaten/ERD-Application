using System.Windows;
using System.Windows.Media;

namespace WPF.Tools.Helpers
{
    public class WPFHelpers
    {
        public static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child =
                    VisualTreeHelper.GetChild(parent, i);

                if (child is T result)
                    return result;

                T? descendant = FindVisualChild<T>(child);

                if (descendant is not null)
                    return descendant;
            }

            return null;
        }
    }
}
