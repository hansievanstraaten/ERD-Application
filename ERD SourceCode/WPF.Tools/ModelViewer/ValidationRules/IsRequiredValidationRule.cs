using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GeneralExtensions;

namespace WPF.Tools.ModelViewer.ValidationRules
{
  public class IsRequiredValidationRule : ValidationRule
  {
    private string requiredMessage = "Required Field";

    public bool IsRequired { get; set; }

    public ModelItemTypeEnum ObjectType { get; set;}
    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    {
      switch (this.ObjectType)
      {
          case ModelItemTypeEnum.DatePicker:

            if (value == null && this.IsRequired)
            {
              return new ValidationResult(false, requiredMessage);
            }

            return ValidationResult.ValidResult;

        case ModelItemTypeEnum.CheckBox:
          case ModelItemTypeEnum.ComboBox:
          case ModelItemTypeEnum.EnumBox:
          case ModelItemTypeEnum.SecureString:
          case ModelItemTypeEnum.TextBox:
            default:

            string content = value as String;

            if (content.IsNullEmptyOrWhiteSpace() && this.IsRequired)
            {
              return new ValidationResult(false, requiredMessage);
            }

            return ValidationResult.ValidResult;
      }
    }
  }
}
