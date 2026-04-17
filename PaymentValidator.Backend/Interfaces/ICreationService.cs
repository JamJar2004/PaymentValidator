namespace PaymentValidator.API.Interfaces
{
	public interface ICreationService : IDisposable, IAsyncDisposable
	{
		Task<IBlacklistService> CreateBlackListServiceAsync();
		Task<PaymentServiceBase> CreatePaymentServiceAsync();
	}
}
