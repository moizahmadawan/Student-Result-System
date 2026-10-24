using StudentResultSystem.Models;

namespace StudentResultSystem.Services;

public enum SortBy { Id, Name, Cgpa }

/// <summary>All business logic. The UI only calls this class.</summary>
public class StudentService
{
    private readonly IStudentRepository _repo;
    private readonly List<Student> _students;

    public StudentService(IStudentRepository repo)
    {
        _repo = repo;
        _students = repo.Load();
    }

    public int Count => _students.Count;

    // ---------- Students ----------

    public Student AddStudent(string name, string department, int semester)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
        if (string.IsNullOrWhiteSpace(department)) throw new ArgumentException("Department cannot be empty.");
        if (semester is < 1 or > 8) throw new ArgumentException("Semester must be between 1 and 8.");

        var student = new Student
        {
            Id = _students.Count == 0 ? 1 : _students.Max(s => s.Id) + 1,
            Name = name.Trim(),
            Department = department.Trim(),
            Semester = semester
        };
        _students.Add(student);
        _repo.Save(_students);
        return student;
    }

    public Student? GetById(int id) => _students.FirstOrDefault(s => s.Id == id);

    public bool RemoveStudent(int id)
    {
        var student = GetById(id);
        if (student is null) return false;
        _students.Remove(student);
        _repo.Save(_students);
        return true;
    }

    /// <summary>Search by ID (if the term is a number) or by name / department text.</summary>
    public List<Student> Search(string term)
    {
        term = term.Trim();
        if (term.Length == 0) return new List<Student>();

        if (int.TryParse(term, out int id))
        {
            var byId = GetById(id);
            return byId is null ? new List<Student>() : new List<Student> { byId };
        }

        return _students
            .Where(s => s.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        s.Department.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.Name)
            .ToList();
    }

    public List<Student> GetAll(SortBy sortBy) => sortBy switch
    {
        SortBy.Name => _students.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList(),
        SortBy.Cgpa => _students.OrderByDescending(s => s.Cgpa).ThenBy(s => s.Name).ToList(),
        _ => _students.OrderBy(s => s.Id).ToList()
    };

    public List<Student> TopStudents(int n) =>
        _students.Where(s => s.Results.Count > 0)
                 .OrderByDescending(s => s.Cgpa)
                 .ThenBy(s => s.Name)
                 .Take(n)
                 .ToList();

    // ---------- Results ----------

    /// <summary>Adds a course result, or updates marks if the course code already exists.</summary>
    public bool AddOrUpdateResult(int studentId, string code, string courseName, int credits, double marks)
    {
        var student = GetById(studentId) ?? throw new InvalidOperationException("Student not found.");
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Course code cannot be empty.");
        if (string.IsNullOrWhiteSpace(courseName)) throw new ArgumentException("Course name cannot be empty.");
        if (credits is < 1 or > 6) throw new ArgumentException("Credit hours must be between 1 and 6.");
        if (marks is < 0 or > 100) throw new ArgumentException("Marks must be between 0 and 100.");

        code = code.Trim().ToUpperInvariant();
        var existing = student.Results.FirstOrDefault(r => r.CourseCode == code);

        bool isNew = existing is null;
        if (existing is null)
        {
            student.Results.Add(new CourseResult
            {
                CourseCode = code, CourseName = courseName.Trim(), CreditHours = credits, Marks = marks
            });
        }
        else
        {
            existing.CourseName = courseName.Trim();
            existing.CreditHours = credits;
            existing.Marks = marks;
        }

        _repo.Save(_students);
        return isNew;
    }

    public bool RemoveResult(int studentId, string code)
    {
        var student = GetById(studentId) ?? throw new InvalidOperationException("Student not found.");
        int removed = student.Results.RemoveAll(r => r.CourseCode.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
        if (removed == 0) return false;
        _repo.Save(_students);
        return true;
    }

    // ---------- Reports ----------

    public List<DepartmentSummary> DepartmentReport() =>
        _students
            .GroupBy(s => s.Department, StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var top = g.OrderByDescending(s => s.Cgpa).First();
                return new DepartmentSummary(
                    g.Key, g.Count(), g.Average(s => s.Cgpa), top.Cgpa, top.Name);
            })
            .OrderByDescending(d => d.AverageCgpa)
            .ToList();

    // ---------- Demo data ----------

    public void SeedDemoDataIfEmpty()
    {
        if (_students.Count > 0) return;

        var a = AddStudent("Ayesha Khan", "Computer Science", 6);
        AddOrUpdateResult(a.Id, "CS301", "Data Structures", 3, 88);
        AddOrUpdateResult(a.Id, "CS305", "Operating Systems", 3, 82);
        AddOrUpdateResult(a.Id, "CS310", "Database Systems", 4, 91);

        var b = AddStudent("Bilal Ahmed", "Computer Science", 6);
        AddOrUpdateResult(b.Id, "CS301", "Data Structures", 3, 72);
        AddOrUpdateResult(b.Id, "CS305", "Operating Systems", 3, 66);
        AddOrUpdateResult(b.Id, "CS310", "Database Systems", 4, 78);

        var c = AddStudent("Hina Raza", "Software Engineering", 4);
        AddOrUpdateResult(c.Id, "SE201", "Software Design", 3, 79);
        AddOrUpdateResult(c.Id, "SE210", "Requirements Engineering", 3, 84);
    }
}
