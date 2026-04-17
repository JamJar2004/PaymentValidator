using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace PaymentValidator.ViewModels
{
    public partial class StringValueDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Text { get; set; }

        [RelayCommand]
        void Ok(Window window)
        {
			if (MessageBox.Show("Are you sure you want to blacklist this user?", string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
			{
				window.DialogResult = true;
				window.Close();
			}
		}
    }
}
