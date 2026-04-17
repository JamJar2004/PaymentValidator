using PaymentValidator.ViewModels;
using System.Windows;

namespace PaymentValidator.Views
{
    public partial class StringValueDialogView : Window
    {
        public StringValueDialogView(StringValueDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
	}
}
