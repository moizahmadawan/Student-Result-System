namespace StudentResultSystem.Services;

/// <summary>
/// Converts marks (0-100) to a letter grade and grade points (4.0 scale).
/// Change the ranges here if your university uses a different scale.
/// </summary>
public static class GradeCalculator
{
    public static (string Letter, double Points) FromMarks(double marks) => marks switch
    {
        >= 85 => ("A", 4.00),
        >= 80 => ("A-", 3.67),
        >= 75 => ("B+", 3.33),
        >= 70 => ("B", 3.00),
        >= 65 => ("B-", 2.67),
        >= 60 => ("C+", 2.33),
        >= 55 => ("C", 2.00),
        >= 50 => ("C-", 1.67),
        >= 45 => ("D", 1.00),
        _ => ("F", 0.00)
    };
}
