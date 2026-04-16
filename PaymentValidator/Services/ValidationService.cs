using System.Diagnostics.CodeAnalysis;

namespace PaymentValidator.Services
{
    public sealed class ValidationService
    {
		private readonly HashSet<string> _validCountries =
		[
			"AL", "AD", "AT", "AZ", "BH", "BY", "BE", "BA", "BR", "BG", "CR", "HR", "CY", "DK", "DO", "SV", "EE", "FO", "FI", "FR", "GE", "DE", "GI", "GR", "GL",
			"GT", "HU", "IS", "IQ", "IE", "IL", "IT", "JO", "KZ", "XK", "KW", "LV", "LB", "LI", "LT", "LU", "MK", "MT", "MR", "MU", "MD", "MC", "ME", "NL", "NO",
			"PK", "PS", "PL", "PT", "QA", "RO", "LC", "SM", "ST", "SA", "RS", "SC", "SK", "SI", "ES", "SE", "CH", "TL", "TN", "TR", "UA", "AE", "GB", "VA", "VG",
		];

		public bool ValidateSenderName(string senderName, [NotNullWhen(false)] out string? message)
		{
			if (senderName == string.Empty)
			{
				message = "Sender name cannot be empty.";
				return false;
			}
			message = null;
			return true;
		}

		public bool ValidateAmountOfMoney(decimal amountInEuros, [NotNullWhen(false)] out string? message)
		{
			if (amountInEuros <= 0.0M)
			{
				message = "Amount of money must be greater than zero.";
				return false;
			}
			message = null;
			return true;
		}

		public bool ValidateIBANNumber(string ibanNumber, [NotNullWhen(false)] out string? message)
		{
			if (ibanNumber.Any((character) => !char.IsAsciiLetterOrDigit(character)))
			{
				message = "IBAN can only contain letters or numbers.";
				return false;
			}

			if (ibanNumber.Length != 22)
			{
				message = "IBAN number must have 22 characters.";
				return false;
			}

			var country = ibanNumber[..2];
			if (!_validCountries.Contains(country))
			{
				message = $"IBAN contains invalid country '{country}'.";
				return false;
			}

			message = null;
			return true;
		}
	}
}
