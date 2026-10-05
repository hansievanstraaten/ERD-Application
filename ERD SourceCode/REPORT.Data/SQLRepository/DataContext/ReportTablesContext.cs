using Microsoft.EntityFrameworkCore;
using REPORT.Data.Common;
using REPORT.Data.SQLRepository.Agrigates;

namespace REPORT.Data.SQLRepository.DataContext
{
	public class ReportTablesContext : DbContext
    {
		public DbSet<ReportMaster> ReportsMaster { get; set; }
		public DbSet<ReportXML> ReportsXML { get; set; }
		public DbSet<ReportConnection> ReportConnections { get; set; }
		public DbSet<ReportCategory> ReportCategories { get; set; }
		public DbSet<ReportXMLPrintParameter> ReportXMLPrintParameters { get; set; }

		public ReportTablesContext(DbContextOptions<ReportTablesContext> options) : base(options)
		{
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ContextManagement.OnModelCreating(modelBuilder);


            //modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            //modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

            //modelBuilder.Configurations.Add(new ReportMasterMapping());
            //modelBuilder.Configurations.Add(new ReportXMLMapping());
            //modelBuilder.Configurations.Add(new ReportConnectionMapping());
            //modelBuilder.Configurations.Add(new ReportCategoryMapping());
            //modelBuilder.Configurations.Add(new ReportXMLPrintParameterMapping());

        }
    }
}