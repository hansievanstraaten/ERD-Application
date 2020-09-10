using ERD.Models.ReportModels;
using System;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data
{
    public sealed class DatabaseConnection : IDisposable
    {
        private static DatabaseConnection databaseConnection_Instance = null;

        private static readonly object lockObject = new object();

        private readonly string rawConnectionString = "Server={0};Database={1};User ID={2};Password={3};Trusted_Connection={4}";

        public static DatabaseConnection Instance
        {
            get
            {
                if (databaseConnection_Instance == null)
                {
                    lock(lockObject)
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

        public bool IsDatabaseSetup { get; private set; }

        public string FilesDirectory { get; private set; }

        public string ConnectionString { get; private set; }

        public void InitializeConnectionString(ReportSetupModel setupModel)
        {
            this.IsDatabaseSetup = setupModel.StorageType == StorageTypeEnum.DatabaseSystem;

            if (this.IsDatabaseSetup)
            {
                object[] args = new object[]
                {
                  setupModel.DataBaseSource.ServerName,
                  setupModel.DataBaseSource.DatabaseName,
                  setupModel.DataBaseSource.UserName,
                  setupModel.DataBaseSource.Password,
                  setupModel.DataBaseSource.TrustedConnection
                };

                this.ConnectionString = String.Format(rawConnectionString, args);
            }
            else
            {
                this.FilesDirectory = setupModel.FileDirectory;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
