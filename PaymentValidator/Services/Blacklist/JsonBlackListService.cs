using PaymentValidator.Interfaces;
using System.IO;
using System.Text.Json;

namespace PaymentValidator.Services.Blacklist
{
	public sealed class BlackListPayload
	{
		public required HashSet<string> BlacklistedUsers { get; init; }
	}

    public sealed class JsonBlackListService(FileInfo file, ILogger<string> logger) : IBlacklistService
    {
		private readonly FileInfo _file = file;

		private readonly ILogger<string> _logger = logger;

		private async Task<BlackListPayload> DeserializePayload()
		{
			var payload = new BlackListPayload() { BlacklistedUsers = [] };
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				payload = await JsonSerializer.DeserializeAsync<BlackListPayload>(stream) ?? new BlackListPayload() { BlacklistedUsers = [] };
			}
			return payload;
		}

		public async Task<bool> TryAddBlacklistedUserAsync(string senderName)
		{
			var payload = await DeserializePayload();
			if (payload.BlacklistedUsers.Add(senderName))
			{
				await using var stream = _file.Open(FileMode.Create);
				await JsonSerializer.SerializeAsync(stream, payload);
				return true;
			}
			await _logger.LogAsync($"Sender of name '{senderName}' has already been blacklisted.");
			return false;
		}

		public async Task<bool> IsUserBlackListedAsync(string senderName)
		{
			if (!_file.Exists)
			{
				return false;
			}

			var payload = await DeserializePayload();
			return payload.BlacklistedUsers.Contains(senderName);
		}

		public async Task<IEnumerable<string>> GetBlacklistedUsersAsync()
		{
			var payload = await DeserializePayload();
			return payload.BlacklistedUsers;
		}

		public async Task<int> RemoveBlackListUsersAsync(IEnumerable<string> names)
		{
			var payload = await DeserializePayload();

			var removeCount = 0;
			
			foreach (var name in names)
			{
				if (payload.BlacklistedUsers.Remove(name))
				{
					++removeCount;
				}
			}
			await using var stream = _file.Open(FileMode.Create);
			await JsonSerializer.SerializeAsync(stream, payload);
			return removeCount;
		}
	}
}
