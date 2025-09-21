using System.Windows;

namespace Hostel223
{
    public partial class GuestWindow : Window
    {
        private Database.User _currentUser;

        public GuestWindow(Database.User user)
        {
            InitializeComponent();
            _currentUser = user;
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            Title = $"Hostel223 - Гость: {_currentUser.FullName}";
            UserInfoTextBlock.Text = $"Пользователь: {_currentUser.FullName}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}