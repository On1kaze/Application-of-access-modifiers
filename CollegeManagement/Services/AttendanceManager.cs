using CollegeManagement.Models;

namespace CollegeManagement.Services;

/// <summary>
/// Статистика посещаемости
/// </summary>
public class AttendanceStatistics
{
    public int TotalClasses { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public int ExcusedCount { get; set; }
    public int LateCount { get; set; }
    public double AttendancePercentage => TotalClasses > 0 
        ? Math.Round(((double)(PresentCount + LateCount) / TotalClasses) * 100, 1) 
        : 100.0;
}

/// <summary>
/// Задание 29.
/// Система посещаемости. Внутренние данные о посещаемости закрыты (private).
/// Публичные методы добавления, изменения и получения статистики посещаемости.
/// </summary>
public class AttendanceManager
{
    // Задание 29: закрытые внутренние данные о посещаемости
    private readonly List<AttendanceRecord> _records = new();

    public IReadOnlyList<AttendanceRecord> Records => _records.AsReadOnly();

    public AttendanceManager()
    {
    }

    public AttendanceManager(IEnumerable<AttendanceRecord> records)
    {
        if (records != null)
        {
            _records.AddRange(records);
        }
    }

    /// <summary>
    /// Метод добавления записи посещаемости
    /// </summary>
    public void AddRecord(AttendanceRecord record)
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record), "Запись посещаемости не может быть null.");

        _records.Add(record);
    }

    /// <summary>
    /// Метод изменения статуса посещаемости
    /// </summary>
    public bool UpdateRecord(int recordId, AttendanceStatus newStatus, string? note = null)
    {
        var record = _records.FirstOrDefault(r => r.Id == recordId);
        if (record == null)
            return false;

        record.Status = newStatus;
        if (note != null)
        {
            record.Note = note.Trim();
        }
        return true;
    }

    /// <summary>
    /// Метод получения статистики посещаемости (для всех или конкретного студента)
    /// </summary>
    public AttendanceStatistics GetAttendanceStatistics(int? studentId = null)
    {
        var query = studentId.HasValue 
            ? _records.Where(r => r.StudentId == studentId.Value) 
            : _records.AsEnumerable();

        var list = query.ToList();
        return new AttendanceStatistics
        {
            TotalClasses = list.Count,
            PresentCount = list.Count(r => r.Status == AttendanceStatus.Present),
            AbsentCount = list.Count(r => r.Status == AttendanceStatus.Absent),
            ExcusedCount = list.Count(r => r.Status == AttendanceStatus.Excused),
            LateCount = list.Count(r => r.Status == AttendanceStatus.Late)
        };
    }

    public void LoadRecords(IEnumerable<AttendanceRecord> records)
    {
        _records.Clear();
        if (records != null)
        {
            _records.AddRange(records);
        }
    }
}
