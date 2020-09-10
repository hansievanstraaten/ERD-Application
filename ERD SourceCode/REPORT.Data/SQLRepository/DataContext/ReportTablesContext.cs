using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.Mappings;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace REPORT.Data.SQLRepository.DataContext
{
	public class ReportTablesContext : DbContext
	{
		public DbSet<ReportMaster> ReportsMaster { get; set; }
		public DbSet<ReportXML> ReportsXML { get; set; }

		public ReportTablesContext() : base(DatabaseConnection.Instance.ConnectionString)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

			modelBuilder.Configurations.Add(new ReportMasterMapping());
			modelBuilder.Configurations.Add(new ReportXMLMapping());

		}
	}
}