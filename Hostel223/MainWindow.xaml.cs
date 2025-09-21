using System.Windows;

namespace Hostel223
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        }

        public MainWindow(Database.User user)
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = Database.Authenticate(login, password);

            if (user != null)
            {
                OpenRoleWindow(user);
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRoleWindow(Database.User user)
        {
            Window window = null;

            switch (user.Role)
            {
                case "Администратор":
                    window = new AdminWindow(user);
                    break;
                case "Менеджер":
                    window = new ManagerWindow(user);
                    break;
                case "Гость":
                    window = new GuestWindow(user);
                    break;
            }

            if (window != null)
            {
                window.Show();
                this.Close();
            }
        }
    }
}