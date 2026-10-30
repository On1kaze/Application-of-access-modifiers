using System.Reflection;
using CollegeManagement.Models;
using CollegeManagement.Services;
using Xunit;

namespace CollegeManagement.Tests;

/// <summary>
/// Тесты для Раздела 4: Модификаторы internal, protected internal, private protected (Задания 21-25).
/// </summary>
public class Section4_InternalAndModifiersTests
{
    // Задание 21: Класс CollegeSettings с модификатором internal
    [Fact]
    public void Task21_CollegeSettings_ShouldBeInternal()
    {
        var type = typeof(CollegeSettings);
        Assert.False(type.IsPublic, "Класс CollegeSettings НЕ должен быть public.");
        Assert.True(type.IsNotPublic, "Класс CollegeSettings должен быть internal (IsNotPublic).");
    }

    // Задание 22: Методы изменения настроек доступны только внутри сборки через internal
    [Fact]
    public void Task22_UpdateSettings_ShouldBeInternal()
    {
        var method = typeof(CollegeSettings).GetMethod("UpdateSettings", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);
        Assert.True(method.IsAssembly, "Метод UpdateSettings должен иметь модификатор internal (Assembly).");

        var settings = new CollegeSettings("Старый колледж", "2025-2026");
        settings.UpdateSettings("Новый колледж", "2026-2027");

        Assert.Equal("Новый колледж", settings.CollegeName);
        Assert.Equal("2026-2027", settings.AcademicYear);
    }

    // Задание 23: protected internal метод GetFullName() в базовом классе Person
    [Fact]
    public void Task23_GetFullName_ShouldBeProtectedInternal()
    {
        var method = typeof(Person).GetMethod("GetFullName", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(method);
        Assert.True(method.IsFamilyOrAssembly, 
            "Метод GetFullName должен быть protected internal (FamilyOrAssembly).");

        var student = new Student("Иван", "Петров", 19, "ИС-21");
        Assert.Equal("Иван Петров", student.GetFullName());
    }

    // Задание 24: ReportManager с методом protected internal GenerateReport()
    [Fact]
    public void Task24_GenerateReport_ShouldBeProtectedInternal()
    {
        var method = typeof(ReportManager).GetMethod("GenerateReport", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(method);
        Assert.True(method.IsFamilyOrAssembly, 
            "Метод GenerateReport должен иметь модификатор protected internal (FamilyOrAssembly).");
    }

    // Задание 25: private protected метод CalculateStatistics() в Person
    [Fact]
    public void Task25_CalculateStatistics_ShouldBePrivateProtected()
    {
        var method = typeof(Person).GetMethod("CalculateStatistics", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);
        Assert.True(method.IsFamilyAndAssembly, 
            "Метод CalculateStatistics должен иметь модификатор private protected (FamilyAndAssembly).");

        var student = new Student("Иван", "Петров", 19, "ИС-21");
        string stats = student.GetInternalStatistics();
        Assert.Contains("Иван Петров", stats);
    }
}
