namespace CollegeManagement.Models;

/// <summary>
/// Задания 14, 15.
/// Класс Subject (учебный предмет) с инкапсулированными полями и назначением преподавателя.
/// </summary>
public class Subject
{
    // Задание 14: закрытые поля для названия, количества часов и преподавателя
    private string _name = string.Empty;
    private int _hours;
    private Teacher? _teacher;

    public int Id { get; set; }

    public int? TeacherId { get; set; }

    /// <summary>
    /// Название предмета
    /// </summary>
    public string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Количество часов
    /// </summary>
    public int Hours
    {
        get => _hours;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Количество часов не может быть отрицательным.");
            _hours = value;
        }
    }

    /// <summary>
    /// Преподаватель предмета (доступ на чтение)
    /// </summary>
    public Teacher? Teacher => _teacher;

    public Subject()
    {
    }

    public Subject(int id, string name, int hours)
    {
        Id = id;
        _name = name?.Trim() ?? string.Empty;
        Hours = hours;
    }

    public Subject(string name, int hours)
        : this(0, name, hours)
    {
    }

    /// <summary>
    /// Задание 15: метод AssignTeacher(), назначающий преподавателя на предмет.
    /// Проверяет, что преподаватель не равен null.
    /// </summary>
    public void AssignTeacher(Teacher teacher)
    {
        if (teacher == null)
            throw new ArgumentNullException(nameof(teacher), "Преподаватель не может быть null.");

        _teacher = teacher;
        TeacherId = teacher.Id;
    }

    /// <summary>
    /// Снятие преподавателя с дисциплины
    /// </summary>
    public void RemoveTeacher()
    {
        _teacher = null;
        TeacherId = null;
    }

    /// <summary>
    /// Публичный метод для получения информации о предмете
    /// </summary>
    public string GetSubjectInfo()
    {
        string teacherName = _teacher != null ? _teacher.GetFullName() : "Не назначен";
        return $"Предмет: {_name}, Часов: {_hours}, Преподаватель: {teacherName}";
    }
}
