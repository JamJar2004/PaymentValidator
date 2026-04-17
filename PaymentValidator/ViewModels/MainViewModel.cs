using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentValidator.API;
using PaymentValidator.API.Interfaces;
using PaymentValidator.API.Services.Creation;
using PaymentValidator.Loggers;
using PaymentValidator.Views;
using System.IO;
using System.Windows;

namespace PaymentValidator.ViewModels
{
	public partial class MainViewModel : ObservableObject, IDisposable, IAsyncDisposable
	{
		private readonly MainWindow _view;

		private readonly ICreationService _creationService;

		public MainViewModel(MainWindow view)
		{
			_view = view;

			var logger = new MessageBoxLogger(view.Dispatcher);

			_creationService = new JsonCreationService(new FileInfo("C:\\BlackList.json"), new FileInfo("C:\\Payments.json"), logger);

			SenderName = string.Empty;
			PaymentAmount = 0.0M;
			IBAN = string.Empty;
		}

		~MainViewModel()
		{
			Dispose();
		}

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
			await using var paymentService = await _creationService.CreatePaymentServiceAsync();
			await using var blackListService = await _creationService.CreateBlackListServiceAsync();
			if (await paymentService.TrySendMoneyAsync(blackListService, SenderName, PaymentAmount, IBAN))
			{
				MessageBox.Show($"Successfully sent {PaymentAmount} euros, from '{SenderName}', to IBAN number {IBAN}.", "Payment Validator", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		[RelayCommand]
		async Task BlacklistUser()
		{
			var stringValueDialogViewModel = new StringValueDialogViewModel();

			var stringValueDialog = new StringValueDialogView(stringValueDialogViewModel)
			{
				Title = "Blacklist User",
				Owner = _view
			};

			if (stringValueDialog.ShowDialog() == true)
			{
				await using var blacklistService = await _creationService.CreateBlackListServiceAsync();
				await blacklistService.TryAddBlacklistedUserAsync(stringValueDialogViewModel.Text);
			}
		}

		[RelayCommand]
		async Task RemoveFromBlacklist()
		{
			await using var blacklistService = await _creationService.CreateBlackListServiceAsync();

			var blacklistedUsers = new List<string>(); 
			await foreach (var user in blacklistService.EnumerateBlacklistedUsersAsync())
			{
				blacklistedUsers.Add(user);
			}

			var removeFromBlackListViewModel = new RemoveBlacklistedUsersViewModel(blacklistedUsers);
			var removeFromBlackListDialog = new RemoveBlacklistedUsersView(removeFromBlackListViewModel)
			{
				Owner = _view
			};

			if (removeFromBlackListDialog.ShowDialog() == true)
			{
				var names = removeFromBlackListViewModel.Users.Where((user) => !user.IsBlacklisted).Select((user) => user.Name);
				var removedUserCount = await blacklistService.RemoveBlackListUsersAsync(names);
				MessageBox.Show($"Successfully removed {removedUserCount} users from blacklist.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		[RelayCommand]
		async Task ViewRecentPayments()
		{
			await using var paymentService = await _creationService.CreatePaymentServiceAsync();

			var payments = new List<PaymentInfo>();
			await foreach (var payment in paymentService.EnumeratePaymentHistoryAsync())
			{
				payments.Add(payment);
			}

			var dialog = new RecentPaymentsView(payments)
			{
				Owner = _view
			};
			dialog.ShowDialog();
		}

		public void Dispose()
		{
			_creationService.Dispose();
			GC.SuppressFinalize(this);
		}

		public async ValueTask DisposeAsync()
		{
			await _creationService.DisposeAsync().ConfigureAwait(false);
			GC.SuppressFinalize(this);
		}
	}
}
