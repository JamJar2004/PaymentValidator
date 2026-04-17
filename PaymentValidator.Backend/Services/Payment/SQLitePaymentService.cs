using PaymentValidator.API.Interfaces;
using System.Data.SQLite;

namespace PaymentValidator.API.Services.Payment
{
	public sealed class SQLitePaymentService(ILogger<string> logger, SQLiteConnection connection) : PaymentServiceBase(logger)
	{
		private const string TABLE_NAME = "paymentHistory";

		private readonly SQLiteConnection _connection = connection;

		private readonly SQLiteCommand _ensureTableCommand = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {TABLE_NAME} (senderName TEXT, amountInEuros DECIMAL(32, 16), ibanNumber TEXT, paymentTime DATETIME);", connection);

		protected async override Task<bool> TrySendMoneyAsyncCore(string senderName, decimal amountInEuros, string ibanNumber, DateTime time)
		{
			await _ensureTableCommand.ExecuteNonQueryAsync();

			await using var insertCommand = new SQLiteCommand($"INSERT INTO {TABLE_NAME} (senderName, amountInEuros, ibanNumber, paymentTime) VALUES (@senderName, @amountInEuros, @ibanNumber, @paymentTime);", _connection);
			insertCommand.Parameters.AddWithValue("@senderName", senderName);
			insertCommand.Parameters.AddWithValue("@amountInEuros", amountInEuros);
			insertCommand.Parameters.AddWithValue("@ibanNumber", ibanNumber);
			insertCommand.Parameters.AddWithValue("@paymentTime", time);

			return await insertCommand.ExecuteNonQueryAsync() != 0;
		}

		public async override IAsyncEnumerable<PaymentInfo> EnumeratePaymentHistoryAsync()
		{
			await _ensureTableCommand.ExecuteNonQueryAsync();

			var selectCommand = new SQLiteCommand($"SELECT * FROM {TABLE_NAME};", _connection);

			var reader = await selectCommand.ExecuteReaderAsync();

			while (reader.Read())
			{
				yield return new PaymentInfo()
				{
					SenderName = reader.GetString(0),
					AmountInEuros = reader.GetDecimal(1),
					IBANNumber = reader.GetString(2),
					PaymentTime = reader.GetDateTime(3),
				};
			}
		}
	}
}
