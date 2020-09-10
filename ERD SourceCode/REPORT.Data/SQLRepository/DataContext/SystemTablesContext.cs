using ERD.Models.ReportModels.StructureModels;
using REPORT.Data.SQLRepository.Mappings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace REPORT.Data.SQLRepository.DataContext
{
	public class SystemTablesContext : DbContext
	{
		public DbSet<Lookup> Lookups { get; set; }

		public SystemTablesContext() : base(DatabaseConnection.Instance.ConnectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

			modelBuilder.Configurations.Add(new LookupMapping());

		}
	}
}