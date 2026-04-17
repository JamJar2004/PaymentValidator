using PaymentValidator.API;
using PaymentValidator.ViewModels;
using System.Windows;

namespace PaymentValidator.Views
{
    public partial class RecentPaymentsView : Window
    {
        public RecentPaymentsView(IReadOnlyCollection<PaymentInfo> payments)
        {
            InitializeComponent();

            DataContext = new RecentPaymentsViewModel()
            {
                Payments = new(payments),
                TotalAttempts = payments.Count,
                TotalAccepted = payments.Count((payment) => payment.Result == PaymentResult.Accepted),
				TotalRejected = payments.Count((payment) => payment.Result == PaymentResult.Rejected)
			};
        }
    }
}
