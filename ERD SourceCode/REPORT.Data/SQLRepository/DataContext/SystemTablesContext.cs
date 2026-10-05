using Microsoft.EntityFrameworkCore;
using REPORT.Data.Common;
using REPORT.Data.SQLRepository.Agrigates;
namespace REPORT.Data.SQLRepository.DataContext
{
	public class SystemTablesContext : DbContext
	{
		public DbSet<Lookup> Lookups { get; set; }

		public SystemTablesContext(DbContextOptions<SystemTablesContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            //modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            //modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 6));

            //modelBuilder.Configurations.Add(new LookupMapping());

            ContextManagement.OnModelCreating(modelBuilder);


        }
	}
}