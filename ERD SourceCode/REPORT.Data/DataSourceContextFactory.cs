using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using REPORT.Data.SQLRepository.DataContext;
using System;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data
{
    public class DataSourceContextFactory
    {
        public static DataSourceContext CreateDataSourceContext(StorageTypeEnum storageType, string connectionString)
        {
            DbContextOptionsBuilder<DataSourceContext> optionsBuilder = new DbContextOptionsBuilder<DataSourceContext>();

            switch (storageType)
            {
                case StorageTypeEnum.SQLite:

                    optionsBuilder.UseSqlite(connectionString, options => options.MigrationsAssembly("REPORT.Migrations.SQLite"));

                    break;

                case StorageTypeEnum.MsSql:

                    optionsBuilder.UseSqlServer(connectionString, options => options.MigrationsAssembly("REPORT.Migrations.SqlServer"));

                    break;

                case StorageTypeEnum.Postgres:

                    optionsBuilder.UseNpgsql(connectionString, options => options.MigrationsAssembly("REPORT.Migrations.SqlServer"));

                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(storageType));
            }

            DataSourceContext response = new DataSourceContext(optionsBuilder.Options);

            IRelationalDatabaseCreator creator = response.Database.GetService<IRelationalDatabaseCreator>();

            if (!creator.Exists())
            {
                creator.Create();
            }

            if (!creator.HasTables())
            {
                creator.CreateTables();
            }

            return response;
        }
    }
}
