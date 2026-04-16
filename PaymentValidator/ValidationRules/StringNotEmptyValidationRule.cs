using System.Globalization;
using System.Windows.Controls;

namespace PaymentValidator.ValidationRules
{
    public sealed class StringNotEmptyValidationRule : ValidationRule
    {
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is string stringValue && stringValue != string.Empty)
			{
				return ValidationResult.ValidResult;
			}
			return new ValidationResult(false, "This field cannot be empty.");
		}
	}
}
