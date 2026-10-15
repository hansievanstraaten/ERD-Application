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
      InitializeComponent();

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
