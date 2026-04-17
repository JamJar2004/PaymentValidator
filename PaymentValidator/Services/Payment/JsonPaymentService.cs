using PaymentValidator.API;
using PaymentValidator.API.Interfaces;
using System.IO;
using System.Text.Json;

namespace PaymentValidator.Services.Payment
{
	

	public sealed class JsonPaymentService(IBlacklistService blackListService, ILogger<string> logger, FileInfo file) : PaymentServiceBase(blackListService, logger)
    {
		private readonly FileInfo _file = file;

		protected async override Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber)
		{
			var paymentCollection = new PaymentCollectionPayload()
			{
				Payments = []
			};

			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				paymentCollection = await JsonSerializer.DeserializeAsync<PaymentCollectionPayload>(stream) ?? new PaymentCollectionPayload() { Payments = [] };
			}

			paymentCollection.Payments.Add(new PaymentPayload()
			{
				SenderName = senderName,
				AmountInEuros = amountInEuros,
				IBANNumber = ibanNumber
			});

			await using var writeStream = _file.Open(FileMode.Create);
			await JsonSerializer.SerializeAsync(writeStream, paymentCollection);
			return true;
		}

		public async override Task<PaymentCollectionPayload> GetRecentPaymentsAsync()
		{
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				return await JsonSerializer.DeserializeAsync<PaymentCollectionPayload>(stream) ?? new PaymentCollectionPayload() { Payments = [] };
			}
			return new PaymentCollectionPayload() { Payments = [] };
		}
	}
}
