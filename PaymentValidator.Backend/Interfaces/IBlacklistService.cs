namespace PaymentValidator.API.Interfaces
{
    public interface IBlacklistService
    {
        Task<bool> TryAddBlacklistedUserAsync(string name);

        Task<bool> IsUserBlackListedAsync(string name);

        Task<IEnumerable<string>> GetBlacklistedUsersAsync();

        Task<int> RemoveBlackListUsersAsync(IEnumerable<string> names);
    }
}
