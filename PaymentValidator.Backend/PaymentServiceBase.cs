using PaymentValidator.API.Interfaces;

namespace PaymentValidator.API
{
	public enum PaymentResult
	{
		Accepted,
		Rejected,
	}

	public sealed class PaymentInfo
	{
		public required string SenderName { get; init; }

		public required decimal AmountInEuros { get; init; }

		public required string IBANNumber { get; init; }

		public required DateTime PaymentTime { get; init; }

		public required PaymentResult Result { get; init; }
	}

	public abstract class PaymentServiceBase(ILogger<string> logger) : IDisposable, IAsyncDisposable
	{
		private readonly ValidationService _validationService = new();

		public ILogger<string> Logger { get; } = logger;

		public async Task<bool> TrySendMoneyAsync(IBlacklistService blacklist, string senderName, decimal amountInEuros, string ibanNumber)
		{
			if (!_validationService.ValidateSenderName(senderName, out var senderMessage))
			{
				await Logger.LogAsync(senderMessage);
				return false;
			}

			if (!_validationService.ValidateAmountOfMoney(amountInEuros, out var amountOfMoneyMessage))
			{
				await Logger.LogAsync(amountOfMoneyMessage);
				return false;
			}

			if (!_validationService.ValidateIBANNumber(ibanNumber, out var ibanMessage))
			{
				await Logger.LogAsync(ibanMessage);
				return false;
			}

			var paymentResult = PaymentResult.Accepted;
			if (await blacklist.IsUserBlackListedAsync(senderName))
			{
				await Logger.LogAsync($"User of name '{senderName}' is blacklisted. The payment has been rejected.");
				paymentResult = PaymentResult.Rejected;
			}

			return await TrySendMoneyAsyncCore(senderName, amountInEuros, ibanNumber, DateTime.Now, paymentResult) && paymentResult == PaymentResult.Accepted;
		}

		protected abstract Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber, DateTime dateTime, PaymentResult paymentResult);

		public abstract IAsyncEnumerable<PaymentInfo> EnumeratePaymentHistoryAsync();

		public virtual void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public virtual async ValueTask DisposeAsync() 
		{
			GC.SuppressFinalize(this);
		}
	}
}
