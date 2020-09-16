using System;
using System.Windows;
using GeneralExtensions;
using WPF.Tools.BaseClasses;

namespace ViSo.Dialogs.Controls
{
    public static class ControlDialog
    {
        private static ControlWindow window;

        public static bool? ShowDialog(string windowTitle, UserControlBase control,
            string boolUpdateMethod,
            bool showOkButton = true,
            bool showCancelButton = true,
            WindowState windowState = WindowState.Normal)
        {
            try
            {
                ControlDialog.window = new ControlWindow(windowTitle, control, boolUpdateMethod, showOkButton, showCancelButton);

                ControlDialog.window.WindowState = windowState;

                return ControlDialog.window.ShowDialog();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());

                return false;
            }
            finally
            {
                ControlDialog.window = null;
            }
        }
    }
}
