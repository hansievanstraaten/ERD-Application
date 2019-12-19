using GeneralExtensions;
using System;
using System.Windows;

namespace ViSo.Dialogs.Input
{
  public static class InputBox
  {
    private static ViewerWindow viewer;

    private static InputBoxModel InputModel { get; set; }

    public static bool? ShowDialog(string windowTitle, string fieldCaption)
    {
      try
      {
        InputBox.InputModel = new InputBoxModel();

        InputBox.viewer = new ViewerWindow(windowTitle, fieldCaption, InputBox.InputModel);

        return InputBox.viewer.ShowDialog();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
      finally
      {
        InputBox.viewer = null;
      }

      return null;
    }

    public static bool? ShowDialog(Window owner, bool topMost, string windowTitle, string fieldCaption)
    {
      try
      {
        InputBox.InputModel = new InputBoxModel();

        InputBox.viewer = new ViewerWindow(windowTitle, fieldCaption, InputBox.InputModel);

        InputBox.viewer.Owner = owner;

        InputBox.viewer.Topmost = topMost;

        return InputBox.viewer.ShowDialog();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
      finally
      {
        InputBox.viewer = null;
      }

      return null;
    }

    public static string Result
    {
      get
      {
        return InputBox.InputModel.Value;
      }
    }
  }
}
