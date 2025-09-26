using System.Linq;
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
            LoadActiveBookings();
        }

        private void InitializeInterface()
        {
            Title = $"Hostel223 - Менеджер: {_currentUser.FullName}";
            UserInfoTextBlock.Text = $"Пользователь: {_currentUser.FullName}";
        }

        private void LoadActiveBookings()
        {
            var activeBookings = Database.Bookings
                .Where(b => b.Status == "Активно" || b.Status == "Подтверждено")
                .Join(Database.Users,
                    booking => booking.UserId,
                    user => user.Id,
                    (booking, user) => new
                    {
                        RoomNumber = Database.Rooms.First(r => r.Id == booking.RoomId).Number,
                        ClientName = user.FullName,
                        booking.CheckInDate,
                        booking.CheckOutDate,
                        booking.Status
                    })
                .ToList();

            ActiveBookingsList.ItemsSource = activeBookings;
        }

        private void BookingManagementButton_Click(object sender, RoutedEventArgs e)
        {
            BookingManagementWindow bookingWindow = new BookingManagementWindow(_currentUser);
            bookingWindow.ShowDialog();
            LoadActiveBookings();
        }

        private void ViewBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            BookingsViewWindow bookingsWindow = new BookingsViewWindow();
            bookingsWindow.ShowDialog();
            LoadActiveBookings();
        }

        private void ClientsManagementButton_Click(object sender, RoutedEventArgs e)
        {
            ClientsManagementWindow clientsWindow = new ClientsManagementWindow();
            clientsWindow.ShowDialog();
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