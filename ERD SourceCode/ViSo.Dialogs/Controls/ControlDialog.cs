using System;
using System.ComponentModel;
using System.Windows;
using GeneralExtensions;
using WPF.Tools.BaseClasses;

namespace ViSo.Dialogs.Controls
{
    public class ControlDialog
    {
        public delegate void WindowsShowIsClosingEvent(object sender, UserControlBase control, CancelEventArgs e);

        public static event WindowsShowIsClosingEvent WindowsShowIsClosing;

        //private static ControlWindow window;

        public static bool? ShowDialog(string windowTitle, UserControlBase control,
            string boolUpdateMethod,
            bool showOkButton = true,
            bool showCancelButton = true,
            WindowState windowState = WindowState.Normal)
        {
            ControlWindow window = null;

            try
            {
                window = new ControlWindow(windowTitle, control, boolUpdateMethod, showOkButton, showCancelButton);

                window.WindowState = windowState;

                return window.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
            finally
            {
                window = null;
            }
        }

        public static void Show(string windowTitle, UserControlBase control,
            string boolUpdateMethod,
            bool showOkButton = true,
            bool showCancelButton = true,
            WindowState windowState = WindowState.Normal)
        {
            try
            {
                ControlWindow window = new ControlWindow(windowTitle, control, boolUpdateMethod, showOkButton, showCancelButton);

                window.WindowState = windowState;

                window.Closing += WindowShow_Closing;

                window.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }            
        }

        private static void WindowShow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                ControlWindow window = sender.To<ControlWindow>();

                window.Closing -= WindowShow_Closing;

                WindowsShowIsClosing?.Invoke(sender, window.UserControl, e);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
    }
}
