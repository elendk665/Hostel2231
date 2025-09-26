using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace Hostel223
{
    public partial class RoomEditWindow : Window
    {
        private Database.Room _editingRoom;

        public RoomEditWindow()
        {
            InitializeComponent();
            Title = "Добавление номера";
        }

        public RoomEditWindow(Database.Room room)
        {
            InitializeComponent();
            _editingRoom = room;
            Title = "Редактирование номера";
            LoadRoomData();
        }

        private void LoadRoomData()
        {
            if (_editingRoom != null)
            {
                NumberTextBox.Text = _editingRoom.Number;
                PriceTextBox.Text = _editingRoom.Price.ToString();
                CapacityTextBox.Text = _editingRoom.Capacity.ToString();
                DescriptionTextBox.Text = _editingRoom.Description;

                // Установка выбранных значений в комбобоксы
                SetComboBoxValue(TypeComboBox, _editingRoom.Type);
                SetComboBoxValue(StatusComboBox, _editingRoom.Status);
            }
        }

        private void SetComboBoxValue(System.Windows.Controls.ComboBox comboBox, string value)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == value)
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string number = NumberTextBox.Text.Trim();
            string type = (TypeComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            string status = (StatusComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString();
            string priceText = PriceTextBox.Text.Trim();
            string capacityText = CapacityTextBox.Text.Trim();
            string description = DescriptionTextBox.Text.Trim();

            // Валидация
            if (string.IsNullOrEmpty(number) || string.IsNullOrEmpty(priceText) ||
                string.IsNullOrEmpty(capacityText) || TypeComboBox.SelectedItem == null ||
                StatusComboBox.SelectedItem == null)
            {
                ShowError("Все поля должны быть заполнены");
                return;
            }

            if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
            {
                ShowError("Некорректное значение цены");
                return;
            }

            if (!int.TryParse(capacityText, out int capacity) || capacity <= 0)
            {
                ShowError("Вместимость должна быть положительным числом");
                return;
            }

            if (_editingRoom == null)
            {
                // Добавление нового номера
                Database.Room newRoom = new Database.Room
                {
                    Id = Database.Rooms.Count > 0 ? Database.Rooms.Max(r => r.Id) + 1 : 1,
                    Number = number,
                    Type = type,
                    Price = price,
                    Status = status,
                    Capacity = capacity,
                    Description = description
                };

                Database.Rooms.Add(newRoom);
            }
            else
            {
                // Редактирование существующего номера
                _editingRoom.Number = number;
                _editingRoom.Type = type;
                _editingRoom.Price = price;
                _editingRoom.Status = status;
                _editingRoom.Capacity = capacity;
                _editingRoom.Description = description;
            }

            DialogResult = true;
            Close();
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