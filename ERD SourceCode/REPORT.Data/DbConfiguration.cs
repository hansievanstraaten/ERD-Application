using ERD.Models.ReportModels;
using GeneralExtensions;
using Newtonsoft.Json;
using System;
using System.IO;
using ViSo.SharedEnums.ReportEnums;

namespace REPORT.Data
{
	public sealed class DbConfiguration
	{
		private static DbConfiguration instance = null;

		private static readonly object lockObject = new object();

		public static DbConfiguration Instance
		{
			get
			{
				if (instance == null)
				{
					lock(lockObject)
					{
						if (instance == null)
						{
							instance = new DbConfiguration();
						}
					}
				}

				return instance;
			}
		}

		public StorageTypeEnum StorageType
		{
			get
			{
				return this.ReportSetup.StorageType;
			}
		}

		public string ReportSetupFileName { get; private set; }
		
		public ReportSetupModel ReportSetup { get; private set; }

		public bool Save()
		{
			try
			{
				this.ReportSetup.DataBaseSource.Password = this.ReportSetup.DataBaseSource.Password.Encrypt();

				this.ReportSetup.DataBaseSource.IsEncrypted = true;

				string fileObject = JsonConvert.SerializeObject(this.ReportSetup);

				File.WriteAllText(this.ReportSetupFileName, fileObject);

				this.ReportSetup.DataBaseSource.Password = this.ReportSetup.DataBaseSource.Password.Decrypt();

				this.ReportSetup.DataBaseSource.IsEncrypted = false;

				return true;
			}
			catch 
			{
				throw;
			}
		}

		public void Initialize(string reportSetupFilename)
		{
			if (reportSetupFilename.IsNullEmptyOrWhiteSpace())
			{
				throw new ApplicationException("Database setup filename not provided.");
			}

			this.ReportSetupFileName = reportSetupFilename;

			if (File.Exists(this.ReportSetupFileName))
			{
				string fileContent = File.ReadAllText(reportSetupFilename);

				this.ReportSetup = JsonConvert.DeserializeObject(fileContent, typeof(ReportSetupModel)) as ReportSetupModel;

				if (this.ReportSetup.DataBaseSource.IsEncrypted)
				{
					this.ReportSetup.DataBaseSource.Password = this.ReportSetup.DataBaseSource.Password.Decrypt();

					this.ReportSetup.DataBaseSource.IsEncrypted = false;
				}
			}
			else
			{
				this.ReportSetup = new ReportSetupModel();
			}
		}
	}
}
