using System.Collections.Generic;
using System.Linq;

public static class Database
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
    }

    private static List<User> _users = new List<User>
    {
        new User { Login = "admin", Password = "admin123", Role = "Администратор", FullName = "Иван Петров" },
        new User { Login = "manager", Password = "manager123", Role = "Менеджер", FullName = "Анна Сидорова" },
        new User { Login = "guest", Password = "guest123", Role = "Гость", FullName = "Сергей Иванов" }
    };

    public static User Authenticate(string login, string password)
    {
        return _users.FirstOrDefault(u => u.Login == login && u.Password == password);
    }
}