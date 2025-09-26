using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hostel223
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Валидация данных
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ShowError("Все поля должны быть заполнены");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            if (password.Length < 4)
            {
                ShowError("Пароль должен содержать минимум 4 символа");
                return;
            }

            // Проверка существующего логина
            if (Database.Users.Any(u => u.Login == login))
            {
                ShowError("Пользователь с таким логином уже существует");
                return;
            }

            // Создание нового пользователя
            Database.User newUser = new Database.User
            {
                Id = Database.Users.Count > 0 ? Database.Users.Max(u => u.Id) + 1 : 1,
                FullName = fullName,
                Login = login,
                Password = password, // В реальном приложении пароль должен хэшироваться
                Role = role
            };

            Database.Users.Add(newUser);

            MessageBox.Show("Регистрация прошла успешно!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Возврат к окну авторизации
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}