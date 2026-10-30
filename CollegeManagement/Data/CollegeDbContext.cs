using CollegeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Data;

public class CollegeDbContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<TeacherUser> TeacherUsers => Set<TeacherUser>();
    public DbSet<StudentUser> StudentUsers => Set<StudentUser>();

    // Задание 21: internal DbSet для настроек
    internal DbSet<CollegeSettings> CollegeSettings => Set<CollegeSettings>();

    public CollegeDbContext(DbContextOptions<CollegeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка Person / Student / Teacher
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasField("_name").IsRequired();
            entity.Property(s => s.Surname).HasField("_surname").IsRequired();
            entity.Property(s => s.Age).HasField("_age").IsRequired();
            entity.Property(s => s.Group).HasField("_group").IsRequired();
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).HasField("_name").IsRequired();
            entity.Property(t => t.Surname).HasField("_surname").IsRequired();
            entity.Property(t => t.Age).HasField("_age").IsRequired();
            entity.Property(t => t.Subject).HasField("_subject").IsRequired();
            entity.Property(t => t.Experience).HasField("_experience").IsRequired();
        });

        // Настройка Group и инкапсулированного списка студентов
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired();
            entity.Property(g => g.Course).IsRequired();

            entity.HasMany(g => g.Students)
                  .WithOne(s => s.GroupNavigation)
                  .HasForeignKey(s => s.GroupId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.Navigation(g => g.Students).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // Настройка Subject
        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasField("_name").IsRequired();
            entity.Property(s => s.Hours).HasField("_hours").IsRequired();

            entity.HasOne(s => s.Teacher)
                  .WithMany()
                  .HasForeignKey(s => s.TeacherId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.Navigation(s => s.Teacher).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // Настройка User и закрытых полей пароля
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).HasField("_username").IsRequired();
            entity.Property(u => u.DisplayName).HasField("_displayName").IsRequired();
            entity.Property(u => u.Email).HasField("_email");
            entity.Property(u => u.Role).HasField("_role").IsRequired();

            entity.Property<string>("_passwordHash").HasColumnName("PasswordHash");
            entity.Property<string>("_passwordSalt").HasColumnName("PasswordSalt");

            entity.HasDiscriminator<string>("UserType")
                  .HasValue<Admin>("Admin")
                  .HasValue<TeacherUser>("Teacher")
                  .HasValue<StudentUser>("Student");
        });

        // Настройка CollegeSettings
        modelBuilder.Entity<CollegeSettings>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.CollegeName).IsRequired();
            entity.Property(c => c.AcademicYear).IsRequired();
        });
    }

    public static void SeedData(CollegeDbContext context)
    {
        context.Database.EnsureCreated();

        if (!context.CollegeSettings.Any())
        {
            var settings = new CollegeSettings("Московский политехнический колледж", "2026-2027");
            context.CollegeSettings.Add(settings);
            context.SaveChanges();
        }

        if (!context.Groups.Any())
        {
            var g1 = new Group("ИС-21", 2);
            var g2 = new Group("П-31", 3);
            var g3 = new Group("ВЕБ-11", 1);
            context.Groups.AddRange(g1, g2, g3);
            context.SaveChanges();
        }

        if (!context.Teachers.Any())
        {
            var t1 = new Teacher("Александр", "Смирнов", 45, "Программирование на C#", 20);
            var t2 = new Teacher("Елена", "Кузнецова", 38, "Базы данных и SQL", 14);
            var t3 = new Teacher("Михаил", "Иванов", 52, "Архитектура ПК и ОС", 25);
            context.Teachers.AddRange(t1, t2, t3);
            context.SaveChanges();
        }

        if (!context.Subjects.Any())
        {
            var t1 = context.Teachers.FirstOrDefault(t => t.Surname == "Смирнов");
            var t2 = context.Teachers.FirstOrDefault(t => t.Surname == "Кузнецова");
            var t3 = context.Teachers.FirstOrDefault(t => t.Surname == "Иванов");

            var s1 = new Subject("Разработка веб-приложений ASP.NET Core", 120);
            if (t1 != null) s1.AssignTeacher(t1);

            var s2 = new Subject("Базы данных и Entity Framework", 90);
            if (t2 != null) s2.AssignTeacher(t2);

            var s3 = new Subject("Операционные системы и сети", 72);
            if (t3 != null) s3.AssignTeacher(t3);

            context.Subjects.AddRange(s1, s2, s3);
            context.SaveChanges();
        }

        if (!context.Students.Any())
        {
            var g1 = context.Groups.FirstOrDefault(g => g.Name == "ИС-21");
            var g2 = context.Groups.FirstOrDefault(g => g.Name == "П-31");
            var g3 = context.Groups.FirstOrDefault(g => g.Name == "ВЕБ-11");

            var st1 = new Student("Иван", "Петров", 19, g1?.Name ?? "ИС-21") { GroupId = g1?.Id };
            var st2 = new Student("Анна", "Сидорова", 20, g1?.Name ?? "ИС-21") { GroupId = g1?.Id };
            var st3 = new Student("Максим", "Волков", 18, g3?.Name ?? "ВЕБ-11") { GroupId = g3?.Id };
            var st4 = new Student("Дарья", "Морозова", 21, g2?.Name ?? "П-31") { GroupId = g2?.Id };
            var st5 = new Student("Артём", "Соколов", 19, g1?.Name ?? "ИС-21") { GroupId = g1?.Id };

            context.Students.AddRange(st1, st2, st3, st4, st5);
            context.SaveChanges();
        }

        if (!context.Grades.Any())
        {
            var st1 = context.Students.FirstOrDefault(s => s.Surname == "Петров");
            var st2 = context.Students.FirstOrDefault(s => s.Surname == "Сидорова");
            var s1 = context.Subjects.FirstOrDefault(s => s.Name.Contains("ASP.NET"));
            var s2 = context.Subjects.FirstOrDefault(s => s.Name.Contains("Базы данных"));

            if (st1 != null && s1 != null)
            {
                context.Grades.Add(new Grade(st1.Id, st1.GetFullName(), s1.Id, s1.Name, 5, DateTime.Today.AddDays(-5), "Отличная практическая работа"));
                context.Grades.Add(new Grade(st1.Id, st1.GetFullName(), s1.Id, s1.Name, 4, DateTime.Today.AddDays(-2), "Тест по модификаторам доступа"));
            }
            if (st2 != null && s2 != null)
            {
                context.Grades.Add(new Grade(st2.Id, st2.GetFullName(), s2.Id, s2.Name, 5, DateTime.Today.AddDays(-4), "Лабораторная работа по EF Core"));
                context.Grades.Add(new Grade(st2.Id, st2.GetFullName(), s2.Id, s2.Name, 4, DateTime.Today.AddDays(-1), "Контрольная работа"));
            }
            context.SaveChanges();
        }

        if (!context.AttendanceRecords.Any())
        {
            var st1 = context.Students.FirstOrDefault(s => s.Surname == "Петров");
            var st2 = context.Students.FirstOrDefault(s => s.Surname == "Сидорова");
            var s1 = context.Subjects.FirstOrDefault();

            if (st1 != null && s1 != null)
            {
                context.AttendanceRecords.Add(new AttendanceRecord(0, st1.Id, st1.GetFullName(), s1.Id, s1.Name, DateTime.Today.AddDays(-3), AttendanceStatus.Present, "Присутствовал"));
                context.AttendanceRecords.Add(new AttendanceRecord(0, st1.Id, st1.GetFullName(), s1.Id, s1.Name, DateTime.Today.AddDays(-1), AttendanceStatus.Late, "Опоздал на 5 минут"));
            }
            if (st2 != null && s1 != null)
            {
                context.AttendanceRecords.Add(new AttendanceRecord(0, st2.Id, st2.GetFullName(), s1.Id, s1.Name, DateTime.Today.AddDays(-3), AttendanceStatus.Present, "Присутствовал"));
                context.AttendanceRecords.Add(new AttendanceRecord(0, st2.Id, st2.GetFullName(), s1.Id, s1.Name, DateTime.Today.AddDays(-1), AttendanceStatus.Present, "Присутствовал"));
            }
            context.SaveChanges();
        }

        if (!context.Users.Any())
        {
            var admin = new Admin("admin", "admin123", "Главный Администратор", "admin@college.edu");
            var teacher = new TeacherUser("teacher", "teacher123", "Александр Смирнов", "smirnov@college.edu", 1);
            var student = new StudentUser("student", "student123", "Иван Петров", "petrov@student.college.edu", 1);

            context.Users.AddRange(admin, teacher, student);
            context.SaveChanges();
        }
    }
}
