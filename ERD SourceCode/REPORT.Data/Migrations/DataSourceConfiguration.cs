using REPORT.Data.SQLRepository.DataContext;
using System.Data.Entity.Migrations;

namespace REPORT.Data.Migrations
{

	internal sealed class DataSourceConfiguration : DbMigrationsConfiguration<DataSourceContext>
    {
        public DataSourceConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DataSourceContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
