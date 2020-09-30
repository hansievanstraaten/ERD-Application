using GeneralExtensions;
using System;
using System.Xml.Linq;
using ViSo.SharedEnums;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace REPORT.Data.Models
{
    [ModelNameAttribute("Section Link")]
    [Serializable]
    public class WhereParameterModel : ModelsBase
    {
        private string columnName;
        private string parameterName;
        private SqlWhereOperatorsEnum andOrOperator;
        private int operatorIndex;

        public XElement ItemXml
        {
            get
            {
                XElement result = new XElement("WhereParameter");

                result.Add(new XAttribute("ObjectType", "WhereParameterModel"));
                result.Add(new XAttribute("ColumnName", this.ColumnName));
                result.Add(new XAttribute("ParameterName", this.ParameterName));
                result.Add(new XAttribute("AndOrOperator", this.AndOrOperator));
                result.Add(new XAttribute("OperatorIndex", this.OperatorIndex));

                return result;
            }

            set
            {
                foreach (XAttribute element in value.Attributes())
                {
                    this.SetPropertyValue(element.Name.LocalName, element.Value);
                }
            }
        }

        [FieldInformation("Column Name", IsRequired = true, Sort = 1)]
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }

            set
            {
                this.columnName = value;

                base.OnPropertyChanged(() => this.ColumnName);
            }
        }

        [FieldInformation("Parameter Name", IsRequired = true, Sort = 2)]
        public string ParameterName
        {
            get
            {
                return this.parameterName;
            }

            set
            {
                this.parameterName = value;

                base.OnPropertyChanged(() => this.ParameterName);
            }
        }

        [FieldInformation("AND / OR", IsRequired = true, Sort = 3)]
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

        public int OperatorIndex
        { 
            get
            {
                return this.operatorIndex;
            }
            
            set
            {
                this.operatorIndex = value;

                base.OnPropertyChanged(() => this.OperatorIndex);
            }
        }
    }
}
