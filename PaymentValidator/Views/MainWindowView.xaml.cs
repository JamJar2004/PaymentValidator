using PaymentValidator.ViewModels;
using System.Windows;

namespace PaymentValidator.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			DataContext = new MainViewModel(this);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			var viewModel = (MainViewModel)DataContext;
			viewModel.Dispose();
		}
    }
}