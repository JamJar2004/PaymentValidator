using PaymentValidator.API.Interfaces;

namespace PaymentValidator.API
{
	public sealed class PaymentInfo
	{
		public required string SenderName { get; init; }

		public required decimal AmountInEuros { get; init; }

		public required string IBANNumber { get; init; }

		public required DateTime PaymentTime { get; init; }
	}

	public abstract class PaymentServiceBase(IBlacklistService blackListService, ILogger<string> logger)
    {
        private readonly IBlacklistService _blackListService = blackListService;
		private readonly ValidationService _validationService = new();

		public ILogger<string> Logger { get; } = logger;

		public async Task<bool> TrySendMoneyAsync(string senderName, decimal amountInEuros, string ibanNumber)
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

			if (await _blackListService.IsUserBlackListedAsync(senderName))
            {
                await Logger.LogAsync($"User of name '{senderName}' is blacklisted. The payment has been rejected.");
                return false;
            }

            return await TrySendMoneyAsyncCore(senderName, amountInEuros, ibanNumber, DateTime.Now);
        }

        protected abstract Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber, DateTime dateTime);

		public abstract IAsyncEnumerable<PaymentInfo> EnumeratePaymentHistoryAsync();
    }
}
