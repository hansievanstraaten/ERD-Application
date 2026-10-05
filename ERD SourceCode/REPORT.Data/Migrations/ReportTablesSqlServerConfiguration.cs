using REPORT.Data.SQLRepository.DataContext;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Data.SQLite.EF6.Migrations;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data.Migrations
{
    public class ReportTablesSqlServerConfiguration : DbMigrationsConfiguration<DataSourceContext>
    {
        public ReportTablesSqlServerConfiguration()
        {
            base.AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
            SetSqlGenerator("System.Data.SqlClient", new SqlServerMigrationSqlGenerator());
            MigrationsNamespace = "REPORT.Data.Migrations.ReportTablesSQL";
            ContextKey = "ReportTablesSqlServerConfiguration";
        }
    }
}
