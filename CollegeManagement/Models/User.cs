using System.Security.Cryptography;
using System.Text;

namespace CollegeManagement.Models;

/// <summary>
/// Задание 26, 27.
/// Базовый класс User и иерархия ролей (Admin, TeacherUser, StudentUser).
/// Использует protected для общих данных и private для конфиденциальных данных.
/// </summary>
public abstract class User
{
    // Задание 27: protected для общих данных
    protected string _username = string.Empty;
    protected string _role = string.Empty;
    protected string _displayName = string.Empty;
    protected string _email = string.Empty;

    // Задание 26, 27: private для конфиденциальных данных (пароль и соль)
    private string _passwordHash = string.Empty;
    private string _passwordSalt = string.Empty;

    public int Id { get; set; }

    public string Username
    {
        get => _username;
        set => _username = value?.Trim() ?? string.Empty;
    }

    public string Role
    {
        get => _role;
        protected set => _role = value;
    }

    public string DisplayName
    {
        get => _displayName;
        set => _displayName = value?.Trim() ?? string.Empty;
    }

    public string Email
    {
        get => _email;
        set => _email = value?.Trim() ?? string.Empty;
    }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    protected User()
    {
    }

    protected User(string username, string password, string displayName, string email, string role)
    {
        _username = username?.Trim() ?? string.Empty;
        _displayName = displayName?.Trim() ?? string.Empty;
        _email = email?.Trim() ?? string.Empty;
        _role = role;
        SetPassword(password);
    }

    /// <summary>
    /// Безопасная установка пароля с солью
    /// </summary>
    public void SetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Пароль не может быть пустым.", nameof(password));

        _passwordSalt = GenerateSalt();
        _passwordHash = HashPassword(password, _passwordSalt);
    }

    /// <summary>
    /// Проверка пароля без раскрытия приватного хэша
    /// </summary>
    public bool VerifyPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(_passwordHash))
            return false;

        string computed = HashPassword(password, _passwordSalt);
        return computed == _passwordHash;
    }

    private static string GenerateSalt()
    {
        byte[] bytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        byte[] combined = Encoding.UTF8.GetBytes(password + salt + "CollegeSalt_2026");
        byte[] hash = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Задание 26: Пароль не должен отображаться при выводе информации.
    /// </summary>
    public virtual string GetUserInfo()
    {
        return $"Пользователь: {_username} ({_displayName}), Роль: {_role}, Email: {_email}";
    }

    public override string ToString()
    {
        return GetUserInfo();
    }
}

/// <summary>
/// Задание 27: Роль Администратора
/// </summary>
public class Admin : User
{
    public Admin()
    {
        _role = "Admin";
    }

    public Admin(string username, string password, string displayName, string email)
        : base(username, password, displayName, email, "Admin")
    {
    }
}

/// <summary>
/// Задание 27: Роль Преподавателя
/// </summary>
public class TeacherUser : User
{
    public int? TeacherEntityId { get; set; }

    public TeacherUser()
    {
        _role = "Teacher";
    }

    public TeacherUser(string username, string password, string displayName, string email, int? teacherEntityId = null)
        : base(username, password, displayName, email, "Teacher")
    {
        TeacherEntityId = teacherEntityId;
    }
}

/// <summary>
/// Задание 27: Роль Студента
/// </summary>
public class StudentUser : User
{
    public int? StudentEntityId { get; set; }

    public StudentUser()
    {
        _role = "Student";
    }

    public StudentUser(string username, string password, string displayName, string email, int? studentEntityId = null)
        : base(username, password, displayName, email, "Student")
    {
        StudentEntityId = studentEntityId;
    }
}

/// <summary>
/// Задание 26: Система авторизации. Класс UserAccount с приватными полями логина и пароля.
/// Реализует методы Login() и Logout(). Пароль не отображается при выводе.
/// </summary>
public class UserAccount
{
    // Задание 26: приватные поля логина и пароля
    private string _login = string.Empty;
    private string _password = string.Empty;
    private bool _isAuthenticated;

    public string Username => _login;
    public bool IsAuthenticated => _isAuthenticated;
    public User? AssociatedUser { get; private set; }

    public UserAccount()
    {
    }

    public UserAccount(string login, string password)
    {
        _login = login?.Trim() ?? string.Empty;
        _password = password ?? string.Empty;
    }

    /// <summary>
    /// Задание 26: публичный метод Login()
    /// </summary>
    public bool Login(string username, string password, User? existingUser = null)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            _isAuthenticated = false;
            return false;
        }

        if (existingUser != null)
        {
            if (existingUser.VerifyPassword(password))
            {
                _login = username.Trim();
                _password = string.Empty; // Пароль не сохраняем в открытом виде
                _isAuthenticated = true;
                AssociatedUser = existingUser;
                return true;
            }
            _isAuthenticated = false;
            return false;
        }

        // Простая аутентификация по сохранённому паролю
        if (_login.Equals(username, StringComparison.OrdinalIgnoreCase) && _password == password)
        {
            _isAuthenticated = true;
            return true;
        }

        _isAuthenticated = false;
        return false;
    }

    /// <summary>
    /// Задание 26: публичный метод Logout()
    /// </summary>
    public void Logout()
    {
        _isAuthenticated = false;
        AssociatedUser = null;
        _password = string.Empty;
    }

    /// <summary>
    /// Задание 26: Пароль не должен отображаться при выводе информации.
    /// </summary>
    public override string ToString()
    {
        return $"Аккаунт: {_login}, Статус: {(_isAuthenticated ? "Авторизован" : "Не в сети")}";
    }
}
