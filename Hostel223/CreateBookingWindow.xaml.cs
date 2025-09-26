using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hostel223
{
    public partial class CreateBookingWindow : Window
    {
        private Database.User _manager;

        public CreateBookingWindow(Database.User manager)
        {
            InitializeComponent();
            _manager = manager;
            InitializeData();
        }

        private void InitializeData()
        {
            // Загружаем клиентов (пользователей с ролью "Гость")
            ClientComboBox.ItemsSource = Database.Users.Where(u => u.Role == "Гость").ToList();
            ClientComboBox.SelectedIndex = 0;

            // Загружаем свободные номера
            RoomComboBox.ItemsSource = Database.Rooms.Where(r => r.Status == "Свободен").ToList();
            RoomComboBox.SelectedIndex = 0;

            // Устанавливаем даты по умолчанию
            CheckInDatePicker.SelectedDate = DateTime.Today;
            CheckOutDatePicker.SelectedDate = DateTime.Today.AddDays(1);
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
                RoomPriceText.Text = $"{selectedRoom.Price}₽";
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

        // Кнопка добавления нового клиента
        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Показываем панель для добавления нового клиента
            NewClientPanel.Visibility = Visibility.Visible;
            ErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        // Кнопка сохранения нового клиента
        private void SaveNewClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateNewClientInput())
            {
                SaveNewClient();
            }
        }

        // Кнопка отмены добавления клиента
        private void CancelNewClientButton_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем панель и очищаем поля
            NewClientPanel.Visibility = Visibility.Collapsed;
            ClearNewClientFields();
            ErrorTextBlock.Visibility = Visibility.Collapsed;
        }

        private bool ValidateNewClientInput()
        {
            if (string.IsNullOrWhiteSpace(NewClientNameTextBox.Text))
            {
                ShowError("Введите ФИО клиента");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewClientPhoneTextBox.Text))
            {
                ShowError("Введите телефон клиента");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NewClientPassportTextBox.Text))
            {
                ShowError("Введите паспортные данные клиента");
                return false;
            }

            return true;
        }

        private void SaveNewClient()
        {
            try
            {
                // Создаем нового клиента
                var newClient = new Database.User
                {
                    Id = Database.Users.Count > 0 ? Database.Users.Max(u => u.Id) + 1 : 1,
                    FullName = NewClientNameTextBox.Text.Trim(),
                    Phone = NewClientPhoneTextBox.Text.Trim(),
                    // Генерируем логин и пароль автоматически
                    Login = GenerateLogin(NewClientNameTextBox.Text.Trim()),
                    Password = "123456", // Пароль по умолчанию
                    Role = "Гость"
                };

                // Добавляем клиента в базу
                Database.Users.Add(newClient);

                // Обновляем комбобокс с клиентами
                ClientComboBox.ItemsSource = Database.Users.Where(u => u.Role == "Гость").ToList();
                ClientComboBox.SelectedItem = newClient;

                // Скрываем панель и очищаем поля
                NewClientPanel.Visibility = Visibility.Collapsed;
                ClearNewClientFields();

                MessageBox.Show($"Клиент {newClient.FullName} успешно добавлен!\nЛогин: {newClient.Login}\nПароль: {newClient.Password}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при добавлении клиента: {ex.Message}");
            }
        }

        private string GenerateLogin(string fullName)
        {
            // Генерируем логин на основе ФИО
            string[] nameParts = fullName.Split(' ');
            string login = "";

            if (nameParts.Length > 0)
                login += nameParts[0].ToLower();
            if (nameParts.Length > 1)
                login += nameParts[1].Substring(0, 1).ToLower();
            if (nameParts.Length > 2)
                login += nameParts[2].Substring(0, 1).ToLower();

            // Добавляем число, если логин уже существует
            string baseLogin = login;
            int counter = 1;

            while (Database.Users.Any(u => u.Login == login))
            {
                login = baseLogin + counter.ToString();
                counter++;
            }

            return login;
        }

        private void ClearNewClientFields()
        {
            NewClientNameTextBox.Text = "";
            NewClientPhoneTextBox.Text = "";
            NewClientPassportTextBox.Text = "";
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                CreateBooking();
            }
        }

        private bool ValidateInput()
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            if (ClientComboBox.SelectedItem == null)
            {
                ShowError("Выберите клиента");
                return false;
            }

            if (RoomComboBox.SelectedItem == null)
            {
                ShowError("Выберите номер");
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

            return true;
        }

        private void CreateBooking()
        {
            try
            {
                var client = (Database.User)ClientComboBox.SelectedItem;
                var room = (Database.Room)RoomComboBox.SelectedItem;
                var checkIn = CheckInDatePicker.SelectedDate.Value;
                var checkOut = CheckOutDatePicker.SelectedDate.Value;

                // Создаем новое бронирование
                var newBooking = new Database.Booking
                {
                    Id = Database.Bookings.Count > 0 ? Database.Bookings.Max(b => b.Id) + 1 : 1,
                    UserId = client.Id,
                    RoomId = room.Id,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    Status = "Подтверждено"
                };

                // Обновляем статус номера
                room.Status = "Занят";

                Database.Bookings.Add(newBooking);

                MessageBox.Show("Бронирование успешно создано!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при создании бронирования: {ex.Message}");
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
            ErrorTextBlock.Visibility = Visibility.Visible;
        }
    }
}