using REPORT.Data.SQLRepository.DataContext;
using System.Data.Entity.Migrations;

namespace REPORT.Data.Migrations
{
	internal sealed class SystemTablesConfiguration : DbMigrationsConfiguration<SystemTablesContext>
	{
		public SystemTablesConfiguration()
		{
			AutomaticMigrationsEnabled = true;

			AutomaticMigrationDataLossAllowed = false;
			MigrationsNamespace = "REPORT.Data.Migrations.SystemTablesConfig";
			ContextKey = "SystemTablesConfiguration";
		}

		protected override void Seed(SystemTablesContext context)
		{
		}
	}
}
