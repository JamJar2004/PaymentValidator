using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentValidator.Loggers;
using PaymentValidator.Services;
using PaymentValidator.Services.Blacklist;
using PaymentValidator.Services.Payment;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PaymentValidator.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		public MainViewModel(Dispatcher dispatcher)
		{
			SenderName = string.Empty;
			PaymentAmount = 0.0M;
			IBAN = string.Empty;

			var logger = new MessageBoxLogger(dispatcher);
			var blacklistService = new JsonBlackListService(new FileInfo("C:\\BlackList.json"), logger);
			Service = new JsonPaymentService(blacklistService, logger, new FileInfo("C:\\Payments.json"));
		}

		public PaymentService Service { get; }

		[ObservableProperty]
		public partial string SenderName { get; set; }

		[ObservableProperty]
		public partial decimal PaymentAmount { get; set; }

		[ObservableProperty]
		public partial string IBAN { get; set; }

		partial void OnSenderNameChanged(string oldValue, string newValue)
		{
			SendPaymentCommand.NotifyCanExecuteChanged();
		}

		partial void OnPaymentAmountChanged(decimal oldValue, decimal newValue)
		{
			SendPaymentCommand.NotifyCanExecuteChanged();
		}

		partial void OnIBANChanged(string oldValue, string newValue)
		{
			SendPaymentCommand.NotifyCanExecuteChanged();
		}

		bool CanSendPayment()
		{
			var validationService = new ValidationService();
			return validationService.ValidateSenderName(SenderName, out _) && validationService.ValidateAmountOfMoney(PaymentAmount, out _) && validationService.ValidateIBANNumber(IBAN, out _);
		}

		[RelayCommand(CanExecute = nameof(CanSendPayment))]
		async Task SendPaymentAsync()
		{
			if (await Service.TrySendMoneyAsync(SenderName, PaymentAmount, IBAN))
			{
				MessageBox.Show($"Successfully sent {PaymentAmount} euros, from '{SenderName}', to IBAN number {IBAN}.", "Payment Validator", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show("Failed to send money.", "Payment Validator", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
