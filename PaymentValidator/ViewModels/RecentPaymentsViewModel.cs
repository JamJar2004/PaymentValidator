using CommunityToolkit.Mvvm.ComponentModel;
using PaymentValidator.API;
using System.Collections.ObjectModel;

namespace PaymentValidator.ViewModels
{
    public partial class RecentPaymentsViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial int TotalAccepted { get; set; }

		[ObservableProperty]
		public partial int TotalRejected { get; set; }

		[ObservableProperty]
		public partial int TotalAttempts { get; set; }


		[ObservableProperty]
        public partial ObservableCollection<PaymentInfo> Payments { get; set; }
    }
}
