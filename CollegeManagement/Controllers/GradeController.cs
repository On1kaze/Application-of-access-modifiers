using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

public class GradeController : Controller
{
    private readonly CollegeDbContext _context;

    public GradeController(CollegeDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Электронный журнал оценок (Задание 30, 28)
    /// </summary>
    public async Task<IActionResult> Index(int? studentId, int? subjectId)
    {
        var grades = await _context.Grades.OrderByDescending(g => g.Date).ToListAsync();

        // Задание 28: использование класса GradeBook с инкапсулированным списком
        var gradeBook = new GradeBook(grades);

        var query = gradeBook.Grades.AsEnumerable();
        if (studentId.HasValue && studentId.Value > 0)
        {
            query = query.Where(g => g.StudentId == studentId.Value);
        }
        if (subjectId.HasValue && subjectId.Value > 0)
        {
            query = query.Where(g => g.SubjectId == subjectId.Value);
        }

        ViewBag.Students = await _context.Students.ToListAsync();
        ViewBag.Subjects = await _context.Subjects.ToListAsync();
        ViewBag.SelectedStudentId = studentId;
        ViewBag.SelectedSubjectId = subjectId;

        // Средний балл
        var displayedGrades = query.ToList();
        ViewBag.AverageGrade = displayedGrades.Count > 0 ? displayedGrades.Average(g => g.Value) : 0.0;

        return View(displayedGrades);
    }

    /// <summary>
    /// Добавление оценки в журнал (Задание 28)
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
    public async Task<IActionResult> Add(int studentId, int subjectId, int value, string comment)
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

        try
        {
            // Задание 28: проверка допустимых значений от 2 до 5 через GradeBook и класс Grade
            var gradeBook = new GradeBook();
            var grade = new Grade(student.Id, student.GetFullName(), subject.Id, subject.Name, value, DateTime.Today, comment);

            gradeBook.AddGrade(grade); // Выполняет валидацию диапазона 2-5

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Оценка {value} по предмету «{subject.Name}» успешно выставлена студенту {student.GetFullName()}!";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ModelState.AddModelError("Value", ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }

        ViewBag.Students = await _context.Students.ToListAsync();
        ViewBag.Subjects = await _context.Subjects.ToListAsync();
        return View();
    }

    /// <summary>
    /// Удаление оценки из журнала через GradeBook (Задание 28)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var grade = await _context.Grades.FindAsync(id);
        if (grade != null)
        {
            var gradeBook = new GradeBook(new[] { grade });
            gradeBook.RemoveGrade(id); // Задание 28

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Оценка успешно удалена из журнала.";
        }

        return RedirectToAction(nameof(Index));
    }
}
