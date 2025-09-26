using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hostel223
{
    public partial class UsersManagementWindow : Window
    {
        public UsersManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersDataGrid.ItemsSource = Database.Users.ToList();
            StatusTextBlock.Text = $"Загружено пользователей: {Database.Users.Count}";
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            if (registrationWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as Database.User;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UserEditWindow editWindow = new UserEditWindow(selectedUser);
            if (editWindow.ShowDialog() == true)
            {
                LoadUsers();
                MessageBox.Show("Данные пользователя обновлены", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as Database.User;
            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.FullName}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Database.Users.Remove(selectedUser);
                LoadUsers();
                MessageBox.Show("Пользователь удален", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }
    }
}