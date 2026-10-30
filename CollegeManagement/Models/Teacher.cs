namespace CollegeManagement.Models;

/// <summary>
/// Задания 9, 10, 18, 19, 23.
/// Класс Teacher, наследующий Person.
/// Демонстрирует инкапсуляцию стажа и отображение информации через protected-метод базового класса.
/// </summary>
public class Teacher : Person
{
    // Задание 9: закрытые поля для предмета и стажа (имя, фамилия, возраст унаследованы от Person)
    private string _subject = string.Empty;
    private int _experience;

    /// <summary>
    /// Предмет преподавателя (публичное свойство с проверкой)
    /// </summary>
    public string Subject
    {
        get => _subject;
        set => _subject = value?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Задание 10: Свойство Experience с закрытым сеттером.
    /// Изменять стаж можно ТОЛЬКО через метод UpdateExperience().
    /// </summary>
    public int Experience
    {
        get => _experience;
        private set => _experience = value;
    }

    public Teacher()
    {
    }

    public Teacher(int id, string name, string surname, int age, string subject, int experience)
        : base(id, name, surname, age)
    {
        _subject = subject?.Trim() ?? string.Empty;
        UpdateExperience(experience);
    }

    public Teacher(string name, string surname, int age, string subject, int experience)
        : this(0, name, surname, age, subject, experience)
    {
    }

    /// <summary>
    /// Задание 10: специальный метод для обновления стажа работы.
    /// Проверяет корректность стажа (стаж >= 0 и не может превышать возраст - 18).
    /// </summary>
    public void UpdateExperience(int newExperience)
    {
        if (newExperience < 0)
            throw new ArgumentOutOfRangeException(nameof(newExperience), "Стаж работы не может быть отрицательным.");

        if (_age > 0 && newExperience > (_age - 18))
            throw new ArgumentException("Стаж работы не может превышать трудоспособный возраст преподавателя.");

        _experience = newExperience;
    }

    /// <summary>
    /// Задание 18, 19: отдельный метод для отображения информации о преподавателе,
    /// использующий защищённый метод базового класса DisplayBasicInfo().
    /// </summary>
    public string GetTeacherInfo()
    {
        string baseInfo = DisplayBasicInfo();
        return $"{baseInfo}, Предмет: {_subject}, Стаж: {_experience} лет";
    }

    /// <summary>
    /// Задание 25: переопределение private protected метода внутри сборки
    /// </summary>
    private protected override string CalculateStatistics()
    {
        return $"Преподаватель: {GetFullName()}, Предмет: {_subject}, Стаж: {_experience} лет";
    }

    public string GetInternalStatistics()
    {
        return CalculateStatistics();
    }
}
