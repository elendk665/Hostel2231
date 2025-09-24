using System.Collections.Generic;
using System.Linq;

namespace Hostel223
{
    public static class Database
    {
        public static List<User> Users { get; set; } = new List<User>
        {
            new User { Id = 1, FullName = "Администратор Системы", Login = "admin", Password = "admin", Role = "Администратор" },
            new User { Id = 2, FullName = "Менеджер Иванов", Login = "manager", Password = "manager", Role = "Менеджер" },
            new User { Id = 3, FullName = "Гость Петров", Login = "guest", Password = "guest", Role = "Гость" }
        };

        public static User Authenticate(string login, string password)
        {
            return Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public class User
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }
    }
}