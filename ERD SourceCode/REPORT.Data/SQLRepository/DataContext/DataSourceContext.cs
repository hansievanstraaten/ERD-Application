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

            // We'll deal with your existing mappings next.
            //modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            //modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

            //modelBuilder.Configurations.Add(new DataSourceMasterMapping());
            //modelBuilder.Configurations.Add(new DataSourceTableMapping());
        }

        //      public DataSourceContext() : base(DatabaseConnection.Instance.CreateConnection(), true)
        //{
        //	switch (DatabaseConnection.Instance.StorageType)
        //	{
        //		case StorageTypeEnum.SQLite:
        //			Database.SetInitializer(new CreateDatabaseIfNotExists<DataSourceContext>());
        //			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataSourceContext, DataSourceConfiguration>());
        //			break;

        //		case StorageTypeEnum.MsSql:
        //			Database.SetInitializer(new CreateDatabaseIfNotExists<DataSourceContext>());
        //			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataSourceContext, DataSourceConfiguration>());
        //			break;
        //	}
        //}

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //	modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
        //	modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

        //	modelBuilder.Configurations.Add(new DataSourceMasterMapping());
        //	modelBuilder.Configurations.Add(new DataSourceTableMapping());

        //}
    }
}