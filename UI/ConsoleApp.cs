using StudentResultSystem.Models;
using StudentResultSystem.Services;
using static StudentResultSystem.UI.ConsoleHelper;

namespace StudentResultSystem.UI;

public class ConsoleApp
{
    private readonly StudentService _service;

    public ConsoleApp(StudentService service) => _service = service;

    public void Run()
    {
        while (true)
        {
            ShowMenu();
            int choice = ReadInt("Choice: ", 0, 9);
            Console.WriteLine();
            if (choice == 0) { Info("Goodbye!"); return; }

            try
            {
                switch (choice)
                {
                    case 1: AddStudent(); break;
                    case 2: AddOrUpdateResult(); break;
                    case 3: RemoveResult(); break;
                    case 4: ShowTranscript(); break;
                    case 5: SearchStudents(); break;
                    case 6: ListStudents(); break;
                    case 7: ShowTopStudents(); break;
                    case 8: ShowDepartmentReport(); break;
                    case 9: DeleteStudent(); break;
                }
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                Error("  " + ex.Message);
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("\n======= STUDENT RESULT MANAGEMENT SYSTEM =======");
        Console.WriteLine(" 1. Add student");
        Console.WriteLine(" 2. Add / update course result");
        Console.WriteLine(" 3. Remove course result");
        Console.WriteLine(" 4. View transcript");
        Console.WriteLine(" 5. Search students");
        Console.WriteLine(" 6. List students (sorted)");
        Console.WriteLine(" 7. Top performers");
        Console.WriteLine(" 8. Department report");
        Console.WriteLine(" 9. Delete student");
        Console.WriteLine(" 0. Exit");
    }

    private void AddStudent()
    {
        string name = ReadString("Name: ");
        string dept = ReadString("Department: ");
        int sem = ReadInt("Semester (1-8): ", 1, 8);
        var s = _service.AddStudent(name, dept, sem);
        Info($"  Student added with ID {s.Id}.");
    }

    private void AddOrUpdateResult()
    {
        int id = ReadInt("Student ID: ");
        var student = _service.GetById(id);
        if (student is null) { Error("  Student not found."); return; }

        Console.WriteLine($"  Student: {student.Name}");
        string code = ReadString("Course code (e.g. CS301): ");
        string name = ReadString("Course name: ");
        int credits = ReadInt("Credit hours (1-6): ", 1, 6);
        double marks = ReadDouble("Marks (0-100): ", 0, 100);

        bool isNew = _service.AddOrUpdateResult(id, code, name, credits, marks);
        var (letter, _) = GradeCalculator.FromMarks(marks);
        Info($"  Result {(isNew ? "added" : "updated")}. Grade: {letter}");
    }

    private void RemoveResult()
    {
        int id = ReadInt("Student ID: ");
        string code = ReadString("Course code to remove: ");
        if (_service.RemoveResult(id, code)) Info("  Result removed.");
        else Error("  Course not found for this student.");
    }

    private void ShowTranscript()
    {
        int id = ReadInt("Student ID: ");
        var s = _service.GetById(id);
        if (s is null) { Error("  Student not found."); return; }

        Console.WriteLine("  ================ TRANSCRIPT ================");
        Console.WriteLine($"  ID: {s.Id}   Name: {s.Name}");
        Console.WriteLine($"  Department: {s.Department}   Semester: {s.Semester}");
        Console.WriteLine("  --------------------------------------------");
        if (s.Results.Count == 0)
        {
            Console.WriteLine("  No course results yet.");
        }
        else
        {
            Console.WriteLine($"  {"Code",-8}{"Course",-26}{"Cr",3}{"Marks",7}{"Grade",7}{"GP",6}");
            foreach (var r in s.Results.OrderBy(r => r.CourseCode))
                Console.WriteLine($"  {r.CourseCode,-8}{Trim(r.CourseName, 25),-26}{r.CreditHours,3}{r.Marks,7:F1}{r.Grade,7}{r.GradePoints,6:F2}");
        }
        Console.WriteLine("  --------------------------------------------");
        Console.WriteLine($"  Total credits: {s.TotalCredits}   CGPA: {s.Cgpa:F2}");
    }

    private void SearchStudents()
    {
        string term = ReadString("Search (ID, name or department): ");
        PrintStudents(_service.Search(term));
    }

    private void ListStudents()
    {
        int opt = ReadInt("Sort by (1 = ID, 2 = Name, 3 = CGPA high to low): ", 1, 3);
        var sort = opt switch { 2 => SortBy.Name, 3 => SortBy.Cgpa, _ => SortBy.Id };
        PrintStudents(_service.GetAll(sort));
    }

    private void ShowTopStudents()
    {
        int n = ReadInt("How many top students? ", 1, 100);
        PrintStudents(_service.TopStudents(n));
    }

    private void ShowDepartmentReport()
    {
        var report = _service.DepartmentReport();
        if (report.Count == 0) { Console.WriteLine("  No data yet."); return; }

        Console.WriteLine($"  {"Department",-24}{"Students",9}{"Avg CGPA",10}{"Best",7}  Top student");
        foreach (var d in report)
            Console.WriteLine($"  {Trim(d.Department, 23),-24}{d.StudentCount,9}{d.AverageCgpa,10:F2}{d.HighestCgpa,7:F2}  {d.TopStudent}");
    }

    private void DeleteStudent()
    {
        int id = ReadInt("Student ID to delete: ");
        var s = _service.GetById(id);
        if (s is null) { Error("  Student not found."); return; }

        string confirm = ReadString($"  Delete {s.Name}? (y/n): ");
        if (confirm.Trim().Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _service.RemoveStudent(id);
            Info("  Student deleted.");
        }
        else Console.WriteLine("  Cancelled.");
    }

    // ---------- helpers ----------

    private static void PrintStudents(List<Student> list)
    {
        if (list.Count == 0) { Console.WriteLine("  No students found."); return; }

        Console.WriteLine($"  {"ID",-5}{"Name",-22}{"Department",-22}{"Sem",4}{"Credits",9}{"CGPA",7}");
        foreach (var s in list)
            Console.WriteLine($"  {s.Id,-5}{Trim(s.Name, 21),-22}{Trim(s.Department, 21),-22}{s.Semester,4}{s.TotalCredits,9}{s.Cgpa,7:F2}");
    }

    private static string Trim(string s, int max) => s.Length <= max ? s : s[..(max - 1)] + "…";
}
