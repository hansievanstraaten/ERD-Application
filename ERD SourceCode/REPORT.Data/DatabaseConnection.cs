using ERD.Models.ReportModels;
using GeneralExtensions;
using System;
using System.IO;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data
{
	public sealed class DatabaseConnection : IDisposable
    {
        private static DatabaseConnection databaseConnection_Instance = null;

        private static readonly object lockObject = new object();

        private readonly string msSqlConnectionString = "Server={0};Database={1};User ID={2};Password={3};Trusted_Connection={4}";

        private string connectionString;

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

        public string ConnectionString 
        {
            get
            {
                return this.connectionString.IsNullEmptyOrWhiteSpace() ?
                    "Server=HO-PRG-005\\SQLEXPRESS;Database=ERD_Print;User ID=LocalUser;Password=LocalUser;Trusted_Connection=True" : 
                    this.connectionString;
            }

            private set
			{
                this.connectionString = value;
			}
        }

        public void InitializeConnectionString(ReportSetupModel setupModel)
        {
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

                    //"Data Source=MyDatabase.sqlite;Version=3;");

     //               string sqLitePath = $"{setupModel.FileDirectory}\\ERD_Reports.sqlite";

     //               SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder
     //               {
     //                   DataSource = $"{sqLitePath};Version=3;"
     //               };

     //               this.ConnectionString = builder.ConnectionString;

     //               if (!File.Exists(sqLitePath))
					//{
     //                   SQLiteConnection.CreateFile(sqLitePath);
     //               }

                    break;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
