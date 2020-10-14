using REPORT.Data.SQLRepository.Agrigates;
using System.Data.Entity.ModelConfiguration;

namespace REPORT.Data.SQLRepository.Mappings
{
	public class ReportCategoryMapping : EntityTypeConfiguration<ReportCategory>
	{
		public ReportCategoryMapping()
		{
			ToTable("ReportCategory");

			HasKey(k => new {  k.CategoryId   });

			Property(colCategoryId =>  colCategoryId.CategoryId).HasColumnName("CategoryId");
			Property(colCategoryName =>  colCategoryName.CategoryName).HasColumnName("CategoryName");
			Property(colIsActive =>  colIsActive.IsActive).HasColumnName("IsActive");
			Property(colParentCategoryId =>  colParentCategoryId.ParentCategoryId).HasColumnName("ParentCategoryId");

		}
	}
}