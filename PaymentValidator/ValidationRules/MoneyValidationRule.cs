using System.Globalization;
using System.Windows.Controls;

namespace PaymentValidator.ValidationRules
{
	public sealed class MoneyValidationRule : ValidationRule
    {
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is not string stringValue || !decimal.TryParse(stringValue, out var decimalValue))
			{
				return new ValidationResult(false, "Value must be a number.");
			}

			if (decimalValue <= 0.0M)
			{
				return new ValidationResult(false, "Value must be greater than zero.");
			}

			return ValidationResult.ValidResult;
		}
	}
}
