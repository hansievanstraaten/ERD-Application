using REPORT.Data.SQLRepository.DataContext;
using System.Data.Entity.Migrations;

namespace REPORT.Data.Migrations
{
	internal sealed class ReportTablesConfiguration : DbMigrationsConfiguration<ReportTablesContext>
	{	
		public ReportTablesConfiguration()
		{
			base.AutomaticMigrationsEnabled = true;

			AutomaticMigrationDataLossAllowed = false;
			MigrationsNamespace = "REPORT.Data.Migrations.ReportTablesConfig";
			ContextKey = "ReportTablesConfiguration";
		}

		protected override void Seed(ReportTablesContext context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method
			//  to avoid creating duplicate seed data.
		}
	}
}
