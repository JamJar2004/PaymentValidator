using PaymentValidator.API.Interfaces;
using PaymentValidator.API.Services.Blacklist;
using PaymentValidator.API.Services.Payment;

namespace PaymentValidator.API.Services.Creation
{
	public sealed class JsonCreationService(FileInfo blackListFile, FileInfo paymentHistoryFile, ILogger<string> logger) : ICreationService
	{
		private readonly FileInfo _blackListFile = blackListFile;

		private readonly FileInfo _paymentHistoryFile = paymentHistoryFile;

		private readonly ILogger<string> _logger = logger;

		public async Task<IBlacklistService> CreateBlackListServiceAsync() => new JsonBlackListService(_logger, _blackListFile);

		public async Task<PaymentServiceBase> CreatePaymentServiceAsync() => new JsonPaymentService(_logger, _paymentHistoryFile);

		public void Dispose() 
		{
			GC.SuppressFinalize(this);
		}

		public async ValueTask DisposeAsync()
		{
			GC.SuppressFinalize(this);
		}
	}
}
