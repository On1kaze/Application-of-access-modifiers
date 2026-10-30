using System.Reflection;
using CollegeManagement.Models;
using Xunit;

namespace CollegeManagement.Tests;

/// <summary>
/// Тесты для Раздела 3: Наследование и protected (Задания 16-20).
/// </summary>
public class Section3_InheritanceAndProtectedTests
{
    // Задание 16: Базовый класс Person с protected полями _name, _surname, _age
    [Fact]
    public void Task16_PersonClass_ShouldHaveProtectedFields()
    {
        var nameField = typeof(Person).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        var surnameField = typeof(Person).GetField("_surname", BindingFlags.NonPublic | BindingFlags.Instance);
        var ageField = typeof(Person).GetField("_age", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(nameField);
        Assert.True(nameField.IsFamily, "Поле _name должно быть protected (Family).");

        Assert.NotNull(surnameField);
        Assert.True(surnameField.IsFamily, "Поле _surname должно быть protected (Family).");

        Assert.NotNull(ageField);
        Assert.True(ageField.IsFamily, "Поле _age должно быть protected (Family).");
    }

    // Задание 17: Наследование класса Student от Person
    [Fact]
    public void Task17_Student_ShouldInheritFromPerson()
    {
        Assert.True(typeof(Person).IsAssignableFrom(typeof(Student)), 
            "Класс Student должен наследоваться от Person.");

        // Проверяем, что в Student нет дублирующих полей _name, _surname, _age (они только в Person)
        var studentOwnFields = typeof(Student).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.DoesNotContain(studentOwnFields, f => f.Name == "_name");
        Assert.DoesNotContain(studentOwnFields, f => f.Name == "_surname");
        Assert.DoesNotContain(studentOwnFields, f => f.Name == "_age");
    }

    // Задание 18: Наследование Teacher от Person и метод GetTeacherInfo()
    [Fact]
    public void Task18_Teacher_ShouldInheritFromPerson_AndHaveGetTeacherInfo()
    {
        Assert.True(typeof(Person).IsAssignableFrom(typeof(Teacher)), 
            "Класс Teacher должен наследоваться от Person.");

        var teacher = new Teacher(1, "Александр", "Смирнов", 45, "C#", 20);
        string info = teacher.GetTeacherInfo();

        Assert.Contains("Александр", info);
        Assert.Contains("Смирнов", info);
        Assert.Contains("C#", info);
        Assert.Contains("20", info);
    }

    // Задание 19: Защищённый метод DisplayBasicInfo() в Person, используемый в Student и Teacher
    [Fact]
    public void Task19_DisplayBasicInfo_ShouldBeProtected_AndUsedInDescendants()
    {
        var method = typeof(Person).GetMethod("DisplayBasicInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);
        Assert.True(method.IsFamily, "Метод DisplayBasicInfo() должен быть protected (Family).");

        var student = new Student(10, "Иван", "Петров", 19, "ИС-21");
        var teacher = new Teacher(20, "Елена", "Кузнецова", 38, "БД", 14);

        // Оба метода GetStudentInfo и GetTeacherInfo включают базовую информацию от DisplayBasicInfo
        Assert.Contains("ФИО: Иван Петров", student.GetStudentInfo());
        Assert.Contains("ФИО: Елена Кузнецова", teacher.GetTeacherInfo());
    }

    // Задание 20: Ограничение доступа к данным базового класса
    [Fact]
    public void Task20_DataAccessibility_ShouldEnforcePrivateVsProtected()
    {
        // Приватное поле _internalPersonCode в Person недоступно в Student напрямую
        var privateFieldInBase = typeof(Person).GetField("_internalPersonCode", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(privateFieldInBase);
        Assert.True(privateFieldInBase.IsPrivate, "Поле _internalPersonCode должно быть private в Person.");

        // Студент не может получить доступ к private полю базового класса без публичного метода самого Person
        var student = new Student(1, "Максим", "Волков", 18, "ВЕБ-11");
        string auditCode = student.ExposeInternalCodeForAudit();
        Assert.False(string.IsNullOrWhiteSpace(auditCode));
    }
}
