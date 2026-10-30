using System.Reflection;
using CollegeManagement.Models;
using Xunit;

namespace CollegeManagement.Tests;

/// <summary>
/// Тесты для Раздела 2: Управление преподавателями и группами (Задания 9-15).
/// </summary>
public class Section2_TeachersAndGroupsTests
{
    // Задание 9: Класс Teacher с закрытыми полями и публичными свойствами
    [Fact]
    public void Task9_Teacher_ShouldHaveEncapsulatedFieldsAndPublicProperties()
    {
        var teacher = new Teacher("Елена", "Кузнецова", 40, "Базы данных", 15);

        Assert.Equal("Елена", teacher.Name);
        Assert.Equal("Кузнецова", teacher.Surname);
        Assert.Equal(40, teacher.Age);
        Assert.Equal("Базы данных", teacher.Subject);
        Assert.Equal(15, teacher.Experience);
    }

    // Задание 10: Ограничение изменения стажа: только через UpdateExperience()
    [Fact]
    public void Task10_Experience_ShouldOnlyBeModifiedViaUpdateExperience()
    {
        var property = typeof(Teacher).GetProperty("Experience");
        Assert.NotNull(property);
        Assert.True(property.SetMethod == null || !property.SetMethod.IsPublic, 
            "Свойство Experience не должно иметь публичного сеттера.");

        var teacher = new Teacher("Александр", "Смирнов", 45, "C#", 20);

        // Корректное обновление стажа
        teacher.UpdateExperience(21);
        Assert.Equal(21, teacher.Experience);

        // Отрицательный стаж недопустим
        Assert.Throws<ArgumentOutOfRangeException>(() => teacher.UpdateExperience(-1));

        // Стаж, превышающий возраст - 18
        Assert.Throws<ArgumentException>(() => teacher.UpdateExperience(50));
    }

    // Задание 11: Класс Group с закрытым списком студентов
    [Fact]
    public void Task11_Group_StudentsList_ShouldBePrivateAndExposedAsReadOnly()
    {
        var field = typeof(Group).GetField("_students", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(field);
        Assert.True(field.IsPrivate, "Поле _students должно быть private.");

        var group = new Group("ИС-21", 2);
        Assert.NotNull(group.Students);
        Assert.IsAssignableFrom<IReadOnlyCollection<Student>>(group.Students);
    }

    // Задание 12: AddStudent() добавляет только если студент ещё не состоит в группе
    [Fact]
    public void Task12_AddStudent_ShouldPreventDuplicates()
    {
        var group = new Group(1, "ИС-21", 2);
        var s1 = new Student(101, "Иван", "Петров", 19, "ИС-21");
        var s2 = new Student(102, "Анна", "Сидорова", 20, "ИС-21");

        // Первое добавление - успешно
        Assert.True(group.AddStudent(s1));
        Assert.True(group.AddStudent(s2));
        Assert.Equal(2, group.Students.Count);

        // Повторное добавление того же студента - отклонено
        Assert.False(group.AddStudent(s1));
        Assert.Equal(2, group.Students.Count);

        // Попытка передать null
        Assert.Throws<ArgumentNullException>(() => group.AddStudent(null!));
    }

    // Задание 13: RemoveStudent() безопасное удаление без прямого доступа к коллекции
    [Fact]
    public void Task13_RemoveStudent_ShouldSafelyRemoveFromGroup()
    {
        var group = new Group(1, "ИС-21", 2);
        var s1 = new Student(101, "Иван", "Петров", 19, "ИС-21");
        group.AddStudent(s1);
        Assert.Single(group.Students);

        // Удаление существующего студента
        Assert.True(group.RemoveStudent(101));
        Assert.Empty(group.Students);

        // Удаление несуществующего студента
        Assert.False(group.RemoveStudent(999));
    }

    // Задание 14: Класс Subject с закрытыми полями для названия, часов и преподавателя
    [Fact]
    public void Task14_Subject_ShouldHavePrivateFields()
    {
        var nameField = typeof(Subject).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        var hoursField = typeof(Subject).GetField("_hours", BindingFlags.NonPublic | BindingFlags.Instance);
        var teacherField = typeof(Subject).GetField("_teacher", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(nameField);
        Assert.True(nameField.IsPrivate, "Поле _name должно быть private.");
        Assert.NotNull(hoursField);
        Assert.True(hoursField.IsPrivate, "Поле _hours должно быть private.");
        Assert.NotNull(teacherField);
        Assert.True(teacherField.IsPrivate, "Поле _teacher должно быть private.");
    }

    // Задание 15: AssignTeacher() назначает преподавателя и проверяет not null
    [Fact]
    public void Task15_AssignTeacher_ShouldCheckNotNullAndAssign()
    {
        var subject = new Subject("Программирование на C#", 120);
        var teacher = new Teacher("Александр", "Смирнов", 45, "C#", 20);

        subject.AssignTeacher(teacher);
        Assert.NotNull(subject.Teacher);
        Assert.Equal("Александр", subject.Teacher.Name);

        // Проверка защиты от null
        Assert.Throws<ArgumentNullException>(() => subject.AssignTeacher(null!));
    }
}
