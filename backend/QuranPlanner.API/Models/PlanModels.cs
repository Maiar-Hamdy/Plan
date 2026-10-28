namespace QuranPlanner.API.Models;

public class PlanRequest
{
    public string StartSurah { get; set; } = "Al-Fatiha";
    public int StartAyah { get; set; } = 1;
    public int DurationWeeks { get; set; } = 4;
    public string Unit { get; set; } = "lines";
    public double AmountPerSession { get; set; } = 5;
    public int SessionsPerWeek { get; set; } = 5;
    public List<string> PreferredDays { get; set; } = new() { "Sun","Mon","Tue","Wed","Thu" };
    public string StudentName { get; set; } = "Little Star";
    public int Age { get; set; } = 8;
    public string TajweedLevel { get; set; } = "صغار";
}

public class SessionTask
{
    public int SessionNumber { get; set; }
    public int WeekNumber { get; set; }
    public string Day { get; set; } = "";
    public string Date { get; set; } = "";
    
    public string FromRef { get; set; } = "";
    public string ToRef { get; set; } = "";
    public int FromPage { get; set; }
    public int ToPage { get; set; }
    public string PageRef { get; set; } = "";
    public string LineRef { get; set; } = "";
    
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Amount { get; set; }
    public string Unit { get; set; } = "";
    public string Emoji { get; set; } = "*";
    
    // الحقول الجديدة
    public string ReviewRef { get; set; } = "";
    public string TajweedLesson { get; set; } = "";
    public bool IsSurahPromotion { get; set; } = false;
    public string PromotionSurahName { get; set; } = "";
    public int Stars { get; set; } = 0;
    
    public bool Completed { get; set; } = false;
}

public class Milestone
{
    public string Title { get; set; } = "";
    public string Emoji { get; set; } = "*";
    public string Date { get; set; } = "";
}

public class PlanResponse
{
    public string StudentName { get; set; } = "";
    public string Summary { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    public int TotalSessions { get; set; }
    public string EstimatedFinish { get; set; } = "";
    public List<SessionTask> Sessions { get; set; } = new();
    public List<Milestone> Milestones { get; set; } = new();
}