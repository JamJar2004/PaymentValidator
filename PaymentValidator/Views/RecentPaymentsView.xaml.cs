using PaymentValidator.API;
using PaymentValidator.ViewModels;
using System.Windows;

namespace PaymentValidator.Views
{
    public partial class RecentPaymentsView : Window
    {
        public RecentPaymentsView(IEnumerable<PaymentInfo> payments)
        {
            InitializeComponent();

            DataContext = new RecentPaymentsViewModel()
            {
                Payments = new(payments)
			};
        }
    }
}
