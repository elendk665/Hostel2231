using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hostel223
{
    public partial class UserEditWindow : Window
    {
        private Database.User _user;

        public UserEditWindow(Database.User user)
        {
            InitializeComponent();
            _user = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            FullNameTextBox.Text = _user.FullName;
            LoginTextBox.Text = _user.Login;

            // Устанавливаем выбранную роль
            foreach (ComboBoxItem item in RoleComboBox.Items)
            {
                if (item.Content.ToString() == _user.Role)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверяем, не занят ли логин другим пользователем
            if (Database.Users.Any(u => u.Login == LoginTextBox.Text && u.Id != _user.Id))
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Обновляем данные пользователя
            _user.FullName = FullNameTextBox.Text;
            _user.Login = LoginTextBox.Text;

            // Обновляем пароль только если он введен
            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                _user.Password = PasswordBox.Password;
            }

            _user.Role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}