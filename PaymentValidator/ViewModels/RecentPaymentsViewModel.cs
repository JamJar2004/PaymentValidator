using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentValidator.Services.Payment;

namespace PaymentValidator.ViewModels
{
    public partial class RecentPaymentsViewModel : ObservableObject
    {
		[ObservableProperty]
        public partial PaymentCollectionPayload Payments { get; set; }
        
    }
}
