using GeneralExtensions;
using REPORT.Data.SQLRepository.DataContext;
using System.Linq;
using System.Reflection;

namespace REPORT.Data.SQLRepository
{
	public class DbScript
	{
		private ReportTablesContext contex;

		public void InitializeReportsDB()
		{
			this.contex = new ReportTablesContext();

			var script = typeof(Properties.Resources)
			  .GetProperties(BindingFlags.Static | BindingFlags.NonPublic |
							 BindingFlags.Public)
			  .Where(p => p.PropertyType == typeof(string) && p.Name == "ERD_Print")
			  .Select(x => new { SqlScript = x.GetValue(null, null) })
			  .FirstOrDefault();
				
			this.contex.Database.ExecuteSqlCommand(script.SqlScript.ParseToString(), new object[] { });
		}
	}
}