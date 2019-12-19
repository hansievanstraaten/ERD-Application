using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ViSo.Dialogs.ModelViewer
{
  public static class ModelView
  {
    public delegate void OnItemBrowseEvent(object sender, object model, string buttonKey);

    public static event OnItemBrowseEvent OnItemBrowse;

    private static ViewerWindow viewer;

    public static bool? ShowDialog(string windowTitle, object model)
    {
      try
      {
        ModelView.viewer = new ViewerWindow(windowTitle, model);

        ModelView.viewer.OnItemBrowse += ModelView.OnItem_Browse;

        return ModelView.viewer.ShowDialog();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
      finally
      {
        ModelView.viewer.OnItemBrowse -= ModelView.OnItem_Browse;

        ModelView.viewer = null;
      }

      return null;
    }

    public static bool? ShowDialog(string windowTitle, object[] models)
    {
      try
      {
        ModelView.viewer = new ViewerWindow(windowTitle, models);

        ModelView.viewer.OnItemBrowse += ModelView.OnItem_Browse;

        return ModelView.viewer.ShowDialog();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
      finally
      {
        ModelView.viewer.OnItemBrowse -= ModelView.OnItem_Browse;

        ModelView.viewer = null;
      }

      return null;
    }
    
    public static bool? ShowDialog(Window owner, bool topMost, string windowTitle, object model)
    {
      try
      {
        ModelView.viewer = new ViewerWindow(windowTitle, model);

        ModelView.viewer.OnItemBrowse += ModelView.OnItem_Browse;

        ModelView.viewer.Owner = owner;

        ModelView.viewer.Topmost = topMost;

        return ModelView.viewer.ShowDialog();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
      finally
      {
        ModelView.viewer.OnItemBrowse -= ModelView.OnItem_Browse;

        ModelView.viewer = null;
      }

      return null;
    }

    public static ViewerWindow Viewer
    {
      get
      {
        return ModelView.viewer;
      }
    }

    private static void OnItem_Browse(object sender, object model, string buttonKey)
    {
      try
      {
        ModelView.OnItemBrowse?.Invoke(sender, model, buttonKey);
      }
      catch (Exception err)
      {
        MessageBox.Show(err.InnerExceptionMessage());
      }
    }

    public static object ModelObject { get; set;}

  }
}
