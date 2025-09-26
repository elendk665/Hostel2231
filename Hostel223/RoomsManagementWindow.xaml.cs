using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static Hostel223.Database;

namespace Hostel223
{
    public partial class RoomsManagementWindow : Window
    {
        public RoomsManagementWindow()
        {
            InitializeComponent(); // Должно быть первой строкой!
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRooms();
        }

        private void LoadRooms()
        {
            try
            {
                if (RoomsDataGrid == null)
                {
                    MessageBox.Show("Ошибка инициализации интерфейса");
                    return;
                }

                var rooms = Database.Rooms.ToList();
                RoomsDataGrid.ItemsSource = rooms;

                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Загружено номеров: {rooms.Count}";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {
            RoomEditWindow roomEditWindow = new RoomEditWindow();
            if (roomEditWindow.ShowDialog() == true)
            {
                LoadRooms();
            }
        }

        private void EditRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите номер для редактирования");
                return;
            }

            var selectedRoom = RoomsDataGrid.SelectedItem as Room;
            RoomEditWindow roomEditWindow = new RoomEditWindow(selectedRoom);
            if (roomEditWindow.ShowDialog() == true)
            {
                LoadRooms();
            }
        }

        private void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoomsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите номер для удаления");
                return;
            }

            var selectedRoom = RoomsDataGrid.SelectedItem as Room;
            if (MessageBox.Show($"Удалить номер {selectedRoom.Number}?", "Подтверждение",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Database.Rooms.Remove(selectedRoom);
                LoadRooms();
                MessageBox.Show("Номер удален");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRooms();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilterComboBox.SelectedItem == null || RoomsDataGrid == null) return;

            var selectedItem = StatusFilterComboBox.SelectedItem as ComboBoxItem;
            string selectedStatus = selectedItem.Content.ToString();

            if (selectedStatus == "Все статусы")
            {
                RoomsDataGrid.ItemsSource = Database.Rooms.ToList();
            }
            else
            {
                RoomsDataGrid.ItemsSource = Database.Rooms.Where(r => r.Status == selectedStatus).ToList();
            }
        }
    }
}