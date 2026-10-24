using System.Text.Json;
using StudentResultSystem.Models;

namespace StudentResultSystem.Services;

public class JsonStudentRepository : IStudentRepository
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly string _path;

    public JsonStudentRepository(string path) => _path = path;

    public List<Student> Load()
    {
        if (!File.Exists(_path)) return new List<Student>();

        try
        {
            string json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
        }
        catch (JsonException)
        {
            // Corrupted file: keep a backup and start fresh instead of crashing.
            File.Move(_path, _path + ".corrupt", overwrite: true);
            return new List<Student>();
        }
    }

    public void Save(IEnumerable<Student> students)
    {
        string? dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(_path, JsonSerializer.Serialize(students, Options));
    }
}
