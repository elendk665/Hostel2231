using System.Linq;
using System.Windows;

namespace Hostel223
{
    public partial class ClientHistoryWindow : Window
    {
        private int _clientId;

        public ClientHistoryWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadClientHistory();
        }

        private void LoadClientHistory()
        {
            var client = Database.Users.FirstOrDefault(u => u.Id == _clientId);
            if (client != null)
            {
                TitleText.Text = $"История бронирований: {client.FullName}";
            }

            var clientBookings = Database.Bookings
                .Where(b => b.UserId == _clientId)
                .Join(Database.Rooms,
                    booking => booking.RoomId,
                    room => room.Id,
                    (booking, room) => new
                    {
                        booking.Id,
                        RoomNumber = room.Number,
                        RoomType = room.Type,
                        booking.CheckInDate,
                        booking.CheckOutDate,
                        Nights = (booking.CheckOutDate - booking.CheckInDate).Days,
                        TotalPrice = (booking.CheckOutDate - booking.CheckInDate).Days * room.Price,
                        booking.Status
                    })
                .OrderByDescending(b => b.CheckInDate)
                .ToList();

            ClientBookingsDataGrid.ItemsSource = clientBookings;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}