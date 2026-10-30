using CollegeManagement.Data;
using CollegeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Services;

/// <summary>
/// Сервис для работы со студентами и инкапсулированными операциями.
/// </summary>
public class StudentService
{
    private readonly CollegeDbContext _context;

    public StudentService(CollegeDbContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllStudentsAsync()
    {
        return await _context.Students.Include(s => s.GroupNavigation).ToListAsync();
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        return await _context.Students.Include(s => s.GroupNavigation).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateStudentAsync(string name, string surname, int age, string groupName)
    {
        // Создание студента через публичный конструктор (Задание 5)
        // В конструкторе вызывается ValidateStudentData() (Задание 6) и проверка возраста (Задание 4)
        var student = new Student(name, surname, age, groupName);

        // Поиск или создание группы
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Name.ToLower() == groupName.ToLower());
        if (group != null)
        {
            group.AddStudent(student); // Задание 12
        }

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> UpdateStudentAgeAsync(int studentId, int newAge)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return false;

        // Задание 4: метод SetAge() разрешает только от 15 до 70 лет
        student.SetAge(newAge);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStudentAsync(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return false;

        // Удаление из группы через метод группы (Задание 13)
        if (student.GroupId.HasValue)
        {
            var group = await _context.Groups.FindAsync(student.GroupId.Value);
            group?.RemoveStudent(studentId);
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }
}
