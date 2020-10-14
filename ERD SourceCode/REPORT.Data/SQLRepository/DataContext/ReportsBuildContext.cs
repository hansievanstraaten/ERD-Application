using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Mappings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace REPORT.Data.SQLRepository.DataContext
{
	public class ReportsBuildContext : DbContext
	{

		public DbSet<DataSourceMaster> DataSourcesMaster { get; set; }
		public DbSet<DataSourceTable> DataSourceTables { get; set; }
		public DbSet<ReportMaster> ReportsMaster { get; set; }
		public DbSet<ReportXML> ReportsXML { get; set; }
		public DbSet<ReportConnection> ReportConnections { get; set; }
		public DbSet<ReportCategory> ReportCategories { get; set; }
		public DbSet<Lookup> Lookups { get; set; }

		public ReportsBuildContext() : base(DatabaseConnection.Instance.ConnectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

			modelBuilder.Configurations.Add(new DataSourceMasterMapping());
			modelBuilder.Configurations.Add(new DataSourceTableMapping());
			modelBuilder.Configurations.Add(new ReportMasterMapping());
			modelBuilder.Configurations.Add(new ReportXMLMapping());
			modelBuilder.Configurations.Add(new ReportConnectionMapping());
			modelBuilder.Configurations.Add(new ReportCategoryMapping());
			modelBuilder.Configurations.Add(new LookupMapping());

		}
	}
}