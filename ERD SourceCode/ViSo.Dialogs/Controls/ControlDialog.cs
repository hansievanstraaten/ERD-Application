using System;
using System.Windows;
using GeneralExtensions;
using WPF.Tools.BaseClasses;

namespace ViSo.Dialogs.Controls
{
  public static class ControlDialog
  {
    private static ControlWindow window;

    public static bool? ShowDialog(string windowTitle, UserControlBase control, string boolUpdateMethod, bool showOkButton = true, bool showCancelButton = true)
    {
      try
      {
        ControlDialog.window = new ControlWindow(windowTitle, control, boolUpdateMethod, showOkButton, showCancelButton);

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
