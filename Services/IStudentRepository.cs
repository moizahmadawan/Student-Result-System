using StudentResultSystem.Models;

namespace StudentResultSystem.Services;

/// <summary>Storage abstraction: JSON today, a database tomorrow.</summary>
public interface IStudentRepository
{
    List<Student> Load();
    void Save(IEnumerable<Student> students);
}
