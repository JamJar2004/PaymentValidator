using System.ComponentModel;

namespace PaymentValidator.ViewModels
{
	public class MainViewModel : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public MainViewModel()
		{
			SenderName = string.Empty;
			PaymentAmount = 0.0M;
			IBAN = string.Empty;
		}

		public event PropertyChangingEventHandler? PropertyChanging;
		public event PropertyChangedEventHandler? PropertyChanged;

		public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

		public string SenderName
		{
			get => field;
			set
			{
				if (field != value)
				{
					PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(SenderName)));
					field = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SenderName)));
				}
			}
		}

		public decimal PaymentAmount
		{
			get => field;
			set
			{
				if (field != value)
				{
					PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(PaymentAmount)));
					field = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentAmount)));
				}
			}
		}

		public string IBAN
		{
			get => field;
			set
			{
				if (field != value)
				{
					PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(IBAN)));
					field = value.ToUpper();
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IBAN)));
				}
			}
		}
	}
}
