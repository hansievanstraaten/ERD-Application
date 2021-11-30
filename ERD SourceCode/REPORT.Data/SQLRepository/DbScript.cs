using ERD.Base;
using ERD.Common;
using ERD.Models.ReportModels;
using ERD.Viewer.Database;
using GeneralExtensions;
using REPORT.Data.SQLRepository.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace REPORT.Data.SQLRepository
{
	public class DbScript
	{
		public void InitializeReportsDB(ReportSetupModel reportSetup)
		{
			Dictionary<string, string> connectionValues = new Dictionary<string, string>();

			connectionValues.Add("ServerName", reportSetup.DataBaseSource.ServerName);
			connectionValues.Add("DatabaseName", reportSetup.DataBaseSource.DatabaseName);
			connectionValues.Add("UserName", reportSetup.DataBaseSource.UserName);
			connectionValues.Add("Password", reportSetup.DataBaseSource.Password);
			connectionValues.Add("TrustedConnection", reportSetup.DataBaseSource.TrustedConnection.ParseToString());

			DataAccess dataAccess = new DataAccess(DatabaseTypeEnum.SQL, connectionValues);

			var script = typeof(Properties.Resources)
			  .GetProperties(BindingFlags.Static | BindingFlags.NonPublic |
							 BindingFlags.Public)
			  .Where(p => p.PropertyType == typeof(string) && p.Name == "ERD_Print")
			  .Select(x => new { SqlScript = x.GetValue(null, null) })
			  .FirstOrDefault();

			dataAccess.ExecuteNonQuery(script.SqlScript.ParseToString());
		}
	}
}