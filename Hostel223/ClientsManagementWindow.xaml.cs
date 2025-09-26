using System.Linq;
using System.Windows;

namespace Hostel223
{
    public partial class ClientsManagementWindow : Window
    {
        public ClientsManagementWindow()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            var clients = Database.Users
                .Where(u => u.Role == "Гость")
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Login,
                    Phone = "Не указан",
                    TotalBookings = Database.Bookings.Count(b => b.UserId == user.Id)
                })
                .ToList();

            ClientsDataGrid.ItemsSource = clients;
        }

        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            if (regWindow.ShowDialog() == true)
            {
                LoadClients();
            }
        }

        private void ViewClientHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsDataGrid.SelectedItem as dynamic;
            if (selectedClient == null)
            {
                MessageBox.Show("Выберите клиента", "Внимание");
                return;
            }

            // Создаем и показываем окно истории клиента
            ClientHistoryWindow historyWindow = new ClientHistoryWindow(selectedClient.Id);
            historyWindow.ShowDialog();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }
    }
}