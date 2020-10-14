using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class ReportCategory : ReportCategoryBase
	{
		// NOTE: For PK that is not identity add the 
		// [Key]
		// [DatabaseGenerated(DatabaseGeneratedOption.None)]
		// Attributes

		// Exsample
		//[Key]
		//[DatabaseGenerated(DatabaseGeneratedOption.None)]
		//new public Int64 MasterReport_Id
		//{
		//	get
		//	{
		//		return base.MasterReport_Id;
		//	}

		//	set
		//	{
		//		base.MasterReport_Id = value;
		//	}
		//}
	}
}