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

namespace ERD.Viewer.Tools.Common
{
  /// <summary>
  /// Interaction logic for ProgressResponce.xaml
  /// </summary>
  public partial class ProgressResponce : WindowBase
  {
    public ProgressResponce(double maximum)
    {
      this.InitializeComponent();

      this.uxProgress.Maximum = maximum;
    }

    public void UpdateProgress(double progress)
    {
      this.uxProgress.Value = progress;
    }

    public void UpdateMessage(object message)
    {
      this.uxMessage.Content = message;
    }

    public void Reset(double maximum)
    {
      this.uxProgress.Maximum = maximum;

      this.uxProgress.Value = 0;
    }
  }
}
