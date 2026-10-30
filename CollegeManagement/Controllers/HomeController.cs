using System.Diagnostics;
using System.Security.Claims;
using CollegeManagement.Data;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers;

public class HomeController : Controller
{
    private readonly CollegeDbContext _context;
    private readonly ReportManager _reportManager;

    public HomeController(CollegeDbContext context, ReportManager reportManager)
    {
        _context = context;
        _reportManager = reportManager;
    }

    /// <summary>
    /// Главная страница колледжа (Задание 30)
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // Задание 21: использование internal класса CollegeSettings
        var settings = await _context.CollegeSettings.FirstOrDefaultAsync() ?? new CollegeSettings();

        ViewBag.CollegeName = settings.CollegeName;
        ViewBag.AcademicYear = settings.AcademicYear;
        ViewBag.Address = settings.Address;
        ViewBag.PhoneNumber = settings.PhoneNumber;
        ViewBag.Summary = settings.GetSummary();

        ViewBag.StudentsCount = await _context.Students.CountAsync();
        ViewBag.TeachersCount = await _context.Teachers.CountAsync();
        ViewBag.GroupsCount = await _context.Groups.CountAsync();
        ViewBag.SubjectsCount = await _context.Subjects.CountAsync();
        ViewBag.GradesCount = await _context.Grades.CountAsync();

        return View();
    }

    /// <summary>
    /// Страница авторизации (Задание 26, 30)
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Index));
        }

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// Обработка авторизации через UserAccount (Задание 26, 27)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Введите имя пользователя и пароль.");
            return View();
        }

        // Поиск пользователя в БД
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.Trim().ToLower());

        // Задание 26: проверка через класс UserAccount
        var userAccount = new UserAccount(username, password);
        bool isValid = userAccount.Login(username, password, user);

        if (!isValid || user == null)
        {
            ModelState.AddModelError("", "Неверное имя пользователя или пароль.");
            return View();
        }

        // Создание Cookie клеймов
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.DisplayName),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Быстрый демо-вход для проверки задания
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickLogin(string role)
    {
        string username = role.ToLowerInvariant() switch
        {
            "admin" => "admin",
            "teacher" => "teacher",
            "student" => "student",
            _ => "admin"
        };
        string password = username + "123";

        return await Login(username, password);
    }

    /// <summary>
    /// Выход из системы (Задание 26)
    /// </summary>
    public async Task<IActionResult> Logout()
    {
        var account = new UserAccount();
        account.Logout(); // Задание 26

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Административная панель (Задание 30, 21, 22, 24)
    /// </summary>
    public async Task<IActionResult> Admin(string? reportType = null)
    {
        var settings = await _context.CollegeSettings.FirstOrDefaultAsync() ?? new CollegeSettings();
        var users = await _context.Users.ToListAsync();

        // Задание 24: вызов protected internal метода GenerateReport() через сервис
        string reportText = string.Empty;
        if (!string.IsNullOrEmpty(reportType))
        {
            reportText = _reportManager.GenerateReport(reportType, _context);
        }
        else
        {
            reportText = _reportManager.GenerateReport("summary", _context);
        }

        ViewBag.ReportText = reportText;
        ViewBag.SelectedReport = reportType ?? "summary";
        ViewBag.Users = users;

        return View(settings);
    }

    /// <summary>
    /// Сохранение настроек колледжа (Задание 22)
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSettings(string collegeName, string academicYear, string address, string phone)
    {
        var settings = await _context.CollegeSettings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new CollegeSettings();
            _context.CollegeSettings.Add(settings);
        }

        // Задание 22: вызов internal методов для изменения настроек
        settings.UpdateSettings(collegeName, academicYear);
        settings.UpdateContacts(address, phone);

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Настройки колледжа успешно обновлены с использованием internal-методов!";
        return RedirectToAction(nameof(Admin));
    }

    /// <summary>
    /// Интерактивная лабораторная страница по модификаторам доступа
    /// </summary>
    public IActionResult AccessModifiersGuide()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
