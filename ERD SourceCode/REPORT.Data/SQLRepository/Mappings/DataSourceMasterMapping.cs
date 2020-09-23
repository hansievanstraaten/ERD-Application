using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class DataSourceMasterMapping : EntityTypeConfiguration<DataSourceMaster>
	{
		public DataSourceMasterMapping()
		{
			ToTable("DataSourceMaster");

			HasKey(k => new {  k.MasterReport_Id   });

			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colMainTableName =>  colMainTableName.MainTableName).HasColumnName("MainTableName");

		}
	}
}