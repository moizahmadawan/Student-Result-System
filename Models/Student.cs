using System.Text.Json.Serialization;

namespace StudentResultSystem.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Department { get; set; } = "";
    public int Semester { get; set; }
    public List<CourseResult> Results { get; set; } = new();

    [JsonIgnore]
    public int TotalCredits => Results.Sum(r => r.CreditHours);

    /// <summary>Credit-weighted GPA on a 4.0 scale (0 if no courses yet).</summary>
    [JsonIgnore]
    public double Cgpa
    {
        get
        {
            int credits = TotalCredits;
            if (credits == 0) return 0;
            return Results.Sum(r => r.GradePoints * r.CreditHours) / credits;
        }
    }
}
