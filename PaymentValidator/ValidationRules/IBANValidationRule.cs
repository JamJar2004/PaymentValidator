using PaymentValidator.API;
using System.Globalization;
using System.Windows.Controls;

namespace PaymentValidator.ValidationRules
{
	public sealed class IBANValidationRule : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is not string stringValue)
			{
				return new ValidationResult(false, "IBAN must be a string value.");
			}

			var validationService = new ValidationService();

			if (!validationService.ValidateIBANNumber(stringValue, out var message))
			{
				return new ValidationResult(false, message);
			}

			return ValidationResult.ValidResult;
		}
	}
}
