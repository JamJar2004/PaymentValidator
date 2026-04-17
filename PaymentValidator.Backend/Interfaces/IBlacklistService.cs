namespace PaymentValidator.API.Interfaces
{
    public interface IBlacklistService : IDisposable, IAsyncDisposable
    {
        Task<bool> TryAddBlacklistedUserAsync(string name);

        Task<bool> IsUserBlackListedAsync(string name);

        IAsyncEnumerable<string> EnumerateBlacklistedUsersAsync();

        Task<int> RemoveBlackListUsersAsync(IEnumerable<string> names);
    }
}
