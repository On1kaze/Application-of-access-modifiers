namespace CollegeManagement.Models;

/// <summary>
/// Задания 11-13.
/// Класс Group (учебная группа) с инкапсулированным списком студентов.
/// </summary>
public class Group
{
    // Задание 11: закрытый список студентов
    private readonly List<Student> _students = new();

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Course { get; set; } = 1;

    /// <summary>
    /// Задание 11, 13: Публичный доступ к списку студентов только для чтения (IReadOnlyList).
    /// Прямая модификация (Add, Remove) извне невозможна.
    /// </summary>
    public IReadOnlyList<Student> Students => _students.AsReadOnly();

    public Group()
    {
    }

    public Group(string name, int course)
        : this(0, name, course)
    {
    }

    public Group(int id, string name, int course)
    {
        Id = id;
        Name = name?.Trim() ?? string.Empty;
        Course = course;
    }

    /// <summary>
    /// Задание 12: публичный метод AddStudent().
    /// Добавляет студента только если он не равен null и ещё не состоит в группе.
    /// </summary>
    public bool AddStudent(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student), "Студент не может быть null.");

        // Проверка: состоит ли уже в группе (по Id или полному совпадению ФИО)
        bool alreadyInGroup = _students.Any(s =>
            (s.StudentId != 0 && s.StudentId == student.StudentId) ||
            (s.Name.Equals(student.Name, StringComparison.OrdinalIgnoreCase) &&
             s.Surname.Equals(student.Surname, StringComparison.OrdinalIgnoreCase)));

        if (alreadyInGroup)
            return false;

        _students.Add(student);
        student.Group = this.Name;
        student.GroupId = this.Id;
        return true;
    }

    /// <summary>
    /// Задание 13: удаление студентов через метод RemoveStudent.
    /// Запрещает прямое удаление элементов из списка студентов извне класса.
    /// </summary>
    public bool RemoveStudent(int studentId)
    {
        var existing = _students.FirstOrDefault(s => s.StudentId == studentId);
        if (existing == null)
            return false;

        _students.Remove(existing);
        if (existing.GroupId == this.Id)
        {
            existing.GroupId = null;
        }
        return true;
    }

    public bool RemoveStudent(Student student)
    {
        if (student == null)
            return false;

        return RemoveStudent(student.StudentId);
    }

    /// <summary>
    /// Метод для загрузки списка из БД при необходимости
    /// </summary>
    public void LoadStudents(IEnumerable<Student> students)
    {
        _students.Clear();
        foreach (var s in students)
        {
            _students.Add(s);
        }
    }
}
