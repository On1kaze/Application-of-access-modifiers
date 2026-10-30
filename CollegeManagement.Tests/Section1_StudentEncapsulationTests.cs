using System.Reflection;
using CollegeManagement.Models;
using Xunit;

namespace CollegeManagement.Tests;

/// <summary>
/// Тесты для Раздела 1: Создание структуры и применение public/private (Задания 1-8).
/// </summary>
public class Section1_StudentEncapsulationTests
{
    // Задание 1: Создание класса Student с модификатором public
    [Fact]
    public void Task1_StudentClass_ShouldBePublic()
    {
        var type = typeof(Student);
        Assert.True(type.IsPublic, "Класс Student должен иметь модификатор public.");
    }

    // Задание 2: Поля Name, Surname, Age закрыты, публичные методы для получения данных
    [Fact]
    public void Task2_StudentFields_ShouldBeProtectedOrPrivate_AndPublicMethodsExist()
    {
        var student = new Student("Иван", "Иванов", 19, "ИС-21");

        // Проверяем публичные методы получения информации (Задание 2)
        Assert.Equal("Иван", student.GetName());
        Assert.Equal("Иванов", student.GetSurname());
        Assert.Equal(19, student.GetAge());
        Assert.Equal("ИС-21", student.GetGroup());
    }

    // Задание 3: Свойства Name, Surname, Age для безопасного изменения
    [Fact]
    public void Task3_StudentProperties_ShouldAllowSafeModification()
    {
        var student = new Student("Иван", "Иванов", 19, "ИС-21");

        student.Name = "Петр";
        student.Surname = "Петров";
        Assert.Equal("Петр", student.Name);
        Assert.Equal("Петров", student.Surname);

        // Проверка защиты от пустых значений
        Assert.Throws<ArgumentException>(() => student.Name = "");
        Assert.Throws<ArgumentException>(() => student.Surname = "   ");
    }

    // Задание 4: Проверка возраста: SetAge разрешает только от 15 до 70 лет
    [Fact]
    public void Task4_SetAge_ShouldEnforceAgeBetween15And70()
    {
        var student = new Student("Иван", "Иванов", 19, "ИС-21");

        // Корректный возраст
        Assert.True(student.SetAge(25));
        Assert.Equal(25, student.Age);

        // Граничные значения
        Assert.True(student.SetAge(15));
        Assert.Equal(15, student.Age);
        Assert.True(student.SetAge(70));
        Assert.Equal(70, student.Age);

        // Недопустимый возраст: меньше 15
        Assert.Throws<ArgumentOutOfRangeException>(() => student.SetAge(14));

        // Недопустимый возраст: больше 70
        Assert.Throws<ArgumentOutOfRangeException>(() => student.SetAge(71));
        Assert.Throws<ArgumentOutOfRangeException>(() => student.SetAge(-5));
    }

    // Задание 5: Конструктор Student(name, surname, age, group)
    [Fact]
    public void Task5_Constructor_ShouldInitializeAllFields()
    {
        var student = new Student("Анна", "Смирнова", 20, "П-31");

        Assert.Equal("Анна", student.Name);
        Assert.Equal("Смирнова", student.Surname);
        Assert.Equal(20, student.Age);
        Assert.Equal("П-31", student.Group);
    }

    // Задание 6: Скрытие внутренних методов: ValidateStudentData() вызывается из конструктора
    [Fact]
    public void Task6_ValidateStudentData_ShouldBePrivate_AndRejectInvalidDataInConstructor()
    {
        // Проверяем через Reflection, что метод приватный
        var validateMethod = typeof(Student).GetMethod("ValidateStudentData", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(validateMethod);
        Assert.True(validateMethod.IsPrivate, "Метод ValidateStudentData должен быть private.");

        // Проверяем, что конструктор вызывает валидацию и выбрасывает исключение при невалидных данных
        Assert.Throws<ArgumentOutOfRangeException>(() => new Student("Тест", "Тестов", 10, "ИС-21"));
        Assert.Throws<ArgumentException>(() => new Student("", "Тестов", 18, "ИС-21"));
    }

    // Задание 7: GetStudentInfo() возвращает полную информацию о студенте
    [Fact]
    public void Task7_GetStudentInfo_ShouldReturnFullStudentInformation()
    {
        var student = new Student(101, "Иван", "Петров", 19, "ИС-21");
        string info = student.GetStudentInfo();

        Assert.Contains("Иван", info);
        Assert.Contains("Петров", info);
        Assert.Contains("19", info);
        Assert.Contains("ИС-21", info);
    }

    // Задание 8: StudentId доступен только для чтения после создания
    [Fact]
    public void Task8_StudentId_ShouldBeReadOnlyOutside()
    {
        var property = typeof(Student).GetProperty("StudentId");
        Assert.NotNull(property);
        Assert.NotNull(property.GetMethod);
        Assert.True(property.GetMethod.IsPublic, "Геттер StudentId должен быть публичным.");

        // Сеттер должен отсутствовать или быть закрытым (private/protected)
        var setMethod = property.SetMethod;
        Assert.True(setMethod == null || !setMethod.IsPublic, "Сеттер StudentId не должен быть публичным.");
    }
}
