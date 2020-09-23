using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class DataSourceTableMapping : EntityTypeConfiguration<DataSourceTable>
	{
		public DataSourceTableMapping()
		{
			ToTable("DataSourceTable");

			HasKey(k => new {  k.MasterReport_Id , k.TableName   });

			Property(colMasterReport_Id =>  colMasterReport_Id.MasterReport_Id).HasColumnName("MasterReport_Id");
			Property(colTableName =>  colTableName.TableName).HasColumnName("TableName");
			Property(colIsAvailable =>  colIsAvailable.IsAvailable).HasColumnName("IsAvailable");

		}
	}
}