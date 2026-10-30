using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

public class GroupController : Controller
{
    private readonly CollegeDbContext _context;

    public GroupController(CollegeDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Страница учебных групп (Задание 30, 11)
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var groups = await _context.Groups.Include(g => g.Students).ToListAsync();
        return View(groups);
    }

    /// <summary>
    /// Просмотр группы и закрытого списка студентов через IReadOnlyList (Задание 11)
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var group = await _context.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
            return NotFound();

        // Студенты, которые ещё не состоят в этой группе (для добавления)
        ViewBag.AvailableStudents = await _context.Students
            .Where(s => s.GroupId != id)
            .ToListAsync();

        return View(group);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, int course)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("Name", "Название группы обязательно.");
            return View();
        }

        var group = new Group(name, course);
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Группа {group.Name} успешно создана!";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Задание 12: Добавление студента в группу через публичный метод AddStudent()
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStudent(int groupId, int studentId)
    {
        var group = await _context.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == groupId);
        var student = await _context.Students.FindAsync(studentId);

        if (group == null || student == null)
            return NotFound();

        // Задание 12: вызов метода AddStudent с проверкой на дублирование
        bool added = group.AddStudent(student);
        if (added)
        {
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Студент {student.GetFullName()} успешно добавлен в группу {group.Name}!";
        }
        else
        {
            TempData["ErrorMessage"] = "Студент уже состоит в данной группе.";
        }

        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    /// <summary>
    /// Задание 13: Удаление студента через метод RemoveStudent()
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveStudent(int groupId, int studentId)
    {
        var group = await _context.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == groupId);
        var student = await _context.Students.FindAsync(studentId);

        if (group == null || student == null)
            return NotFound();

        // Задание 13: контролируемое удаление из списка группы
        bool removed = group.RemoveStudent(studentId);
        if (removed)
        {
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Студент {student.GetFullName()} удален из группы {group.Name}.";
        }
        else
        {
            TempData["ErrorMessage"] = "Студент не найден в списке группы.";
        }

        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _context.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == id);
        if (group != null)
        {
            foreach (var s in group.Students)
            {
                s.GroupId = null;
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Группа успешно удалена.";
        }

        return RedirectToAction(nameof(Index));
    }
}
