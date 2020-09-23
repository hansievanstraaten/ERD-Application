using System;
using ViSo.SharedEnums;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.Models
{
    [ModelNameAttribute("Section Link")]
    [Serializable]
    public class SectionLinkModel : ModelsBase
    {
        private string parentTable;
        private string sectionColumn;
        private SqlWhereOperatorsEnum andOrOperator;
        private string parentColumn;

        [FieldInformation("Canvas Name", IsRequired = true, Sort = 1)]
        public string ParentTable
        {
            get
            {
                return this.parentTable;
            }

            set
            {
                this.parentTable = value;

                base.OnPropertyChanged(() => this.ParentTable);
            }
        }

        [FieldInformation("Section Column", IsRequired = true, Sort = 1)]
        public string SectionColumn
        {
            get
            {
                return this.sectionColumn;
            }

            set
            {
                this.sectionColumn = value;

                base.OnPropertyChanged(() => this.SectionColumn);
            }
        }

        [FieldInformation("AND / OR", IsRequired = true, Sort = 1)]
        public SqlWhereOperatorsEnum AndOrOperator
        {
            get
            {
                return this.andOrOperator;
            }

            set
            {
                this.andOrOperator = value;

                base.OnPropertyChanged(() => this.AndOrOperator);
            }
        }

        [FieldInformation("Parent Column", IsRequired = true, Sort = 1)]
        public string ParentColumn
        {
            get
            {
                return this.parentColumn;
            }

            set
            {
                this.parentColumn = value;

                base.OnPropertyChanged(() => this.ParentColumn);
            }
        }

    }
}
