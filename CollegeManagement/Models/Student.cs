namespace CollegeManagement.Models;

/// <summary>
/// Задания 1-8, 17, 19, 25.
/// Класс Student с демонстрацией модификаторов public, private, protected, private protected.
/// Наследует Person (задание 17).
/// </summary>
public class Student : Person
{
    // Задание 1: поле группы (также может ссылаться на класс Group и строковое название)
    private string _group = string.Empty;

    // Ссылка на сущность Group для связей EF Core
    public int? GroupId { get; set; }
    public virtual Group? GroupNavigation { get; set; }

    /// <summary>
    /// Задание 8: Идентификатор студента StudentId доступен только для чтения после создания объекта.
    /// Синхронизирован с базовым Id.
    /// </summary>
    public int StudentId
    {
        get => Id;
        private set => Id = value;
    }

    /// <summary>
    /// Задание 3: свойство Name с безопасной валидацией
    /// </summary>
    public override string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Имя студента не может быть пустым.", nameof(value));
            _name = value.Trim();
        }
    }

    /// <summary>
    /// Задание 3: свойство Surname с безопасной валидацией
    /// </summary>
    public override string Surname
    {
        get => _surname;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Фамилия студента не может быть пустой.", nameof(value));
            _surname = value.Trim();
        }
    }

    /// <summary>
    /// Задание 3, 4: свойство Age. Изменение только в пределах от 15 до 70 лет.
    /// </summary>
    public override int Age
    {
        get => _age;
        protected set => SetAge(value);
    }

    /// <summary>
    /// Свойство Group с валидацией
    /// </summary>
    public string Group
    {
        get => _group;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Группа не может быть пустой.", nameof(value));
            _group = value.Trim();
        }
    }

    /// <summary>
    /// Конструктор по умолчанию для EF Core / сериализации
    /// </summary>
    public Student()
    {
    }

    /// <summary>
    /// Задание 5: публичный конструктор, принимающий имя, фамилию, возраст и группу.
    /// Вызывает валидацию (Задание 6).
    /// </summary>
    public Student(string name, string surname, int age, string group)
        : this(0, name, surname, age, group)
    {
    }

    /// <summary>
    /// Перегруженный конструктор с идентификатором
    /// </summary>
    public Student(int id, string name, string surname, int age, string group)
        : base(id, name, surname, age)
    {
        _group = group?.Trim() ?? string.Empty;
        ValidateStudentData();
    }

    /// <summary>
    /// Задание 4: метод SetAge(), разрешающий устанавливать возраст только от 15 до 70 лет.
    /// </summary>
    public bool SetAge(int age)
    {
        if (age < 15 || age > 70)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Возраст студента должен быть от 15 до 70 лет.");
        }
        _age = age;
        return true;
    }

    /// <summary>
    /// Задание 2: публичные методы для получения персональных данных студента.
    /// </summary>
    public string GetName() => _name;
    public string GetSurname() => _surname;
    public int GetAge() => _age;
    public string GetGroup() => _group;

    /// <summary>
    /// Задание 6: приватный метод проверки введённых данных.
    /// </summary>
    private void ValidateStudentData()
    {
        if (string.IsNullOrWhiteSpace(_name))
            throw new ArgumentException("Имя студента обязательно для заполнения.");
        if (string.IsNullOrWhiteSpace(_surname))
            throw new ArgumentException("Фамилия студента обязательна для заполнения.");
        if (_age < 15 || _age > 70)
            throw new ArgumentOutOfRangeException(nameof(_age), "Возраст студента должен быть от 15 до 70 лет.");
        if (string.IsNullOrWhiteSpace(_group))
            throw new ArgumentException("Группа обязательна для заполнения.");
    }

    /// <summary>
    /// Задание 7, 19: публичный метод GetStudentInfo(), использующий protected-метод базового класса DisplayBasicInfo().
    /// </summary>
    public string GetStudentInfo()
    {
        // Использование protected-метода DisplayBasicInfo() из Person
        string baseInfo = DisplayBasicInfo();
        return $"{baseInfo}, Группа: {_group}";
    }

    /// <summary>
    /// Задание 25: переопределение private protected метода внутри той же сборки.
    /// </summary>
    private protected override string CalculateStatistics()
    {
        return $"Студент: {GetFullName()}, Группа: {_group}, Возраст: {_age}";
    }

    /// <summary>
    /// Публичная обертка для демонстрации вызова private protected метода внутри сборки
    /// </summary>
    public string GetInternalStatistics()
    {
        return CalculateStatistics();
    }
}
