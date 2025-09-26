using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hostel223
{
    public partial class EditBookingWindow : Window
    {
        private Database.Booking _booking;
        private Database.Room _originalRoom;

        public EditBookingWindow(int bookingId)
        {
            InitializeComponent();
            _booking = Database.Bookings.FirstOrDefault(b => b.Id == bookingId);
            _originalRoom = Database.Rooms.FirstOrDefault(r => r.Id == _booking.RoomId);
            LoadBookingData();
        }

        private void LoadBookingData()
        {
            if (_booking == null) return;

            var client = Database.Users.FirstOrDefault(u => u.Id == _booking.UserId);
            var room = Database.Rooms.FirstOrDefault(r => r.Id == _booking.RoomId);

            BookingIdText.Text = $"№{_booking.Id}";
            ClientText.Text = client?.FullName ?? "Неизвестно";

            // Загружаем все номера
            RoomComboBox.ItemsSource = Database.Rooms.ToList();
            RoomComboBox.SelectedItem = room;

            CheckInDatePicker.SelectedDate = _booking.CheckInDate;
            CheckOutDatePicker.SelectedDate = _booking.CheckOutDate;

            // Устанавливаем статус
            foreach (ComboBoxItem item in StatusComboBox.Items)
            {
                if (item.Content.ToString() == _booking.Status)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            UpdateRoomInfo();
            CalculateTotal();
        }

        private void RoomComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRoomInfo();
            CalculateTotal();
        }

        private void Dates_Changed(object sender, SelectionChangedEventArgs e)
        {
            CalculateTotal();
        }

        private void UpdateRoomInfo()
        {
            if (RoomComboBox.SelectedItem is Database.Room selectedRoom)
            {
                RoomTypeText.Text = selectedRoom.Type;
                RoomPriceText.Text = $"{selectedRoom.Price}₽/ночь";
            }
        }

        private void CalculateTotal()
        {
            if (RoomComboBox.SelectedItem is Database.Room selectedRoom &&
                CheckInDatePicker.SelectedDate.HasValue &&
                CheckOutDatePicker.SelectedDate.HasValue)
            {
                var checkIn = CheckInDatePicker.SelectedDate.Value;
                var checkOut = CheckOutDatePicker.SelectedDate.Value;

                if (checkOut > checkIn)
                {
                    int nights = (checkOut - checkIn).Days;
                    decimal totalPrice = nights * selectedRoom.Price;

                    NightsText.Text = $"Количество ночей: {nights}";
                    TotalPriceText.Text = $"Общая стоимость: {totalPrice}₽";
                }
                else
                {
                    NightsText.Text = "Количество ночей: 0";
                    TotalPriceText.Text = "Общая стоимость: 0₽";
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                SaveBookingChanges();
            }
        }

        private bool ValidateInput()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;

            if (RoomComboBox.SelectedItem == null)
            {
                ShowError("Выберите номер для бронирования");
                return false;
            }

            if (!CheckInDatePicker.SelectedDate.HasValue || !CheckOutDatePicker.SelectedDate.HasValue)
            {
                ShowError("Укажите даты заезда и выезда");
                return false;
            }

            if (CheckOutDatePicker.SelectedDate <= CheckInDatePicker.SelectedDate)
            {
                ShowError("Дата выезда должна быть позже даты заезда");
                return false;
            }

            if (CheckInDatePicker.SelectedDate < DateTime.Today)
            {
                ShowError("Дата заезда не может быть в прошлом");
                return false;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                ShowError("Выберите статус бронирования");
                return false;
            }

            return true;
        }

        private void SaveBookingChanges()
        {
            try
            {
                var selectedRoom = (Database.Room)RoomComboBox.SelectedItem;
                var status = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                var checkIn = CheckInDatePicker.SelectedDate.Value;
                var checkOut = CheckOutDatePicker.SelectedDate.Value;

                // Если номер изменился, обновляем статусы комнат
                if (selectedRoom.Id != _originalRoom.Id)
                {
                    // Освобождаем старый номер
                    _originalRoom.Status = "Свободен";

                    // Занимаем новый номер, если бронирование активно
                    if (status == "Подтверждено" || status == "Активно")
                    {
                        selectedRoom.Status = "Занят";
                    }
                    else
                    {
                        selectedRoom.Status = "Свободен";
                    }
                }
                else
                {
                    // Обновляем статус текущего номера в зависимости от статуса бронирования
                    if (status == "Подтверждено" || status == "Активно")
                    {
                        selectedRoom.Status = "Занят";
                    }
                    else if (status == "Завершено" || status == "Отменено")
                    {
                        selectedRoom.Status = "Свободен";
                    }
                }

                // Обновляем данные бронирования
                _booking.RoomId = selectedRoom.Id;
                _booking.CheckInDate = checkIn;
                _booking.CheckOutDate = checkOut;
                _booking.Status = status;

                MessageBox.Show("Изменения в бронировании сохранены успешно!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при сохранении изменений: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }
    }
}