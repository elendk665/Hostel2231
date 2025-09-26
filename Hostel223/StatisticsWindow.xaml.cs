// StatisticsWindow.xaml.cs
using System.Linq;
using System.Windows;

namespace Hostel223
{
    public partial class StatisticsWindow : Window
    {
        public StatisticsWindow()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            // Общая статистика
            TotalRoomsText.Text = Database.Rooms.Count.ToString();
            OccupiedRoomsText.Text = Database.Rooms.Count(r => r.Status == "Занят").ToString();

            if (Database.Rooms.Count > 0)
            {
                double occupancyRate = (double)Database.Rooms.Count(r => r.Status == "Занят") / Database.Rooms.Count * 100;
                OccupancyRateText.Text = $"{occupancyRate:F1}%";
            }
            else
            {
                OccupancyRateText.Text = "0%";
            }

            // Финансы (заглушки)
            MonthlyIncomeText.Text = "45,000 руб";
            AverageBillText.Text = "1,500 руб";
            TotalBookingsText.Text = "30";

            // Популярные номера
            var popularRooms = Database.Rooms.Select(r => new
            {
                Number = r.Number,
                Type = r.Type,
                BookingCount = new System.Random().Next(1, 10)
            }).OrderByDescending(r => r.BookingCount).ToList();

            PopularRoomsList.ItemsSource = popularRooms;
        }
    }
}