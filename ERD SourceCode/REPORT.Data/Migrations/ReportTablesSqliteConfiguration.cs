using REPORT.Data.SQLRepository.DataContext;
using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;

namespace REPORT.Data.Migrations
{
    public class ReportTablesSqliteConfiguration : DbMigrationsConfiguration<DataSourceContext>
    {
        public ReportTablesSqliteConfiguration() 
        {
            base.AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
            MigrationsNamespace = "REPORT.Data.Migrations.ReportTablesSqlite";
            ContextKey = "ReportTablesSqliteConfiguration";
        }
    }
}
