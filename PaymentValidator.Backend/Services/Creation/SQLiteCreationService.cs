using PaymentValidator.API.Interfaces;
using PaymentValidator.API.Services.Blacklist;
using PaymentValidator.API.Services.Payment;
using System.Data.SQLite;

namespace PaymentValidator.API.Services.Creation
{
	public sealed class SQLiteCreationService(FileInfo databaseFile, ILogger<string> logger) : ICreationService
	{
		private readonly FileInfo _databaseFile = databaseFile;

		private readonly ILogger<string> _logger = logger;

		private SQLiteConnection? _connection;

		public async Task<IBlacklistService> CreateBlackListServiceAsync()
		{
			if (_connection is null)
			{
				_connection = new($"Data Source={_databaseFile.FullName}");
				await _connection.OpenAsync();
			}

			return new SQLiteBlackListService(_connection);
		}

		public async Task<PaymentServiceBase> CreatePaymentServiceAsync() 
		{
			if (_connection is null)
			{
				_connection = new($"Data Source={_databaseFile.FullName}");
				await _connection.OpenAsync();
			}

			return new SQLitePaymentService(_logger, _connection); 
		}

		public void Dispose()
		{
			_connection?.Dispose();
			GC.SuppressFinalize(this);	
		}

		public async ValueTask DisposeAsync()
		{
			if (_connection is not null)
			{
				await _connection.DisposeAsync();
			}
			GC.SuppressFinalize(this);
		}
	}
}
