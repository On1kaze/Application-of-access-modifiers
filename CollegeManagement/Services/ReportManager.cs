using CollegeManagement.Data;
using CollegeManagement.Models;

namespace CollegeManagement.Services;

/// <summary>
/// Задание 24.
/// Класс ReportManager с методом protected internal GenerateReport().
/// Метод доступен внутри текущей сборки (в том числе контроллерам и сервисам)
/// и классам-наследникам в других сборках.
/// </summary>
public class ReportManager
{
    /// <summary>
    /// Задание 24: protected internal метод генерации отчёта
    /// </summary>
    protected internal virtual string GenerateReport(string reportType, CollegeDbContext dbContext)
    {
        var settings = dbContext.CollegeSettings.FirstOrDefault() ?? new CollegeSettings();

        return reportType.ToLowerInvariant() switch
        {
            "students" => GenerateStudentsReport(dbContext, settings),
            "teachers" => GenerateTeachersReport(dbContext, settings),
            "grades" => GenerateGradesReport(dbContext, settings),
            "attendance" => GenerateAttendanceReport(dbContext, settings),
            _ => GenerateSummaryReport(dbContext, settings)
        };
    }

    private string GenerateSummaryReport(CollegeDbContext dbContext, CollegeSettings settings)
    {
        int studentsCount = dbContext.Students.Count();
        int teachersCount = dbContext.Teachers.Count();
        int groupsCount = dbContext.Groups.Count();
        int gradesCount = dbContext.Grades.Count();

        return $"=== СВОДНЫЙ ОТЧЁТ: {settings.CollegeName} ({settings.AcademicYear}) ===\n" +
               $"Всего студентов: {studentsCount}\n" +
               $"Преподавательский состав: {teachersCount} чел.\n" +
               $"Учебных групп: {groupsCount}\n" +
               $"Выставлено оценок: {gradesCount}\n" +
               $"Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm}";
    }

    private string GenerateStudentsReport(CollegeDbContext dbContext, CollegeSettings settings)
    {
        var students = dbContext.Students.ToList();
        var lines = students.Select(s => $"- {s.GetStudentInfo()}").ToList();

        return $"=== ОТЧЁТ ПО СТУДЕНТАМ: {settings.CollegeName} ===\n" +
               $"Всего студентов: {students.Count}\n" +
               string.Join("\n", lines);
    }

    private string GenerateTeachersReport(CollegeDbContext dbContext, CollegeSettings settings)
    {
        var teachers = dbContext.Teachers.ToList();
        var lines = teachers.Select(t => $"- {t.GetTeacherInfo()}").ToList();

        return $"=== ОТЧЁТ ПО ПРЕПОДАВАТЕЛЯМ: {settings.CollegeName} ===\n" +
               $"Всего преподавателей: {teachers.Count}\n" +
               string.Join("\n", lines);
    }

    private string GenerateGradesReport(CollegeDbContext dbContext, CollegeSettings settings)
    {
        var grades = dbContext.Grades.ToList();
        double avg = grades.Count > 0 ? grades.Average(g => g.Value) : 0.0;

        return $"=== УСПЕВАЕМОСТЬ: {settings.CollegeName} ===\n" +
               $"Всего оценок: {grades.Count}\n" +
               $"Средний балл по колледжу: {avg:F2}";
    }

    private string GenerateAttendanceReport(CollegeDbContext dbContext, CollegeSettings settings)
    {
        var records = dbContext.AttendanceRecords.ToList();
        int present = records.Count(r => r.Status == AttendanceStatus.Present);
        double rate = records.Count > 0 ? ((double)present / records.Count) * 100 : 100.0;

        return $"=== ОТЧЁТ ПО ПОСЕЩАЕМОСТИ: {settings.CollegeName} ===\n" +
               $"Всего записей: {records.Count}\n" +
               $"Процент посещаемости: {rate:F1}%";
    }

    /// <summary>
    /// Публичный фасад для вызова из представлений и внешних вызовов
    /// </summary>
    public string CreateReport(string reportType, CollegeDbContext dbContext)
    {
        return GenerateReport(reportType, dbContext);
    }
}
