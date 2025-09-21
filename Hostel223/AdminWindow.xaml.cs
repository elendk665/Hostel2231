using System.Windows;

namespace Hostel223
{
    public partial class AdminWindow : Window
    {
        private Database.User _currentUser;

        public AdminWindow(Database.User user)
        {
            InitializeComponent();
            _currentUser = user;
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            Title = $"Hostel223 - Администратор: {_currentUser.FullName}";
            UserInfoTextBlock.Text = $"Пользователь: {_currentUser.FullName}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}