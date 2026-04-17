using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace PaymentValidator.ViewModels
{
	public partial class UserViewModel(RemoveBlacklistedUsersViewModel parent) : ObservableObject
	{
		private readonly RemoveBlacklistedUsersViewModel _parent = parent;

		[ObservableProperty]
		public partial string Name { get; set; }

		[ObservableProperty]
		public partial bool IsBlacklisted { get; set; }

		partial void OnIsBlacklistedChanged(bool oldValue, bool newValue)
		{
			_parent.OkCommand.NotifyCanExecuteChanged();
		}
	}

    public partial class RemoveBlacklistedUsersViewModel : ObservableObject
    {
		public RemoveBlacklistedUsersViewModel(IEnumerable<string> userNames)
		{
			Users = new(userNames.Select((userName) => new UserViewModel(this)
			{
				Name = userName,
				IsBlacklisted = true,
			}));
		}

		[ObservableProperty]
		public partial ObservableCollection<UserViewModel> Users { get; set; }

		bool CanExecuteOk() => Users.Any((user) => !user.IsBlacklisted);

		[RelayCommand(CanExecute = nameof(CanExecuteOk))]
		void Ok(Window window)
		{
			window.DialogResult = true;
			window.Close();
		}
	}
}
