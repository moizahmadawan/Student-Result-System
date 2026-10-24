namespace StudentResultSystem.Models;

public record DepartmentSummary(
    string Department,
    int StudentCount,
    double AverageCgpa,
    double HighestCgpa,
    string TopStudent);
