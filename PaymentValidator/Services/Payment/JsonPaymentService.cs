using PaymentValidator.Interfaces;
using PaymentValidator.Services.Blacklist;
using System.IO;
using System.Text.Json;

namespace PaymentValidator.Services.Payment
{
	public sealed class PaymentPayload
	{
		public required string SenderName { get; init; }

		public required decimal AmountInEuros { get; init; }

		public required string IBANNumber { get; init; }
	}

	public sealed class PaymentCollectionPayload
	{
		public required List<PaymentPayload> Payments { get; init; }
	}

	public sealed class JsonPaymentService(IBlacklistService blackListService, ILogger<string> logger, FileInfo file) : PaymentService(blackListService, logger)
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
	}
}
