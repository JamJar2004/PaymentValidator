using System.Windows;

namespace PaymentValidator.Views
{
    public partial class StringValueDialogView : Window
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(StringValueDialogView));

        public StringValueDialogView()
        {
            InitializeComponent();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to blacklist this user?", string.Empty, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                DialogResult = true;
				Close();
			}
        }
	}
}
