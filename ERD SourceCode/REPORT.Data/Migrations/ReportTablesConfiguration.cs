using REPORT.Data.SQLRepository.DataContext;
using System.Data.Entity.Migrations;
using System.Data.SQLite.EF6.Migrations;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data.Migrations
{
	internal sealed class ReportTablesConfiguration : DbMigrationsConfiguration<ReportTablesContext>
	{	
		public ReportTablesConfiguration()
		{
			switch (DatabaseConnection.Instance.StorageType)
            { 
				case StorageTypeEnum.SQLite:
					base.AutomaticMigrationsEnabled = false;
					SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
					break;

                case StorageTypeEnum.MsSql: 
					base.AutomaticMigrationsEnabled = true;
					break;
            }

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
