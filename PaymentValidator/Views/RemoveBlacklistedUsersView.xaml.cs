using PaymentValidator.ViewModels;
using System.Windows;

namespace PaymentValidator.Views
{
    /// <summary>
    /// Interaction logic for RemoveBlacklistedUsersView.xaml
    /// </summary>
    public partial class RemoveBlacklistedUsersView : Window
    {
        public RemoveBlacklistedUsersView(RemoveBlacklistedUsersViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
