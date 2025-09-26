using System.Linq;
using System.Windows;

namespace Hostel223
{
    public partial class BookingManagementWindow : Window
    {
        private Database.User _currentManager;

        public BookingManagementWindow(Database.User manager)
        {
            InitializeComponent();
            _currentManager = manager;
            LoadBookings();
        }

        private void LoadBookings()
        {
            var bookings = Database.Bookings
                .Join(Database.Users,
                    booking => booking.UserId,
                    user => user.Id,
                    (booking, user) => new { booking, user })
                .Join(Database.Rooms,
                    combined => combined.booking.RoomId,
                    room => room.Id,
                    (combined, room) => new
                    {
                        combined.booking.Id,
                        ClientName = combined.user.FullName,
                        RoomNumber = room.Number,
                        combined.booking.CheckInDate,
                        combined.booking.CheckOutDate,
                        combined.booking.Status,
                        TotalPrice = (combined.booking.CheckOutDate - combined.booking.CheckInDate).Days * room.Price
                    })
                .OrderByDescending(b => b.CheckInDate)
                .ToList();

            BookingsDataGrid.ItemsSource = bookings;
            StatusTextBlock.Text = $"Загружено бронирований: {bookings.Count}";
        }

        private void CreateBookingButton_Click(object sender, RoutedEventArgs e)
        {
            CreateBookingWindow createWindow = new CreateBookingWindow(_currentManager);
            if (createWindow.ShowDialog() == true)
            {
                LoadBookings();
            }
        }

        private void EditBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooking = BookingsDataGrid.SelectedItem as dynamic;
            if (selectedBooking == null)
            {
                MessageBox.Show("Выберите бронирование для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Открываем окно редактирования с передачей ID бронирования
            EditBookingWindow editWindow = new EditBookingWindow(selectedBooking.Id);
            if (editWindow.ShowDialog() == true)
            {
                LoadBookings();
                MessageBox.Show("Бронирование успешно отредактировано", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CancelBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooking = BookingsDataGrid.SelectedItem as dynamic;
            if (selectedBooking == null)
            {
                MessageBox.Show("Выберите бронирование для отмены", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dbBooking = Database.Bookings.FirstOrDefault(b => b.Id == selectedBooking.Id);

            if (dbBooking != null && MessageBox.Show($"Отменить бронирование №{selectedBooking.Id}?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                dbBooking.Status = "Отменено";

                // Освобождаем номер
                var room = Database.Rooms.FirstOrDefault(r => r.Number == selectedBooking.RoomNumber);
                if (room != null)
                {
                    room.Status = "Свободен";
                }

                LoadBookings();
                MessageBox.Show("Бронирование отменено", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ConfirmBookingButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedBooking = BookingsDataGrid.SelectedItem as dynamic;
            if (selectedBooking == null)
            {
                MessageBox.Show("Выберите бронирование для подтверждения", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dbBooking = Database.Bookings.FirstOrDefault(b => b.Id == selectedBooking.Id);

            if (dbBooking != null)
            {
                dbBooking.Status = "Подтверждено";

                // Занимаем номер
                var room = Database.Rooms.FirstOrDefault(r => r.Number == selectedBooking.RoomNumber);
                if (room != null)
                {
                    room.Status = "Занят";
                }

                LoadBookings();
                MessageBox.Show("Бронирование подтверждено", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookings();
        }
    }
}