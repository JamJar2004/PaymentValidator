using PaymentValidator.API.Interfaces;
using System.Text.Json;

namespace PaymentValidator.API.Services.Payment
{
	public sealed class PaymentCollectionPayload
	{
		public required List<PaymentInfo> Payments { get; init; }
	}

	public sealed class JsonPaymentService(IBlacklistService blackListService, ILogger<string> logger, FileInfo file) : PaymentServiceBase(blackListService, logger)
    {
		private readonly FileInfo _file = file;

		private async Task<PaymentCollectionPayload> DeserializePayload()
		{
			var payload = new PaymentCollectionPayload() { Payments = [] };
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				payload = await JsonSerializer.DeserializeAsync<PaymentCollectionPayload>(stream) ?? payload;
			}
			return payload;
		}

		protected async override Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber)
		{
			var paymentCollection = await DeserializePayload();

			paymentCollection.Payments.Add(new PaymentInfo()
			{
				SenderName = senderName,
				AmountInEuros = amountInEuros,
				IBANNumber = ibanNumber
			});

			await using var stream = _file.Open(FileMode.Create);
			await JsonSerializer.SerializeAsync(stream, paymentCollection);
			return true;
		}

		public async override Task<IEnumerable<PaymentInfo>> GetRecentPaymentsAsync()
		{
			var payload = await DeserializePayload();
			return payload.Payments;
		}
	}
}
