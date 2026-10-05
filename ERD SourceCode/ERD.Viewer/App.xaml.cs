using System.Data.Common;
using System.Data.SQLite;
using WPF.Tools.BaseClasses;

namespace ERD.Viewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ApplicationBase
    {
        public App()
        {
            DbProviderFactories.RegisterFactory("System.Data.SQLite", SQLiteFactory.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
        }

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
