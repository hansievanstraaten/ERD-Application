using Microsoft.EntityFrameworkCore;
using REPORT.Data.SQLRepository.Agrigates;

namespace REPORT.Data.Common
{
    internal static class ContextManagement
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportConnection>()
                .HasKey(x => new
                {
                    x.MasterReport_Id,
                    x.ReportConnectionName
                });

            modelBuilder.Entity<ReportXML>()
                .HasKey(x => new
                {
                    x.ReportXMLVersion,
                    x.MasterReport_Id
                });

            modelBuilder.Entity<DataSourceTable>()
                .HasKey(x => new
                {
                    x.MasterReport_Id,
                    x.TableName
                });

            modelBuilder.Entity<Lookup>()
                .HasKey(x => new
                {
                    x.LookupGroup,
                    x.GroupKey
                });

            modelBuilder.Entity<ReportXMLPrintParameter>()
                .HasKey(x => new
                {
                    x.TableName,
                    x.ColumnName,
                    x.ReportXMLVersion,
                    x.MasterReport_Id
                });

            modelBuilder.Entity<ReportMaster>()
                .HasOne(x => x.ReportCategory)
                .WithMany(x => x.Reports)
                .HasForeignKey(x => x.CategoryId);

            modelBuilder.Entity<ReportConnection>()
                .HasOne(m => m.ReportMaster)
                .WithMany(m => m.ReportConnections)
                .HasForeignKey(m => m.MasterReport_Id);

            modelBuilder.Entity<ReportXML>()
                .HasOne(r => r.ReportMaster)
                .WithMany(r => r.ReportXML)
                .HasForeignKey(r => r.MasterReport_Id);

            modelBuilder.Entity<ReportXMLPrintParameter>()
                .HasOne(p => p.ReportXML)
                .WithMany(p => p.Parameters)
                .HasForeignKey(p => new { p.ReportXMLVersion, p.MasterReport_Id });
        }
    }
}
