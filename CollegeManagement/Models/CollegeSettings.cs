namespace CollegeManagement.Models;

/// <summary>
/// Задание 21, 22.
/// Класс настроек колледжа с модификатором internal.
/// Методы изменения настроек доступны только внутри текущей сборки.
/// </summary>
internal class CollegeSettings
{
    public int Id { get; set; } = 1;

    public string CollegeName { get; private set; } = "Политехнический колледж";

    public string AcademicYear { get; private set; } = "2026-2027";

    public string Address { get; private set; } = "ул. Университетская, д. 10";

    public string PhoneNumber { get; private set; } = "+7 (800) 555-35-35";

    public CollegeSettings()
    {
    }

    public CollegeSettings(string collegeName, string academicYear)
    {
        CollegeName = collegeName?.Trim() ?? string.Empty;
        AcademicYear = academicYear?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Задание 22: методы изменения настроек доступны только внутри текущей сборки с помощью internal.
    /// </summary>
    internal void UpdateSettings(string collegeName, string academicYear)
    {
        if (string.IsNullOrWhiteSpace(collegeName))
            throw new ArgumentException("Название колледжа не может быть пустым.", nameof(collegeName));

        if (string.IsNullOrWhiteSpace(academicYear))
            throw new ArgumentException("Учебный год не может быть пустым.", nameof(academicYear));

        CollegeName = collegeName.Trim();
        AcademicYear = academicYear.Trim();
    }

    internal void UpdateContacts(string address, string phone)
    {
        if (!string.IsNullOrWhiteSpace(address))
            Address = address.Trim();

        if (!string.IsNullOrWhiteSpace(phone))
            PhoneNumber = phone.Trim();
    }

    internal string GetSummary()
    {
        return $"{CollegeName} | Учебный год: {AcademicYear}";
    }
}
