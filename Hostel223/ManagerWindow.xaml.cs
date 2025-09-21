using System.Windows;

namespace Hostel223
{
    public partial class ManagerWindow : Window
    {
        private Database.User _currentUser;

        public ManagerWindow(Database.User user)
        {
            InitializeComponent();
            _currentUser = user;
            InitializeInterface();
        }

        private void InitializeInterface()
        {
            Title = $"Hostel223 - Менеджер: {_currentUser.FullName}";
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