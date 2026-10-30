using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

public class StudentController : Controller
{
    private readonly CollegeDbContext _context;
    private readonly StudentService _studentService;

    public StudentController(CollegeDbContext context, StudentService studentService)
    {
        _context = context;
        _studentService = studentService;
    }

    /// <summary>
    /// Страница со списком студентов (Задание 30)
    /// </summary>
    public async Task<IActionResult> Index(string? searchGroup)
    {
        var query = _context.Students.Include(s => s.GroupNavigation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchGroup))
        {
            query = query.Where(s => s.Group.Contains(searchGroup.Trim()));
        }

        var students = await query.ToListAsync();
        ViewBag.Groups = await _context.Groups.Select(g => g.Name).Distinct().ToListAsync();
        ViewBag.SearchGroup = searchGroup;

        return View(students);
    }

    /// <summary>
    /// Детальная информация о студенте с демонстрацией вызовов методов инкапсуляции
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
            return NotFound();

        // Задание 7, 19: GetStudentInfo()
        ViewBag.StudentInfo = student.GetStudentInfo();

        // Задание 23: protected internal метод GetFullName()
        ViewBag.FullName = student.GetFullName();

        // Задание 25: private protected метод через публичную обертку
        ViewBag.InternalStatistics = student.GetInternalStatistics();

        // Задание 20: доступ к коду аудита Person
        ViewBag.AuditCode = student.ExposeInternalCodeForAudit();

        // Оценки студента
        ViewBag.Grades = await _context.Grades.Where(g => g.StudentId == id).ToListAsync();

        // Посещаемость
        ViewBag.Attendance = await _context.AttendanceRecords.Where(a => a.StudentId == id).ToListAsync();

        return View(student);
    }

    /// <summary>
    /// Создание студента (Задание 1-6)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.AvailableGroups = await _context.Groups.ToListAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string surname, int age, string group)
    {
        try
        {
            // Задание 5, 6: Конструктор выполняет ValidateStudentData() и SetAge() (15-70 лет)
            await _studentService.CreateStudentAsync(name, surname, age, group);
            TempData["SuccessMessage"] = $"Студент {name} {surname} успешно добавлен в группу {group}!";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ModelState.AddModelError("Age", ex.Message);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Ошибка при создании: {ex.Message}");
        }

        ViewBag.AvailableGroups = await _context.Groups.ToListAsync();
        return View();
    }

    /// <summary>
    /// Задание 4: Изменение возраста студента через метод SetAge()
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> EditAge(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAge(int id, int age)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFound();

        try
        {
            // Задание 4: вызов метода SetAge(), проверяющего 15-70 лет
            student.SetAge(age);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Возраст студента {student.GetFullName()} успешно обновлён до {age} лет через метод SetAge().";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ModelState.AddModelError("Age", ex.Message);
            return View(student);
        }
    }

    /// <summary>
    /// Удаление студента
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        bool success = await _studentService.DeleteStudentAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "Студент успешно удален.";
        }
        else
        {
            TempData["ErrorMessage"] = "Студент не найден.";
        }

        return RedirectToAction(nameof(Index));
    }
}
