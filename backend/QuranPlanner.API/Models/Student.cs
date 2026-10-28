namespace QuranPlanner.API.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string StageName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Plan> Plans { get; set; } = new();
}