using PaymentValidator.API.Interfaces;
using System.Text.Json;

namespace PaymentValidator.API.Services.Blacklist
{
    public sealed class JsonBlackListService(ILogger<string> logger, FileInfo file) : IBlacklistService
    {
		private readonly FileInfo _file = file;

		private readonly ILogger<string> _logger = logger;

		private async Task<HashSet<string>> DeserializePayload()
		{
			var payload = new HashSet<string>();
			_file.Refresh();
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				payload = await JsonSerializer.DeserializeAsync<HashSet<string>>(stream) ?? payload;
			}
			return payload;
		}

		public async Task<bool> TryAddBlacklistedUserAsync(string senderName)
		{
			var payload = await DeserializePayload();
			if (payload.Add(senderName))
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
			_file.Refresh();
			if (!_file.Exists)
			{
				return false;
			}

			var payload = await DeserializePayload();
			return payload.Contains(senderName);
		}

		public async IAsyncEnumerable<string> EnumerateBlacklistedUsersAsync()
		{
			_file.Refresh();
			if (_file.Exists)
			{
				await using var stream = _file.OpenRead();
				await foreach (var name in JsonSerializer.DeserializeAsyncEnumerable<string>(stream))
				{
					if (name is not null)
					{
						yield return name;
					}
				}
			}
		}

		public async Task<int> RemoveBlackListUsersAsync(IEnumerable<string> names)
		{
			var payload = await DeserializePayload();

			var removeCount = 0;
			
			foreach (var name in names)
			{
				if (payload.Remove(name))
				{
					++removeCount;
				}
			}
			await using var stream = _file.Open(FileMode.Create);
			await JsonSerializer.SerializeAsync(stream, payload);
			return removeCount;
		}

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
