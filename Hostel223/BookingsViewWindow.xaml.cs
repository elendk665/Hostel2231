using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hostel223
{
    public partial class BookingsViewWindow : Window
    {
        public BookingsViewWindow()
        {
            try
            {
                InitializeComponent();

                // Ждем полной инициализации интерфейса
                this.Loaded += (s, e) => LoadAllBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке окна: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllBookings(string statusFilter = null)
        {
            try
            {
                // Проверяем, что DataGrid инициализирован
                if (AllBookingsDataGrid == null)
                {
                    MessageBox.Show("DataGrid не инициализирован", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверяем наличие данных
                if (Database.Bookings == null || !Database.Bookings.Any())
                {
                    AllBookingsDataGrid.ItemsSource = null;
                    return;
                }

                var bookingsQuery = from booking in Database.Bookings
                                    where booking != null
                                    join user in Database.Users on booking.UserId equals user.Id
                                    where user != null
                                    join room in Database.Rooms on booking.RoomId equals room.Id
                                    where room != null
                                    select new
                                    {
                                        booking.Id,
                                        ClientName = user.FullName ?? "Неизвестно",
                                        RoomNumber = room.Number ?? "Неизвестно",
                                        RoomType = room.Type ?? "Неизвестно",
                                        CheckInDate = booking.CheckInDate,
                                        CheckOutDate = booking.CheckOutDate,
                                        Nights = booking.CheckOutDate >= booking.CheckInDate ?
                                                (booking.CheckOutDate - booking.CheckInDate).Days : 0,
                                        TotalPrice = booking.CheckOutDate >= booking.CheckInDate ?
                                                   (booking.CheckOutDate - booking.CheckInDate).Days * room.Price : 0,
                                        Status = booking.Status ?? "Неизвестно"
                                    };

                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "Все статусы")
                {
                    bookingsQuery = bookingsQuery.Where(b => b.Status == statusFilter);
                }

                AllBookingsDataGrid.ItemsSource = bookingsQuery.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки бронирований: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                if (AllBookingsDataGrid != null)
                    AllBookingsDataGrid.ItemsSource = null;
            }
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AllBookingsDataGrid == null) return;

                string selectedStatus = "Все статусы";

                if (StatusFilterComboBox?.SelectedItem is ComboBoxItem selectedItem)
                {
                    selectedStatus = selectedItem.Content?.ToString() ?? "Все статусы";
                }

                LoadAllBookings(selectedStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}