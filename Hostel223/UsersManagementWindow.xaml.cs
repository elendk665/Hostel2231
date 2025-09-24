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
            // Открываем окно регистрации для добавления нового пользователя
            RegistrationWindow registrationWindow = new RegistrationWindow();
            if (registrationWindow.ShowDialog() == true)
            {
                LoadUsers(); // Обновляем список после добавления
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

            // Здесь можно создать окно редактирования пользователя
            MessageBox.Show($"Редактирование пользователя: {selectedUser.FullName}",
                "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
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