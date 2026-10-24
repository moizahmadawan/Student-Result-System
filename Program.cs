using StudentResultSystem.Services;
using StudentResultSystem.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Data is stored in ./data/students.json (relative to where you run the program)
string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "students.json");

var service = new StudentService(new JsonStudentRepository(dataPath));
service.SeedDemoDataIfEmpty();   // only runs on the very first launch

try
{
    new ConsoleApp(service).Run();
}
catch (EndOfStreamException)
{
    // input stream closed - exit quietly
}
