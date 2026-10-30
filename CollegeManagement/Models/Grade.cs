namespace CollegeManagement.Models;

/// <summary>
/// Сущность оценки для электронного журнала.
/// </summary>
public class Grade
{
    private int _value;

    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Оценка от 2 до 5
    /// </summary>
    public int Value
    {
        get => _value;
        set
        {
            if (value < 2 || value > 5)
                throw new ArgumentOutOfRangeException(nameof(value), "Оценка должна быть в диапазоне от 2 до 5.");
            _value = value;
        }
    }

    public Grade()
    {
    }

    public Grade(int id, int studentId, string studentName, int subjectId, string subjectName, int value, DateTime date, string comment = "")
    {
        Id = id;
        StudentId = studentId;
        StudentName = studentName?.Trim() ?? string.Empty;
        SubjectId = subjectId;
        SubjectName = subjectName?.Trim() ?? string.Empty;
        Value = value; // Проверяется сеттером
        Date = date;
        Comment = comment?.Trim() ?? string.Empty;
    }

    public Grade(int studentId, string studentName, int subjectId, string subjectName, int value, DateTime date, string comment = "")
        : this(0, studentId, studentName, subjectId, subjectName, value, date, comment)
    {
    }
}

/// <summary>
/// Задание 28.
/// Класс GradeBook (электронный журнал оценок).
/// Список оценок закрыт (private).
/// Добавление и удаление через публичные методы с валидацией 2-5.
/// </summary>
public class GradeBook
{
    // Задание 28: закрытый список оценок
    private readonly List<Grade> _grades = new();

    /// <summary>
    /// Доступ только для чтения
    /// </summary>
    public IReadOnlyList<Grade> Grades => _grades.AsReadOnly();

    public GradeBook()
    {
    }

    public GradeBook(IEnumerable<Grade> initialGrades)
    {
        if (initialGrades != null)
        {
            foreach (var g in initialGrades)
            {
                AddGrade(g);
            }
        }
    }

    /// <summary>
    /// Задание 28: публичный метод добавления оценки с валидацией от 2 до 5.
    /// </summary>
    public bool AddGrade(Grade grade)
    {
        if (grade == null)
            throw new ArgumentNullException(nameof(grade), "Оценка не может быть null.");

        if (grade.Value < 2 || grade.Value > 5)
            throw new ArgumentOutOfRangeException(nameof(grade.Value), "Оценка должна быть в пределах от 2 до 5.");

        _grades.Add(grade);
        return true;
    }

    public bool AddGrade(int studentId, string studentName, int subjectId, string subjectName, int value, string comment = "")
    {
        var grade = new Grade(studentId, studentName, subjectId, subjectName, value, DateTime.Today, comment);
        return AddGrade(grade);
    }

    /// <summary>
    /// Задание 28: удаление оценки по идентификатору.
    /// </summary>
    public bool RemoveGrade(int gradeId)
    {
        var grade = _grades.FirstOrDefault(g => g.Id == gradeId);
        if (grade == null)
            return false;

        return _grades.Remove(grade);
    }

    public bool RemoveGrade(Grade grade)
    {
        if (grade == null)
            return false;

        return _grades.Remove(grade);
    }

    /// <summary>
    /// Получение среднего балла студента
    /// </summary>
    public double GetAverageGradeForStudent(int studentId)
    {
        var studentGrades = _grades.Where(g => g.StudentId == studentId).ToList();
        return studentGrades.Count > 0 ? studentGrades.Average(g => g.Value) : 0.0;
    }

    /// <summary>
    /// Получение среднего балла по предмету
    /// </summary>
    public double GetAverageGradeForSubject(int subjectId)
    {
        var subjectGrades = _grades.Where(g => g.SubjectId == subjectId).ToList();
        return subjectGrades.Count > 0 ? subjectGrades.Average(g => g.Value) : 0.0;
    }
}
