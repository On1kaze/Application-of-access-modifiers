using System.Reflection;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Xunit;

namespace CollegeManagement.Tests;

/// <summary>
/// Тесты для Раздела 5: Реализация функциональности сайта (Задания 26-30).
/// </summary>
public class Section5_SiteFeaturesTests
{
    // Задание 26: Система авторизации, класс UserAccount с приватными логином и паролем
    [Fact]
    public void Task26_UserAccount_ShouldHavePrivateCredentials_AndNeverDisplayPassword()
    {
        var loginField = typeof(UserAccount).GetField("_login", BindingFlags.NonPublic | BindingFlags.Instance);
        var passwordField = typeof(UserAccount).GetField("_password", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(loginField);
        Assert.True(loginField.IsPrivate, "Поле _login должно быть private.");

        Assert.NotNull(passwordField);
        Assert.True(passwordField.IsPrivate, "Поле _password должно быть private.");

        var account = new UserAccount("admin", "secretPassword123");

        // Пароль НЕ должен отображаться в ToString()
        string displayInfo = account.ToString();
        Assert.DoesNotContain("secretPassword123", displayInfo);

        // Проверка методов Login() и Logout()
        Assert.True(account.Login("admin", "secretPassword123"));
        Assert.True(account.IsAuthenticated);

        account.Logout();
        Assert.False(account.IsAuthenticated);
    }

    // Задание 27: Разделение ролей пользователей (User, Admin, TeacherUser, StudentUser)
    [Fact]
    public void Task27_UserRoles_ShouldHaveProtectedSharedData_AndPrivateSecrets()
    {
        // Protected поля базового класса User
        var usernameField = typeof(User).GetField("_username", BindingFlags.NonPublic | BindingFlags.Instance);
        var roleField = typeof(User).GetField("_role", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(usernameField);
        Assert.True(usernameField.IsFamily, "Поле _username должно быть protected (Family).");
        Assert.NotNull(roleField);
        Assert.True(roleField.IsFamily, "Поле _role должно быть protected (Family).");

        // Private поля для конфиденциальных данных (пароль и соль)
        var hashField = typeof(User).GetField("_passwordHash", BindingFlags.NonPublic | BindingFlags.Instance);
        var saltField = typeof(User).GetField("_passwordSalt", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(hashField);
        Assert.True(hashField.IsPrivate, "Поле _passwordHash должно быть private.");
        Assert.NotNull(saltField);
        Assert.True(saltField.IsPrivate, "Поле _passwordSalt должно быть private.");

        // Проверка создания пользователей ролей
        var admin = new Admin("admin", "adminPass", "Администратор", "admin@college.ru");
        var teacher = new TeacherUser("teacher", "teacherPass", "Преподаватель", "t@college.ru");
        var student = new StudentUser("student", "studentPass", "Студент", "s@college.ru");

        Assert.Equal("Admin", admin.Role);
        Assert.Equal("Teacher", teacher.Role);
        Assert.Equal("Student", student.Role);

        // Проверка хэширования пароля
        Assert.True(admin.VerifyPassword("adminPass"));
        Assert.False(admin.VerifyPassword("wrongPass"));
    }

    // Задание 28: Журнал оценок (GradeBook), закрытый список и проверка диапазона 2-5
    [Fact]
    public void Task28_GradeBook_ShouldHavePrivateList_AndValidateGradeBetween2And5()
    {
        var field = typeof(GradeBook).GetField("_grades", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        Assert.True(field.IsPrivate, "Список _grades в GradeBook должен быть private.");

        var gradeBook = new GradeBook();

        // Допустимые оценки от 2 до 5
        Assert.True(gradeBook.AddGrade(1, "Студент", 1, "C#", 5, "Отлично"));
        Assert.True(gradeBook.AddGrade(1, "Студент", 1, "C#", 4, "Хорошо"));
        Assert.True(gradeBook.AddGrade(1, "Студент", 1, "C#", 3, "Удовлетворительно"));
        Assert.True(gradeBook.AddGrade(1, "Студент", 1, "C#", 2, "Неудовлетворительно"));

        Assert.Equal(4, gradeBook.Grades.Count);

        // Недопустимые оценки: меньше 2 или больше 5
        Assert.Throws<ArgumentOutOfRangeException>(() => gradeBook.AddGrade(1, "Студент", 1, "C#", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => gradeBook.AddGrade(1, "Студент", 1, "C#", 6));
        Assert.Throws<ArgumentOutOfRangeException>(() => gradeBook.AddGrade(1, "Студент", 1, "C#", 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => gradeBook.AddGrade(1, "Студент", 1, "C#", -3));

        // Удаление оценки
        var firstGrade = gradeBook.Grades[0];
        Assert.True(gradeBook.RemoveGrade(firstGrade));
        Assert.Equal(3, gradeBook.Grades.Count);
    }

    // Задание 29: Система посещаемости (AttendanceManager) с приватными данными и статистикой
    [Fact]
    public void Task29_AttendanceManager_ShouldHavePrivateData_AndComputeStatistics()
    {
        var field = typeof(AttendanceManager).GetField("_records", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        Assert.True(field.IsPrivate, "Поле _records в AttendanceManager должно быть private.");

        var manager = new AttendanceManager();

        var r1 = new AttendanceRecord(1, 10, "Студент 1", 1, "C#", DateTime.Today, AttendanceStatus.Present);
        var r2 = new AttendanceRecord(2, 10, "Студент 1", 1, "C#", DateTime.Today.AddDays(-1), AttendanceStatus.Late);
        var r3 = new AttendanceRecord(3, 10, "Студент 1", 1, "C#", DateTime.Today.AddDays(-2), AttendanceStatus.Absent);

        manager.AddRecord(r1);
        manager.AddRecord(r2);
        manager.AddRecord(r3);

        // Обновление статуса
        Assert.True(manager.UpdateRecord(3, AttendanceStatus.Excused, "Справка"));

        // Статистика
        var stats = manager.GetAttendanceStatistics();
        Assert.Equal(3, stats.TotalClasses);
        Assert.Equal(1, stats.PresentCount);
        Assert.Equal(1, stats.LateCount);
        Assert.Equal(1, stats.ExcusedCount);
        Assert.Equal(0, stats.AbsentCount);
    }

    // Задание 30: Финальная проверка интеграции и моделей
    [Fact]
    public void Task30_FinalAssembly_ShouldEnsureAllComponentsFunction()
    {
        var group = new Group(1, "ИС-21", 2);
        var student = new Student(1, "Иван", "Петров", 19, "ИС-21");
        var teacher = new Teacher(1, "Александр", "Смирнов", 45, "C#", 20);
        var subject = new Subject(1, "Разработка на C#", 120);

        group.AddStudent(student);
        subject.AssignTeacher(teacher);

        Assert.Contains(student, group.Students);
        Assert.Equal(teacher, subject.Teacher);
        Assert.NotNull(student.GetStudentInfo());
        Assert.NotNull(teacher.GetTeacherInfo());
    }
}
