using GeneralExtensions;
using REPORT.Data.SQLRepository.Agrigates;
using REPORT.Data.SQLRepository.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace REPORT.Data.SQLRepository.Repositories
{
	public abstract class SystemTablesRepository_Base
	{
		public SystemTablesContext dataContext;

		public SystemTablesRepository_Base()
		{
			this.dataContext = new SystemTablesContext();
		}

		public LookupModel GetLookupByPrimaryKey (string LookupGroup, int GroupKey )
		{
			Lookup result =this.dataContext
				.Lookups
				.FirstOrDefault(pk => pk.LookupGroup == LookupGroup && pk.GroupKey == GroupKey  );

			if (result == null)
			{
				return null;
			}

			return result.CopyToObject(new LookupModel()) as LookupModel;
		}



	}
}