using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

public class TeacherController : Controller
{
    private readonly CollegeDbContext _context;

    public TeacherController(CollegeDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Страница преподавателей (Задание 30, 9, 18)
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var teachers = await _context.Teachers.ToListAsync();
        return View(teachers);
    }

    /// <summary>
    /// Детальная страница преподавателя
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
            return NotFound();

        // Задание 18: отображение через GetTeacherInfo()
        ViewBag.TeacherInfo = teacher.GetTeacherInfo();

        // Задание 23: protected internal GetFullName()
        ViewBag.FullName = teacher.GetFullName();

        // Задание 25: private protected CalculateStatistics()
        ViewBag.InternalStatistics = teacher.GetInternalStatistics();

        // Предметы преподавателя
        ViewBag.Subjects = await _context.Subjects.Where(s => s.TeacherId == id).ToListAsync();

        return View(teacher);
    }

    /// <summary>
    /// Добавление преподавателя (Задание 9)
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string surname, int age, string subject, int experience)
    {
        try
        {
            var teacher = new Teacher(name, surname, age, subject, experience);
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Преподаватель {teacher.GetFullName()} успешно добавлен!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }

    /// <summary>
    /// Задание 10: Обновление стажа только через UpdateExperience()
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> UpdateExperience(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
            return NotFound();

        return View(teacher);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateExperience(int id, int experience)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
            return NotFound();

        try
        {
            // Задание 10: вызов специального метода UpdateExperience()
            teacher.UpdateExperience(experience);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Стаж преподавателя {teacher.GetFullName()} успешно обновлён до {experience} лет.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("Experience", ex.Message);
            return View(teacher);
        }
    }

    /// <summary>
    /// Удаление преподавателя
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher != null)
        {
            // Снятие с предметов
            var subjects = await _context.Subjects.Where(s => s.TeacherId == id).ToListAsync();
            foreach (var s in subjects)
            {
                s.RemoveTeacher();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Преподаватель успешно удален.";
        }

        return RedirectToAction(nameof(Index));
    }
}
