using System.Text.Json.Serialization;
using StudentResultSystem.Services;

namespace StudentResultSystem.Models;

/// <summary>Result of one course for one student.</summary>
public class CourseResult
{
    public string CourseCode { get; set; } = "";
    public string CourseName { get; set; } = "";
    public int CreditHours { get; set; }
    public double Marks { get; set; }

    [JsonIgnore]
    public string Grade => GradeCalculator.FromMarks(Marks).Letter;

    [JsonIgnore]
    public double GradePoints => GradeCalculator.FromMarks(Marks).Points;
}
