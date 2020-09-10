using ERD.Models.ReportModels.StructureModels;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class LookupMapping : EntityTypeConfiguration<Lookup>
	{
		public LookupMapping()
		{
			ToTable("Lookup");

			HasKey(k => new {  k.LookupGroup , k.GroupKey   });

			Property(colLookupGroup =>  colLookupGroup.LookupGroup).HasColumnName("LookupGroup");
			Property(colGroupKey =>  colGroupKey.GroupKey).HasColumnName("GroupKey");
			Property(colGroupDescription =>  colGroupDescription.GroupDescription).HasColumnName("GroupDescription");

		}
	}
}