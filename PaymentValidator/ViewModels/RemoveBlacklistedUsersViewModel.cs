using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PaymentValidator.Services.Blacklist;
using System.Collections.ObjectModel;
using System.Windows;

namespace PaymentValidator.ViewModels
{
	public partial class UserViewModel : ObservableObject
	{
		[ObservableProperty]
		public partial string Name { get; set; }

		[ObservableProperty]
		public partial bool IsBlacklisted { get; set; }
	}

    public partial class RemoveBlacklistedUsersViewModel : ObservableObject
    {
		public RemoveBlacklistedUsersViewModel(IEnumerable<string> userNames)
		{
			Users = new(userNames.Select((userName) => new UserViewModel()
			{
				Name = userName,
				IsBlacklisted = true,
			}));
		}

		[ObservableProperty]
		public partial ObservableCollection<UserViewModel> Users { get; set; }

		[RelayCommand]
		void Ok(Window window)
		{
			window.DialogResult = true;
			window.Close();
		}
	}
}
