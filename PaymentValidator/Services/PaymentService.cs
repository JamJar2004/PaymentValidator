using PaymentValidator.Interfaces;

namespace PaymentValidator.Services
{
    public abstract class PaymentService(IBlacklistService blackListService, ValidationService validationService, ILogger<string> logger)
    {
        private readonly IBlacklistService _blackListService = blackListService;
		private readonly ValidationService _validationService = validationService;

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
                await Logger.LogAsync($"User of name '{senderName}' is blacklisted.");
                return false;
            }

            return await TrySendMoneyAsyncCore(senderName, amountInEuros, ibanNumber);
        }

        protected abstract Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber);
    }
}
