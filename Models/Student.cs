namespace DemoWebApplication.Models;

public class Student
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public string StudentEmail { get; set; }
    public int Age { get; set; }
    public Gender StudentGender { get; set; }
}
public enum Gender
{
    Male,
    Female
}
