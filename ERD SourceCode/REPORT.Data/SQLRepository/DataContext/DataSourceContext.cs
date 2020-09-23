using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Mappings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace REPORT.Data.SQLRepository.DataContext
{
	public class DataSourceContext : DbContext
	{
		public DbSet<DataSourceMaster> DataSourcesMaster { get; set; }
		public DbSet<DataSourceTable> DataSourceTables { get; set; }

		public DataSourceContext() : base(DatabaseConnection.Instance.ConnectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

			modelBuilder.Configurations.Add(new DataSourceMasterMapping());
			modelBuilder.Configurations.Add(new DataSourceTableMapping());

		}
	}
}