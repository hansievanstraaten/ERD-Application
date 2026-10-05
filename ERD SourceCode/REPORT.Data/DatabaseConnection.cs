using ERD.Common;
using ERD.Models.ReportModels;
using GeneralExtensions;
using System;
using System.Data.Common;
using System.IO;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data
{
    public sealed class DatabaseConnection : IDisposable
    {
        private static DatabaseConnection databaseConnection_Instance = null;

        private static readonly object lockObject = new object();

        private readonly string msSqlConnectionString = "Server={0};Database={1};User ID={2};Password={3};Trusted_Connection={4};Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";

        private readonly string sqLiteConnectionString = "Data Source={0};Foreign Keys=True;";

        private readonly string postgreSqlConnectionString = "Host={0};Port={1};Database={2};Username={3};Password={4};Trust Server Certificate=true;";

        private readonly string defaultSqliteDatabaseFileName = "{0}.sqlite";

        private string connectionString;

        private StorageTypeEnum storageType = StorageTypeEnum.MsSql;

        public static DatabaseConnection Instance
        {
            get
            {
                if (databaseConnection_Instance == null)
                {
                    lock (lockObject)
                    {
                        if (databaseConnection_Instance == null)
                        {
                            databaseConnection_Instance = new DatabaseConnection();
                        }
                    }
                }

                return databaseConnection_Instance;
            }
        }

        ~DatabaseConnection()
        {
            this.Dispose();
        }

        public string ConnectionString
        {
            get
            {
                return this.connectionString.IsNullEmptyOrWhiteSpace() ?
                    "Server=HO-PRG-005\\SQLEXPRESS;Database=ERD_Print;User ID=LocalUser;Password=LocalUser;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;" :
                    this.connectionString;
            }

            private set
            {
                this.connectionString = value;
            }
        }

        public string ProviderInvariantName
        {
            get
            {
                switch (this.storageType)
                {
                    case StorageTypeEnum.SQLite:
                        return "System.Data.SQLite";

                    case StorageTypeEnum.Postgres:
                        return "Npgsql";

                    case StorageTypeEnum.MsSql:
                    default:
                        return "System.Data.SqlClient";
                }
            }
        }

        public StorageTypeEnum StorageType
        {
            get
            {
                return this.storageType;
            }
        }

        public DbConnection CreateConnection()
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(this.ProviderInvariantName);

            DbConnection connection = factory.CreateConnection();

            if (connection == null)
            {
                throw new ApplicationException($"Unable to create provider connection for '{this.ProviderInvariantName}'.");
            }

            connection.ConnectionString = this.ConnectionString;

            return connection;
        }

        public void InitializeConnectionString(ReportSetupModel setupModel)
        {
            if (setupModel == null)
            {
                throw new ApplicationException("Database setup model not provided.");
            }

            this.storageType = setupModel.StorageType;

            switch (setupModel.StorageType)
            {
                case StorageTypeEnum.MsSql:

                    object[] args = new object[]
                    {
                      setupModel.DataBaseSource.ServerName,
                      setupModel.DataBaseSource.DatabaseName,
                      setupModel.DataBaseSource.UserName,
                      setupModel.DataBaseSource.Password,
                      setupModel.DataBaseSource.TrustedConnection
                    };

                    this.ConnectionString = String.Format(msSqlConnectionString, args);

                    break;

                case StorageTypeEnum.SQLite:
                    if (setupModel.FileDirectory.IsNullEmptyOrWhiteSpace())
                    {
                        throw new ApplicationException("DB file location is required for SQLite setup.");
                    }

                    Directory.CreateDirectory(setupModel.FileDirectory);

                    string databaseFileName = setupModel.DataBaseSource?.DatabaseName;

                    if (databaseFileName.IsNullEmptyOrWhiteSpace())
                    {
                        databaseFileName = string.Format(this.defaultSqliteDatabaseFileName, General.ProjectModel.ModelName);
                    }
                    else if (Path.GetExtension(databaseFileName).IsNullEmptyOrWhiteSpace())
                    {
                        databaseFileName = $"{databaseFileName}.sqlite";
                    }

                    string sqLitePath = Path.Combine(setupModel.FileDirectory, databaseFileName);

                    this.ConnectionString = String.Format(this.sqLiteConnectionString, sqLitePath);

                    break;

                case StorageTypeEnum.Postgres:
                    object[] pgArgs = new object[]
                    {
                      setupModel.DataBaseSource.ServerName,
                      setupModel.DataBaseSource.PortName,
                      setupModel.DataBaseSource.DatabaseName,
                      setupModel.DataBaseSource.UserName,
                      setupModel.DataBaseSource.Password
                    };

                    string connection = setupModel.DataBaseSource.SSLMode ?
                        $"{String.Format(this.postgreSqlConnectionString, pgArgs)}SSL Mode=Require;"
                        :
                        String.Format(this.postgreSqlConnectionString, pgArgs);

                    if (setupModel.DataBaseSource.TrustedConnection)
                    {
                        connection += "Trust Server Certificate=true;";
                    }

                    this.ConnectionString = connection;
                    break;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
