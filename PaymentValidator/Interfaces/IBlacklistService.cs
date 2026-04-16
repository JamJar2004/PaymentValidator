namespace PaymentValidator.Interfaces
{
    public interface IBlacklistService
    {
        Task<bool> TryAddBlacklistedUserAsync(string senderName);

        Task<bool> IsUserBlackListedAsync(string senderName);
    }
}
