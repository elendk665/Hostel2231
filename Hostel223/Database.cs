using System.Collections.Generic;
using System.Linq;

namespace Hostel223
{
    public static class Database
    {
        // Статические свойства с инициализацией
        public static List<User> Users { get; set; } = new List<User>();
        public static List<Room> Rooms { get; set; } = new List<Room>();
        public static List<Booking> Bookings { get; set; } = new List<Booking>();

        // Статический конструктор
        static Database()
        {
            InitializeTestData();
        }

        // Инициализация тестовых данных
        private static void InitializeTestData()
        {
            // Очищаем списки перед добавлением
            Users.Clear();
            Rooms.Clear();
            Bookings.Clear();

            // Тестовые пользователи
            Users.Add(new User { Id = 1, FullName = "Администратор", Login = "admin", Password = "admin", Role = "Администратор" });
            Users.Add(new User { Id = 2, FullName = "Менеджер Иванов", Login = "manager", Password = "manager", Role = "Менеджер" });
            Users.Add(new User { Id = 3, FullName = "Гость Петров", Login = "guest", Password = "guest", Role = "Гость" });
            Users.Add(new User { Id = 4, FullName = "Сидоров Алексей", Login = "sidorov", Password = "123", Role = "Гость" });

            // Тестовые номера
            Rooms.Add(new Room { Id = 1, Number = "101", Type = "Одноместный", Price = 1000, Status = "Свободен", Capacity = 1, Description = "Стандартный номер" });
            Rooms.Add(new Room { Id = 2, Number = "102", Type = "Двухместный", Price = 1500, Status = "Свободен", Capacity = 2, Description = "Улучшенный номер" });
            Rooms.Add(new Room { Id = 3, Number = "201", Type = "Люкс", Price = 3000, Status = "Занят", Capacity = 2, Description = "Номер люкс" });
            Rooms.Add(new Room { Id = 4, Number = "202", Type = "Эконом", Price = 800, Status = "Свободен", Capacity = 1, Description = "Эконом класс" });
            Rooms.Add(new Room { Id = 5, Number = "301", Type = "Премиум", Price = 5000, Status = "Свободен", Capacity = 3, Description = "Премиум номер" });

            // Тестовые бронирования
            Bookings.Add(new Booking
            {
                Id = 1,
                RoomId = 3,
                UserId = 3,
                CheckInDate = System.DateTime.Today,
                CheckOutDate = System.DateTime.Today.AddDays(3),
                Status = "Активно"
            });
            Bookings.Add(new Booking
            {
                Id = 2,
                RoomId = 1,
                UserId = 4,
                CheckInDate = System.DateTime.Today.AddDays(1),
                CheckOutDate = System.DateTime.Today.AddDays(5),
                Status = "Подтверждено"
            });
            Bookings.Add(new Booking
            {
                Id = 3,
                RoomId = 2,
                UserId = 3,
                CheckInDate = System.DateTime.Today.AddDays(-5),
                CheckOutDate = System.DateTime.Today.AddDays(-1),
                Status = "Завершено"
            });
            Bookings.Add(new Booking
            {
                Id = 4,
                RoomId = 4,
                UserId = 4,
                CheckInDate = System.DateTime.Today.AddDays(2),
                CheckOutDate = System.DateTime.Today.AddDays(4),
                Status = "Отменено"
            });
        }

        // Аутентификация пользователя
        public static User Authenticate(string login, string password)
        {
            return Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        // Методы для получения следующего ID
        public static int GetNextUserId() => Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1;
        public static int GetNextRoomId() => Rooms.Count > 0 ? Rooms.Max(r => r.Id) + 1 : 1;
        public static int GetNextBookingId() => Bookings.Count > 0 ? Bookings.Max(b => b.Id) + 1 : 1;

        // Методы для поиска по ID
        public static User GetUserById(int id) => Users.FirstOrDefault(u => u.Id == id);
        public static Room GetRoomById(int id) => Rooms.FirstOrDefault(r => r.Id == id);
        public static Booking GetBookingById(int id) => Bookings.FirstOrDefault(b => b.Id == id);

        // Методы для проверки существования
        public static bool UserExists(int id) => Users.Any(u => u.Id == id);
        public static bool RoomExists(int id) => Rooms.Any(r => r.Id == id);
        public static bool BookingExists(int id) => Bookings.Any(b => b.Id == id);

        // Методы для получения связанных данных
        public static User GetUserByBookingId(int bookingId)
        {
            var booking = GetBookingById(bookingId);
            return booking != null ? GetUserById(booking.UserId) : null;
        }

        public static Room GetRoomByBookingId(int bookingId)
        {
            var booking = GetBookingById(bookingId);
            return booking != null ? GetRoomById(booking.RoomId) : null;
        }

        // Методы для получения доступных номеров
        public static List<Room> GetAvailableRooms(System.DateTime checkIn, System.DateTime checkOut)
        {
            // Получаем номера, которые заняты в указанный период
            var busyRoomIds = Bookings
                .Where(b => b.Status != "Отменено" && b.Status != "Завершено")
                .Where(b => !(checkOut <= b.CheckInDate || checkIn >= b.CheckOutDate))
                .Select(b => b.RoomId)
                .Distinct()
                .ToList();

            // Возвращаем свободные номера
            return Rooms
                .Where(r => r.Status == "Свободен" && !busyRoomIds.Contains(r.Id))
                .ToList();
        }

        // Методы для обновления статусов
        public static bool UpdateRoomStatus(int roomId, string status)
        {
            var room = GetRoomById(roomId);
            if (room != null)
            {
                room.Status = status;
                return true;
            }
            return false;
        }

        public static bool UpdateBookingStatus(int bookingId, string status)
        {
            var booking = GetBookingById(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                return true;
            }
            return false;
        }

        // Метод для добавления нового бронирования
        public static bool AddBooking(int userId, int roomId, System.DateTime checkIn, System.DateTime checkOut, string status = "Активно")
        {
            try
            {
                // Проверяем доступность номера
                var availableRooms = GetAvailableRooms(checkIn, checkOut);
                if (!availableRooms.Any(r => r.Id == roomId))
                    return false;

                // Создаем новое бронирование
                var newBooking = new Booking
                {
                    Id = GetNextBookingId(),
                    UserId = userId,
                    RoomId = roomId,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    Status = status
                };

                Bookings.Add(newBooking);

                // Обновляем статус номера
                UpdateRoomStatus(roomId, "Занят");

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Метод для удаления бронирования
        public static bool DeleteBooking(int bookingId)
        {
            try
            {
                var booking = GetBookingById(bookingId);
                if (booking != null)
                {
                    // Освобождаем номер
                    UpdateRoomStatus(booking.RoomId, "Свободен");

                    // Удаляем бронирование
                    Bookings.Remove(booking);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Метод для сброса данных к тестовым
        public static void ResetToTestData()
        {
            InitializeTestData();
        }

        // Классы моделей
        public class User
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Phone { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public class Room
        {
            public int Id { get; set; }
            public string Number { get; set; }
            public string Type { get; set; }
            public decimal Price { get; set; }
            public string Status { get; set; }
            public int Capacity { get; set; }
            public string Description { get; set; }
        }

        public class Booking
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int RoomId { get; set; }
            public System.DateTime CheckInDate { get; set; }
            public System.DateTime CheckOutDate { get; set; }
            public string Status { get; set; }
        }
    }
}