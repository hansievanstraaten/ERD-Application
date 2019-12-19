using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Tools.ModelViewer
{
  public class ModelViewerToolTip : StackPanel
  {
    private ModelViewer viewer;

    public ModelViewerToolTip()
    {
      this.Loaded += this.ModelViewerToolTip_Loaded;

      this.ClassObjects = new List<object>();
    }

    public List<object> ClassObjects;

    private void ModelViewerToolTip_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.Children.Clear();

        this.viewer = new ModelViewer();

        this.viewer.Items.AddRange(this.ClassObjects.ToArray());

        this.Children.Add(this.viewer);
      }
      catch
      {
        // DO NOTHING
      }
    }
  }
}
