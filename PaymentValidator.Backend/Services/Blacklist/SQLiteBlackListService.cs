using PaymentValidator.API.Interfaces;
using System.Data.SQLite;

namespace PaymentValidator.API.Services.Blacklist
{
	public sealed class SQLiteBlackListService(SQLiteConnection connection) : IBlacklistService
	{
		private const string TABLE_NAME = "blacklisted";

		private readonly SQLiteConnection _connection = connection;
		private readonly SQLiteCommand _ensureTableCommand = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {TABLE_NAME} (name TEXT);", connection);

		public async Task<bool> TryAddBlacklistedUserAsync(string name)
		{
			await _ensureTableCommand.ExecuteNonQueryAsync();

			await using var insertCommand = new SQLiteCommand($"INSERT INTO {TABLE_NAME} (name) VALUES (@name);", _connection);
			insertCommand.Parameters.AddWithValue("@name", name);

			return await insertCommand.ExecuteNonQueryAsync() != 0;
		}

		public async Task<bool> IsUserBlackListedAsync(string name)
		{
			await _ensureTableCommand.ExecuteNonQueryAsync();

			await using var containsCommand = new SQLiteCommand($"SELECT EXISTS(SELECT 1 FROM {TABLE_NAME} WHERE name = @name);", _connection);
			containsCommand.Parameters.AddWithValue("@name", name);

			return Convert.ToBoolean(await containsCommand.ExecuteScalarAsync());
		}

		public async IAsyncEnumerable<string> EnumerateBlacklistedUsersAsync()
		{
			await _ensureTableCommand.ExecuteNonQueryAsync();

			var selectCommand = new SQLiteCommand($"SELECT name FROM {TABLE_NAME};", _connection);

			var reader = await selectCommand.ExecuteReaderAsync();

			while (reader.Read())
			{
				yield return reader.GetString(0);
			}
		}

		public async Task<int> RemoveBlackListUsersAsync(IEnumerable<string> names)
		{
			await _ensureTableCommand.ExecuteNonQueryAsync();

			var result = 0;
			foreach (var name in names)
			{
				var containsCommand = new SQLiteCommand($"DELETE FROM {TABLE_NAME} WHERE name = @name;", _connection);
				containsCommand.Parameters.AddWithValue("@name", name);
				result += await containsCommand.ExecuteNonQueryAsync();
			}

			return result;
		}

		public void Dispose()
		{
			_ensureTableCommand.Dispose();
			GC.SuppressFinalize(this);
		}

		public async ValueTask DisposeAsync()
		{
			await _ensureTableCommand.DisposeAsync();
			GC.SuppressFinalize(this);
		}
	}
}
