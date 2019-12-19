using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer
{
  /// <summary>
  /// Interaction logic for ObjectViewer.xaml
  /// </summary>
  public partial class ObjectViewer : WindowBase
  {
    public delegate void ModelViewItemBrowseEvent(object sender, string buttonKey);

    public event ModelViewItemBrowseEvent ModelViewItemBrowse;

    public ObjectViewer(string windowTitle, object model)
    {
      this.InitializeComponent();

      this.Title = windowTitle;

      this.ModelObject = model;

      this.uxObjectViewer.ModelViewItemBrowse += this.ModelViewItem_Browse;

      this.uxObjectViewer.Items.Add(this.ModelObject);
    }
    
    public object ModelObject {get; set; }

    private void Accept_Clicked(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.uxObjectViewer.HasValidationError)
        {
          return;
        }

        this.DialogResult = true;

        this.Close();
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message);
      }
    }
    
    private void ModelViewItem_Browse(object sender, string buttonKey)
    {
      this.ModelViewItemBrowse?.Invoke(sender, buttonKey);
    }
  }
}
