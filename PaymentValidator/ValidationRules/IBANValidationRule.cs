using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace PaymentValidator.ValidationRules
{
	public sealed class IBANValidationRule : ValidationRule
	{
		private readonly HashSet<string> _validCountries =
		[
			"AL", "AD", "AT", "AZ", "BH", "BY", "BE", "BA", "BR", "BG", "CR", "HR", "CY", "DK", "DO", "SV", "EE", "FO", "FI", "FR", "GE", "DE", "GI", "GR", "GL",
			"GT", "HU", "IS", "IQ", "IE", "IL", "IT", "JO", "KZ", "XK", "KW", "LV", "LB", "LI", "LT", "LU", "MK", "MT", "MR", "MU", "MD", "MC", "ME", "NL", "NO",
			"PK", "PS", "PL", "PT", "QA", "RO", "LC", "SM", "ST", "SA", "RS", "SC", "SK", "SI", "ES", "SE", "CH", "TL", "TN", "TR", "UA", "AE", "GB", "VA", "VG",
		];

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			if (value is not string stringValue)
			{
				return new ValidationResult(false, "IBAN must be a string value.");
			}

			if (stringValue.Any((character) => !char.IsAsciiLetterOrDigit(character)))
			{
				return new ValidationResult(false, "IBAN can only contain letters or numbers.");
			}

			if (stringValue.Length != 22)
			{
				return new ValidationResult(false, "IBAN number must have 22 characters.");
			}

			var country = stringValue[..2];
			if (!_validCountries.Contains(country))
			{
				return new ValidationResult(false, $"IBAN contains invalid country '{country}'.");
			}

			return ValidationResult.ValidResult;
		}
	}
}
