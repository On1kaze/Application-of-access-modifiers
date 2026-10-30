# CollegeManagement — Применение модификаторов доступа в C# (.NET 8)

Полнофункциональное веб-приложение на **ASP.NET Core MVC** и **Entity Framework Core (SQLite)**, разработанное для практического изучения и демонстрации всех модификаторов доступа языка C#:
- `public`
- `private`
- `protected`
- `internal`
- `protected internal`
- `private protected`

Репозиторий проекта: [https://github.com/On1kaze/Application-of-access-modifiers](https://github.com/On1kaze/Application-of-access-modifiers)

---

## 🚀 Как запустить проект локально

### Вариант 1: Через терминал (.NET CLI)

1. Убедитесь, что установлен [.NET SDK 8.0+](https://dotnet.microsoft.com/download).
2. Клонируйте репозиторий или перейдите в корень проекта:
   ```bash
   git clone https://github.com/On1kaze/Application-of-access-modifiers.git
   cd Application-of-access-modifiers
   ```
3. Запустите веб-приложение:
   ```bash
   dotnet run --project CollegeManagement/CollegeManagement.csproj
   ```
   Либо перейдите в папку проекта:
   ```bash
   cd CollegeManagement
   dotnet run
   ```
4. Откройте в браузере адрес, указанный в консоли (обычно `http://localhost:5000` или `https://localhost:5001`). При первом запуске база данных SQLite (`college.db`) создается и автоматически наполняется демонстрационными данными.

### Вариант 2: В Visual Studio Code

1. Откройте папку проекта в VS Code:
   ```bash
   code .
   ```
2. Нажмите `F5` для запуска отладки (конфигурация `.vscode/launch.json` уже настроена).
3. Или используйте задачи VS Code (`Ctrl+Shift+P` -> `Tasks: Run Task` -> `run`).

### Вариант 3: Запуск модульных тестов

В решении реализован отдельный тестовый проект `CollegeManagement.Tests` (30 xUnit-тестов, проверяющих каждое из 30 заданий):
```bash
dotnet test
```

---

## 📂 Структура проекта

```text
Application-of-access-modifiers/
│
├── .vscode/                     # Конфигурация для запуска и отладки в VS Code
│   ├── launch.json
│   ├── tasks.json
│   ├── settings.json
│   └── extensions.json
│
├── CollegeManagement.sln        # Файл решения .NET
│
├── CollegeManagement/           # Основной веб-проект ASP.NET Core MVC
│   │
│   ├── Controllers/             # Контроллеры MVC
│   │   ├── HomeController.cs        # Главная, авторизация, админ-панель, справка
│   │   ├── StudentController.cs     # Управление студентами, валидация возраста
│   │   ├── TeacherController.cs     # Управление преподавателями и стажем
│   │   ├── GroupController.cs       # Учебные группы и привязка студентов
│   │   ├── GradeController.cs       # Электронный журнал оценок (2-5)
│   │   └── AttendanceController.cs  # Учет и статистика посещаемости
│   │
│   ├── Models/                  # Модели предметной области с модификаторами
│   │   ├── Person.cs                # Базовый класс (protected, protected internal, private protected)
│   │   ├── Student.cs               # Наследник Person, инкапсуляция возраста и валидации
│   │   ├── Teacher.cs               # Наследник Person, инкапсуляция стажа
│   │   ├── Group.cs                 # Закрытый список студентов, методы Add/Remove
│   │   ├── Subject.cs               # Учебная дисциплина, метод AssignTeacher
│   │   ├── CollegeSettings.cs       # internal класс и internal методы настроек
│   │   ├── User.cs                  # Иерархия пользователей (Admin, Teacher, Student) и UserAccount
│   │   ├── Grade.cs                 # Оценка и класс GradeBook с валидацией 2-5
│   │   └── AttendanceRecord.cs      # Запись посещаемости и статусы
│   │
│   ├── Services/                # Сервисный слой приложения
│   │   ├── StudentService.cs        # Бизнес-логика работы со студентами
│   │   ├── ReportManager.cs         # Генерация отчетов (protected internal)
│   │   └── AttendanceManager.cs     # Учет посещаемости и расчет статистики
│   │
│   ├── Data/                    # Слой доступа к данным (EF Core)
│   │   └── CollegeDbContext.cs      # Контекст БД SQLite с авто-сидингом данных
│   │
│   ├── Views/                   # Представления Razor
│   │   ├── Home/
│   │   ├── Student/
│   │   ├── Teacher/
│   │   ├── Group/
│   │   ├── Grade/
│   │   ├── Attendance/
│   │   └── Shared/
│   │
│   ├── wwwroot/                 # Статические файлы (CSS, JS, библиотеки)
│   └── Program.cs               # Точка входа, DI, Cookie-аутентификация, SQLite
│
└── CollegeManagement.Tests/     # 30 модульных тестов по всем заданиям
    ├── Section1_StudentEncapsulationTests.cs
    ├── Section2_TeachersAndGroupsTests.cs
    ├── Section3_InheritanceAndProtectedTests.cs
    ├── Section4_InternalAndModifiersTests.cs
    └── Section5_SiteFeaturesTests.cs
```

---

## 📋 Реализация 30 заданий лабораторной работы

### 1. Создание структуры и применение public/private
1. **Класс Student:** Создан `public class Student` с полями Name, Surname, Age, Group.
2. **Защита персональных данных:** Поля закрыты через `private`/`protected`. Реализованы публичные методы `GetName()`, `GetSurname()`, `GetAge()`, `GetGroup()`.
3. **Создание свойств:** Свойства `Name`, `Surname`, `Age` обеспечивают безопасную модификацию данных с валидацией.
4. **Проверка возраста:** Метод `SetAge()` разрешает устанавливать возраст только в диапазоне от 15 до 70 лет, генерируя исключение `ArgumentOutOfRangeException` при нарушении.
5. **Добавление конструктора:** Публичный конструктор `Student(name, surname, age, group)` инициализирует данные объекта.
6. **Скрытие внутренних методов:** Приватный метод `ValidateStudentData()` проверяет корректность данных и вызывается из конструктора.
7. **Отображение информации:** Публичный метод `GetStudentInfo()` возвращает полную строковую информацию о студенте.
8. **Запрет прямого изменения данных:** Свойство `StudentId` сделано доступным только для чтения извне (`public int StudentId { get; private set; }`).

### 2. Управление преподавателями и группами
9. **Класс Teacher:** Добавлен класс преподавателя с закрытыми полями и открытыми свойствами (Name, Surname, Age, Subject, Experience).
10. **Ограничение изменения стажа:** Свойство `Experience` имеет закрытый сеттер и изменяется исключительно через метод `UpdateExperience()`.
11. **Класс Group:** Учебная группа с названием, курсом и закрытым списком студентов (`private readonly List<Student> _students`), доступным для чтения как `IReadOnlyList<Student>`.
12. **Добавление студентов в группу:** Метод `AddStudent(Student student)` добавляет студента только если он не `null` и еще не состоит в группе.
13. **Удаление студентов:** Метод `RemoveStudent(int studentId)` обеспечивает контролируемое удаление из списка. Прямое удаление извне невозможно.
14. **Класс Subject:** Учебный предмет с закрытыми полями названия `_name`, часов `_hours` и преподавателя `_teacher`.
15. **Связь преподавателя с предметом:** Метод `AssignTeacher(Teacher teacher)` назначает преподавателя с обязательной проверкой `teacher != null`.

### 3. Наследование и protected
16. **Базовый класс Person:** Выделен абстрактный класс `Person` с защищёнными полями `protected string _name`, `protected string _surname`, `protected int _age`.
17. **Наследование класса Student:** Класс `Student` наследует `Person`. Дублирующиеся поля удалены.
18. **Наследование класса Teacher:** Класс `Teacher` наследует `Person`. Реализован метод `GetTeacherInfo()`.
19. **Использование protected-методов:** В `Person` реализован защищённый метод `protected virtual string DisplayBasicInfo()`, используемый внутри методов `Student.GetStudentInfo()` и `Teacher.GetTeacherInfo()`.
20. **Ограничение доступа к данным:** Продемонстрировано разграничение: поля `protected` доступны наследникам, а закрытые поля (например, `_internalPersonCode`) и приватные методы доступны исключительно внутри базового класса `Person`.

### 4. Модификаторы internal и protected internal
21. **Класс CollegeSettings:** Объявлен с модификатором `internal class CollegeSettings`. Хранит название колледжа и учебный год.
22. **Ограничение доступа к настройкам:** Методы `UpdateSettings()` и `UpdateContacts()` имеют модификатор `internal` и доступны только внутри текущей сборки.
23. **Protected internal метод:** В базовый класс `Person` добавлен метод `protected internal virtual string GetFullName()`, доступный внутри сборки и в классах-наследниках.
24. **Разделение доступа к отчётам:** Создан класс `ReportManager` с методом `protected internal virtual string GenerateReport(...)`, используемый в проекте для генерации отчетов по успеваемости, студентам и преподавателям.
25. **Применение private protected:** В `Person` реализован метод `private protected virtual string CalculateStatistics()`, доступный классам-наследникам ТОЛЬКО внутри текущей сборки.

### 5. Реализация функциональности сайта
26. **Система авторизации:** Создан класс `UserAccount` с приватными полями логина и пароля, методами `Login()` и `Logout()`. Пароль никогда не выводится в строковом представлении.
27. **Разделение ролей пользователей:** Базовый класс `User` (общие поля `protected`, хэш и соль пароля `private`) и наследники `Admin`, `TeacherUser`, `StudentUser`.
28. **Журнал оценок:** Класс `GradeBook` с закрытым списком оценок `_grades`, методами `AddGrade()` и `RemoveGrade()`. Проверка допустимости оценок строго от 2 до 5.
29. **Система посещаемости:** Класс `AttendanceManager` с закрытыми данными о посещаемости, методами добавления, изменения и расчетом статистики `GetAttendanceStatistics()`.
30. **Финальная сборка веб-приложения:** Полноценное веб-приложение:
    - Страница авторизации (`/Home/Login`) с поддержкой ролей и быстрым входом в 1 клик.
    - Главная страница колледжа (`/Home/Index`) со сводкой настроек и статистикой.
    - Страница студентов (`/Student`) с каталогом, просмотром карточки, изменением возраста и удалением.
    - Страница преподавателей (`/Teacher`) с обновлением стажа и закрепленными предметами.
    - Страница групп (`/Group`) с просмотром состава, зачислением и отчислением.
    - Электронный журнал оценок (`/Grade`) со шкалой 2-5 и расчетом среднего балла.
    - Страница посещаемости (`/Attendance`) с отметками присутствия/опозданий и процентовкой.
    - Административная панель (`/Home/Admin`) для редактирования настроек колледжа и формирования отчетов.
    - Интерактивный гид по модификаторам доступа (`/Home/AccessModifiersGuide`).

---

## 👥 Демонстрационные учетные записи

Для удобства проверки в системе преднастроены учетные записи:

| Роль | Логин | Пароль | Доступные функции |
| :--- | :--- | :--- | :--- |
| **Администратор** | `admin` | `admin123` | Полный доступ, админ-панель, изменение настроек колледжа, генерация отчетов |
| **Преподаватель** | `teacher` | `teacher123` | Выставление оценок, отметки посещаемости, просмотр групп и студентов |
| **Студент** | `student` | `student123` | Просмотр успеваемости, журнала, групп и расписания |

На странице авторизации и главной странице доступны кнопки **быстрого входа в 1 клик** без ручного ввода пароля.
