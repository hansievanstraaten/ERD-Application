using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REPORT.Data.SQLRepository.Agrigates
{
    public class DataSourceMaster : DataSourceMasterBase
	{
        // Inheritance was only done as a POC and there is not real practical reason for doing so

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        new public Int64 MasterReport_Id
        {
            get
            {
                return base.MasterReport_Id;
            }

            set
            {
                base.MasterReport_Id = value;
            }
        }
    }
}