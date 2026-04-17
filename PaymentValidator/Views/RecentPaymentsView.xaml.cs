using PaymentValidator.API;
using PaymentValidator.ViewModels;
using System.Windows;

namespace PaymentValidator.Views
{
    public partial class RecentPaymentsView : Window
    {
        public RecentPaymentsView(PaymentCollectionPayload paymentCollection)
        {
            InitializeComponent();

            DataContext = new RecentPaymentsViewModel()
            {
                Payments = paymentCollection
            };
        }
    }
}
