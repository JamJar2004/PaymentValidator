using PaymentValidator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace PaymentValidator.Services.Blacklist
{
	public sealed class BlackListPayload
	{
		public required HashSet<string> BlacklistedUsers { get; init; }
	}

    public sealed class JsonBlackListService(Stream stream, ILogger<string> logger) : IBlacklistService
    {
		private readonly Stream _stream = stream;

		private readonly ILogger<string> _logger = logger;

		public async Task<bool> TryAddBlacklistedUserAsync(string senderName)
		{
			var payload = await JsonSerializer.DeserializeAsync<BlackListPayload>(_stream) ?? new BlackListPayload() { BlacklistedUsers = [] };
			if (payload.BlacklistedUsers.Add(senderName))
			{
				await JsonSerializer.SerializeAsync(_stream, payload);
				return true;
			}
			await _logger.LogAsync($"Sender of name '{senderName}' has already been blacklisted.");
			return false;
		}

		public async Task<bool> IsUserBlackListedAsync(string senderName)
		{
			var payload = await JsonSerializer.DeserializeAsync<BlackListPayload>(_stream) ?? new BlackListPayload() { BlacklistedUsers = [] };
			return payload.BlacklistedUsers.Contains(senderName);
		}
	}
}
