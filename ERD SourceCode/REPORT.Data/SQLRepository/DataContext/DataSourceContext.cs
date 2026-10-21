using Microsoft.EntityFrameworkCore;
using REPORT.Data.Common;
using REPORT.Data.SQLRepository.Agrigates;

namespace REPORT.Data.SQLRepository.DataContext
{
	public class DataSourceContext : DbContext
    {
        #region DATA SOURCE TABLES
        public DbSet<DataSourceMaster> DataSourcesMaster { get; set; }
        
        public DbSet<DataSourceTable> DataSourceTables { get; set; }

        #endregion

        #region REPORT TABLES

        public DbSet<ReportMaster> ReportsMaster { get; set; }

        public DbSet<ReportXML> ReportsXML { get; set; }

        public DbSet<ReportConnection> ReportConnections { get; set; }

        public DbSet<ReportCategory> ReportCategories { get; set; }

        public DbSet<ReportXMLPrintParameter> ReportXMLPrintParameters { get; set; }

        #endregion

        #region SYSTEM TABLES

        public DbSet<Lookup> Lookups { get; set; }

        #endregion

        public DataSourceContext(DbContextOptions<DataSourceContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ContextManagement.OnModelCreating(modelBuilder);
        }
    }
}