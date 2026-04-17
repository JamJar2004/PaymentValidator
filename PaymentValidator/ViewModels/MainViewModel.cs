using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentValidator.Interfaces;
using PaymentValidator.Loggers;
using PaymentValidator.Services;
using PaymentValidator.Services.Blacklist;
using PaymentValidator.Services.Payment;
using PaymentValidator.Views;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PaymentValidator.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly MainWindow _view;

		public MainViewModel(MainWindow view)
		{
			_view = view;

			SenderName = string.Empty;
			PaymentAmount = 0.0M;
			IBAN = string.Empty;

			var logger = new MessageBoxLogger(view.Dispatcher);
			BlacklistService = new JsonBlackListService(new FileInfo("C:\\BlackList.json"), logger);
			PaymentService = new JsonPaymentService(BlacklistService, logger, new FileInfo("C:\\Payments.json"));
		}

		public IBlacklistService BlacklistService { get; }

		public PaymentServiceBase PaymentService { get; }

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
			if (await PaymentService.TrySendMoneyAsync(SenderName, PaymentAmount, IBAN))
			{
				MessageBox.Show($"Successfully sent {PaymentAmount} euros, from '{SenderName}', to IBAN number {IBAN}.", "Payment Validator", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		[RelayCommand]
		void BlacklistUser()
		{
			var stringValueDialogViewModel = new StringValueDialogViewModel();

			var stringValueDialog = new StringValueDialogView(stringValueDialogViewModel)
			{
				Title = "Blacklist User",
				Owner = _view
			};

			if (stringValueDialog.ShowDialog() == true)
			{
				BlacklistService.TryAddBlacklistedUserAsync(stringValueDialogViewModel.Text);
			}
		}

		[RelayCommand]
		async Task RemoveFromBlacklist()
		{
			var removeFromBlackListViewModel = new RemoveBlacklistedUsersViewModel(await BlacklistService.GetBlacklistedUsersAsync());
			var removeFromBlackListDialog = new RemoveBlacklistedUsersView(removeFromBlackListViewModel)
			{
				Owner = _view
			};

			if (removeFromBlackListDialog.ShowDialog() == true)
			{
				var names = removeFromBlackListViewModel.Users.Where((user) => !user.IsBlacklisted).Select((user) => user.Name);
				var removedUserCount = await BlacklistService.RemoveBlackListUsersAsync(names);
				MessageBox.Show($"Successfully removed {removedUserCount} users from blacklist.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		[RelayCommand]
		async Task ViewRecentPayments()
		{
			var dialog = new RecentPaymentsView(await PaymentService.GetRecentPaymentsAsync())
			{
				Owner = _view
			};
			dialog.ShowDialog();
		}
	}
}
