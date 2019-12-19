using WPF.Tools.BaseClasses;

namespace ERD.Viewer
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : ApplicationBase
  {
    private void ApplicationBase_Exit(object sender, System.Windows.ExitEventArgs e)
    {
      try
      {
        this.Dispatcher.InvokeShutdown();
      }
      catch
      {
        // DO NOTHING
      }
    }
  }
}
