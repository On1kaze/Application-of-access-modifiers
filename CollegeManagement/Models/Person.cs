namespace CollegeManagement.Models;

/// <summary>
/// Задание 16, 19, 20, 23, 25.
/// Базовый класс Person, демонстрирующий использование модификаторов доступа:
/// protected, protected internal, private protected, private.
/// </summary>
public abstract class Person
{
    // Задание 16: protected поля, доступные классам-наследникам
    protected string _name = string.Empty;
    protected string _surname = string.Empty;
    protected int _age;

    // Задание 20: private поле, доступное ТОЛЬКО внутри базового класса Person
    private readonly string _internalPersonCode = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

    public int Id { get; protected set; }

    // Публичные свойства с безопасным доступом к защищённым полям
    public virtual string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }

    public virtual string Surname
    {
        get => _surname;
        set => _surname = value?.Trim() ?? string.Empty;
    }

    public virtual int Age
    {
        get => _age;
        protected set => _age = value;
    }

    protected Person()
    {
    }

    protected Person(int id, string name, string surname, int age)
    {
        Id = id;
        _name = name?.Trim() ?? string.Empty;
        _surname = surname?.Trim() ?? string.Empty;
        _age = age;
    }

    /// <summary>
    /// Задание 19: защищённый метод DisplayBasicInfo(), доступный наследникам.
    /// </summary>
    protected virtual string DisplayBasicInfo()
    {
        return $"ID: {Id}, ФИО: {_name} {_surname}, Возраст: {_age}";
    }

    /// <summary>
    /// Задание 23: protected internal метод GetFullName().
    /// Доступен внутри текущей сборки и во всех классах-наследниках (даже в других сборках).
    /// </summary>
    protected internal virtual string GetFullName()
    {
        return $"{_name} {_surname}".Trim();
    }

    /// <summary>
    /// Задание 25: private protected метод CalculateStatistics().
    /// Доступен наследникам ТОЛЬКО внутри текущей сборки.
    /// </summary>
    private protected virtual string CalculateStatistics()
    {
        return $"Базовая статистика для {_name} {_surname} (ID: {Id})";
    }

    /// <summary>
    /// Задание 20: приватный метод базового класса, недоступный наследникам напрямую.
    /// </summary>
    private string GetInternalPersonCode()
    {
        return _internalPersonCode;
    }

    /// <summary>
    /// Публичный метод проверки доступности приватного поля через метод самого базового класса.
    /// </summary>
    public string ExposeInternalCodeForAudit()
    {
        return GetInternalPersonCode();
    }
}
