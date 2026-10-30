using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

public class AttendanceController : Controller
{
    private readonly CollegeDbContext _context;
    private readonly AttendanceManager _attendanceManager;

    public AttendanceController(CollegeDbContext context, AttendanceManager attendanceManager)
    {
        _context = context;
        _attendanceManager = attendanceManager;
    }

    /// <summary>
    /// Страница посещаемости (Задание 30, 29)
    /// </summary>
    public async Task<IActionResult> Index(int? studentId, int? subjectId)
    {
        var records = await _context.AttendanceRecords.OrderByDescending(a => a.Date).ToListAsync();

        // Задание 29: загрузка в AttendanceManager с закрытыми данными
        _attendanceManager.LoadRecords(records);

        var query = _attendanceManager.Records.AsEnumerable();
        if (studentId.HasValue && studentId.Value > 0)
        {
            query = query.Where(r => r.StudentId == studentId.Value);
        }
        if (subjectId.HasValue && subjectId.Value > 0)
        {
            query = query.Where(r => r.SubjectId == subjectId.Value);
        }

        // Задание 29: расчет статистики посещаемости через метод GetAttendanceStatistics()
        var stats = _attendanceManager.GetAttendanceStatistics(studentId);

        ViewBag.Stats = stats;
        ViewBag.Students = await _context.Students.ToListAsync();
        ViewBag.Subjects = await _context.Subjects.ToListAsync();
        ViewBag.SelectedStudentId = studentId;
        ViewBag.SelectedSubjectId = subjectId;

        return View(query.ToList());
    }

    /// <summary>
    /// Добавление отметки посещаемости (Задание 29)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        ViewBag.Students = await _context.Students.ToListAsync();
        ViewBag.Subjects = await _context.Subjects.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int studentId, int subjectId, AttendanceStatus status, string note, DateTime date)
    {
        var student = await _context.Students.FindAsync(studentId);
        var subject = await _context.Subjects.FindAsync(subjectId);

        if (student == null || subject == null)
        {
            ModelState.AddModelError("", "Студент или предмет не найден.");
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Subjects = await _context.Subjects.ToListAsync();
            return View();
        }

        var record = new AttendanceRecord(0, student.Id, student.GetFullName(), subject.Id, subject.Name, date, status, note);

        // Задание 29: добавление через AttendanceManager
        _attendanceManager.AddRecord(record);

        _context.AttendanceRecords.Add(record);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Отметка посещаемости для {student.GetFullName()} успешно сохранена!";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Обновление статуса посещаемости (Задание 29)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int recordId, AttendanceStatus status, string? note)
    {
        var record = await _context.AttendanceRecords.FindAsync(recordId);
        if (record != null)
        {
            _attendanceManager.LoadRecords(new[] { record });
            _attendanceManager.UpdateRecord(recordId, status, note);

            record.Status = status;
            if (note != null) record.Note = note;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Статус посещаемости успешно обновлен!";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record != null)
        {
            _context.AttendanceRecords.Remove(record);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Запись посещаемости удалена.";
        }

        return RedirectToAction(nameof(Index));
    }
}
