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

        private void UsersManagementButton_Click(object sender, RoutedEventArgs e)
        {
            UsersManagementWindow usersWindow = new UsersManagementWindow();
            usersWindow.ShowDialog();
        }

        private void RoomsManagementButton_Click(object sender, RoutedEventArgs e)
        {
            RoomsManagementWindow roomsWindow = new RoomsManagementWindow();
            roomsWindow.ShowDialog();
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindow statsWindow = new StatisticsWindow();
            statsWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}