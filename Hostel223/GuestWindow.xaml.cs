using System;
using System.Linq;
using System.Windows;

namespace Hostel223
{
    public partial class GuestWindow : Window
    {
        private Database.User _currentUser;

        public GuestWindow(Database.User user)
        {
            InitializeComponent();
            _currentUser = user;
            InitializeInterface();
            LoadAvailableRooms();
        }

        private void InitializeInterface()
        {
            Title = $"Hostel223 - Гость: {_currentUser.FullName}";
            UserInfoTextBlock.Text = $"Пользователь: {_currentUser.FullName}";
        }

        private void LoadAvailableRooms()
        {
            // Загружаем свободные номера
            var availableRooms = Database.Rooms
                .Where(r => r.Status == "Свободен")
                .ToList();

            AvailableRoomsList.ItemsSource = availableRooms;
        }

        private void BookRoomButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно бронирования
            GuestBookingWindow bookingWindow = new GuestBookingWindow(_currentUser);
            if (bookingWindow.ShowDialog() == true)
            {
                LoadAvailableRooms(); // Обновляем список после бронирования
                MessageBox.Show("Номер успешно забронирован!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MyBookingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно с бронированиями пользователя
            GuestBookingsWindow bookingsWindow = new GuestBookingsWindow(_currentUser);
            bookingsWindow.ShowDialog();
            LoadAvailableRooms(); // Обновляем список после возможных изменений
        }

        private void HelpInfoButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем справочную информацию
            GuestHelpWindow helpWindow = new GuestHelpWindow();
            helpWindow.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }

    internal class GuestHelpWindow
    {
        public GuestHelpWindow()
        {
        }

        internal void ShowDialog()
        {
            throw new NotImplementedException();
        }
    }

    internal class GuestBookingsWindow
    {
        private Database.User currentUser;

        public GuestBookingsWindow(Database.User currentUser)
        {
            this.currentUser = currentUser;
        }

        internal void ShowDialog()
        {
            throw new NotImplementedException();
        }
    }

    internal class GuestBookingWindow
    {
        private Database.User currentUser;

        public GuestBookingWindow(Database.User currentUser)
        {
            this.currentUser = currentUser;
        }

        internal bool ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}