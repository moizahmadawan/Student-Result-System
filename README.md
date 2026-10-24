# Student Result Management System (C# / .NET 8)

A console application built with C# and .NET 8 to manage students, course results, CGPA and department reports. Data is saved automatically to a JSON file, so nothing is lost when you close the program.

## Features

- Add, search, list and delete students
- Add, update and remove course results (marks 0-100, credit hours 1-6)
- Automatic **letter grade, grade points and credit-weighted CGPA** (4.0 scale)
- Printable-style **transcript** for each student
- List students sorted by ID, name or CGPA
- **Top performers** list
- **Department report** (student count, average CGPA, best student)
- Input validation with friendly error messages
- Persistent storage in `data/students.json`
- Demo data is created on the very first launch

## Concepts Demonstrated

| Concept | Where |
|---|---|
| OOP (classes, encapsulation) | `Models/` |
| Interface + dependency injection | `IStudentRepository` is passed to `StudentService` |
| Separation of concerns | `Models` / `Services` / `UI` layers |
| LINQ (`Where`, `GroupBy`, `OrderBy`, `Average`, `Take`) | `StudentService` |
| JSON serialization (`System.Text.Json`) | `JsonStudentRepository` |
| Pattern matching and switch expressions | `GradeCalculator`, `ConsoleApp` |
| Records | `DepartmentSummary` |
| Exception handling and validation | `StudentService`, `ConsoleApp` |

## Project Structure

```
StudentResultSystem/
├── Models/
│   ├── CourseResult.cs        # One course result (marks, grade, grade points)
│   ├── DepartmentSummary.cs   # Record used by the department report
│   └── Student.cs             # Student with results and CGPA
├── Services/
│   ├── GradeCalculator.cs     # Marks -> letter grade and grade points
│   ├── IStudentRepository.cs  # Storage abstraction
│   ├── JsonStudentRepository.cs
│   └── StudentService.cs      # All business logic (LINQ)
├── UI/
│   ├── ConsoleApp.cs          # Menu and screens
│   └── ConsoleHelper.cs       # Safe input helpers
├── Program.cs                 # Entry point
├── StudentResultSystem.csproj
├── .gitignore
├── LICENSE
└── README.md
```

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

Check your version:

```bash
dotnet --version
```

## Run

```bash
git clone https://github.com/YOUR_USERNAME/StudentResultSystem.git
cd StudentResultSystem
dotnet run
```

Open the folder in Visual Studio or VS Code and press **F5** to run it from the IDE.

## Grading Scale

| Marks | Grade | Points |
|---|---|---|
| 85+ | A | 4.00 |
| 80-84 | A- | 3.67 |
| 75-79 | B+ | 3.33 |
| 70-74 | B | 3.00 |
| 65-69 | B- | 2.67 |
| 60-64 | C+ | 2.33 |
| 55-59 | C | 2.00 |
| 50-54 | C- | 1.67 |
| 45-49 | D | 1.00 |
| below 45 | F | 0.00 |

CGPA = sum(grade points x credit hours) / total credit hours.
To use a different scale, edit `Services/GradeCalculator.cs`.

## Possible Improvements

- Unit tests with xUnit
- SQLite or SQL Server repository (the interface is already in place)
- Export transcript to PDF or CSV
- WPF / WinForms / ASP.NET Core front end

## Author

**Malik Moiz Ahmad Awan**
BS Computer Science, University of Gujrat

Licensed under the MIT License.
