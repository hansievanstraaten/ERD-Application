using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPF.Tools.BaseClasses;

namespace WPF.Tools.Exstention
{
    public static class UIElmentExstentions
    {
        public static UIElement FindParentControlBase(this UIElement element, Type controlType)
        {
            DependencyObject dep = element;

            while (dep != null)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(dep);

                if (parent == null)
                {
                    return null;
                }

                if (parent.GetType() == controlType || parent.GetType().BaseType == controlType)
                {
                    return parent as UIElement;
                }

                dep = parent;
            }

            return null;
        }

        public static UIElement[] FindVisualControls(this FrameworkElement parent)
        {
            List<UIElement> itemList = new List<UIElement>();

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(parent); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, x);

                itemList.Add(child as UIElement);

                itemList.AddRange(FindVisualChildren<UIElement>(child).ToArray());
            }

            return itemList.ToArray();
        }

        public static UIElement[] FindVisualControls(this FrameworkElement parent, Type controlType)
        {
            List<UIElement> itemList = new List<UIElement>();

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(parent); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, x);

                if (child == null)
                {
                    continue;
                }

                if (child.GetType() == controlType)
                {
                    itemList.Add(child as UIElement);
                }

                itemList.AddRange(FindVisualChildren<UIElement>(child, controlType).ToArray());
            }

            return itemList.ToArray();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject obj, Type objType) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int x = 0; x < VisualTreeHelper.GetChildrenCount(obj); x++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, x);

                    if (child != null && child is T && child.GetType() == objType)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOf in FindVisualChildren<T>(child, objType))
                    {
                        yield return childOf;
                    }
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int x = 0; x < VisualTreeHelper.GetChildrenCount(obj); x++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, x);

                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOf in FindVisualChildren<T>(child))
                    {
                        yield return childOf;
                    }
                }
            }
        }

        public static void CloseIfNotMainWindow(this FrameworkElement element, bool dialogResult)
        {
            WindowBase parentWindow = element.GetParentWindow();

            if (parentWindow == null || parentWindow == Application.Current.MainWindow)
            {
                return;
            }

            //parentWindow.DialogResult = dialogResult;
            //if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            //if (parentWindow.IsModel)
            //{
            //}
            //else
            //{   // This is for closing the ControlDialog when not Modal
            //  parentWindow.SetPropertyValue("DialogResult", dialogResult);
            //}
            parentWindow.SetPropertyValue("DialogResult", dialogResult);

            parentWindow.Close();
        }

        public static WindowBase GetParentWindow(this FrameworkElement element)
        {
            DependencyObject dep = element;

            while (dep != null)
            {
                DependencyObject obj = LogicalTreeHelper.GetParent(dep);

                if (obj is WindowBase)
                {
                    return obj as WindowBase;
                }

                dep = obj;
            }

            return null;
        }

    }
}
