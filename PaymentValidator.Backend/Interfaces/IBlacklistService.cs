namespace PaymentValidator.API.Interfaces
{
    public interface IBlacklistService
    {
        Task<bool> TryAddBlacklistedUserAsync(string name);

        Task<bool> IsUserBlackListedAsync(string name);

        IAsyncEnumerable<string> EnumerateBlacklistedUsersAsync();

        Task<int> RemoveBlackListUsersAsync(IEnumerable<string> names);
    }
}
