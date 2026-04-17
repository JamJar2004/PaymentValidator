using PaymentValidator.API.Interfaces;
using System.Windows;
using System.Windows.Threading;

namespace PaymentValidator.Loggers
{
    public sealed class MessageBoxLogger(Dispatcher dispatcher) : ILogger<string>
    {
		private readonly Dispatcher _dispatcher = dispatcher;

		public async Task LogAsync(string message)
		{
			await _dispatcher.InvokeAsync(() =>
			{
				MessageBox.Show(message, "Payment Validator");
			});
		}
	}
}
