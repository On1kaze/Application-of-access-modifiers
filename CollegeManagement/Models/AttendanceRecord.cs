namespace CollegeManagement.Models;

public enum AttendanceStatus
{
    Present,    // Присутствовал
    Absent,     // Отсутствовал
    Excused,    // Уважительная причина
    Late        // Опоздал
}

/// <summary>
/// Сущность записи посещаемости занятия.
/// </summary>
public class AttendanceRecord
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today;

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    public string Note { get; set; } = string.Empty;

    public AttendanceRecord()
    {
    }

    public AttendanceRecord(int id, int studentId, string studentName, int subjectId, string subjectName, DateTime date, AttendanceStatus status, string note = "")
    {
        Id = id;
        StudentId = studentId;
        StudentName = studentName?.Trim() ?? string.Empty;
        SubjectId = subjectId;
        SubjectName = subjectName?.Trim() ?? string.Empty;
        Date = date;
        Status = status;
        Note = note?.Trim() ?? string.Empty;
    }
}
