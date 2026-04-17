using PaymentValidator.API.Interfaces;
using System.Text.Json;

namespace PaymentValidator.API.Services.Payment
{
	public sealed class JsonPaymentService(IBlacklistService blackListService, ILogger<string> logger, FileInfo file) : PaymentServiceBase(blackListService, logger)
    {
		private readonly FileInfo _file = file;

		private async Task<List<PaymentInfo>> DeserializePayload()
		{
			var payload = new List<PaymentInfo>();
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				payload = await JsonSerializer.DeserializeAsync<List<PaymentInfo>>(stream) ?? payload;
			}
			return payload;
		}

		protected async override Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber, DateTime time)
		{
			var paymentCollection = await DeserializePayload();

			paymentCollection.Add(new PaymentInfo()
			{
				SenderName = senderName,
				AmountInEuros = amountInEuros,
				IBANNumber = ibanNumber,
				PaymentTime = time
			});

			await using var stream = _file.Open(FileMode.Create);
			await JsonSerializer.SerializeAsync(stream, paymentCollection);
			return true;
		}

		public async override IAsyncEnumerable<PaymentInfo> EnumeratePaymentHistoryAsync()
		{
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				await foreach (var payment in JsonSerializer.DeserializeAsyncEnumerable<PaymentInfo>(stream))
				{
					if (payment is not null)
					{
						yield return payment;
					}
				}
			}
		}
	}
}
