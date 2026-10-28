namespace QuranPlanner.API.Models;

// ===== Student =====
public class StudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string StageName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int PlanCount { get; set; }
}

public class CreateStudentDto
{
    public string Name { get; set; } = "";
    public string StageName { get; set; } = "";
}

// ===== Plan =====
public class CreatePlanDto
{
    // Ã˜Â¨Ã™Å Ã˜Â§Ã™â€ Ã˜Â§Ã˜Âª Ã˜Â§Ã™â€žÃ˜Â·Ã˜Â§Ã™â€žÃ˜Â¨ (Ã˜Â¬Ã˜Â¯Ã™Å Ã˜Â¯ Ã˜Â£Ã™Ë† Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯)
    public int? StudentId { get; set; }       // Ã™â€žÃ™Ë† Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯
    public string? StudentName { get; set; }  // Ã™â€žÃ™Ë† Ã˜Â¬Ã˜Â¯Ã™Å Ã˜Â¯
    public string StageName { get; set; } = "";

    // Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â±Ã˜Â­Ã™â€žÃ˜Â©
    public DateTime StartDate { get; set; } = DateTime.Today;
    public string AmountLabel { get; set; } = "";  // Ã™â€¦Ã˜Â«Ã˜Â§Ã™â€ž: "Ã˜ÂµÃ™ÂÃ˜Â­Ã˜Â©"
    public double AmountLines { get; set; } = 15;  // Ã˜Â§Ã™â€žÃ˜Â£Ã˜Â³Ã˜Â·Ã˜Â± Ã˜Â§Ã™â€žÃ™â€¦Ã™Æ’Ã˜Â§Ã™ÂÃ˜Â¦Ã˜Â©
    public int DaysPerWeek { get; set; } = 5;
    

    // Ã˜ÂªÃ™Ë†Ã˜Â§Ã˜Â±Ã™Å Ã˜Â®
    
    
    

    public string? RulesLesson { get; set; }
    public string TajweedLevel { get; set; } = "ØµØºØ§Ø±";

    // Ã™â€ Ã™â€šÃ˜Â·Ã˜Â© Ã˜Â§Ã™â€žÃ˜Â¨Ã˜Â¯Ã˜Â§Ã™Å Ã˜Â©
    public string StartSurah { get; set; } = "Al-Fatiha";
    public int StartAyah { get; set; } = 1;

    // Ã˜Â§Ã™â€žÃ™â€¦Ã˜Â¯Ã˜Â©
    public int DurationWeeks { get; set; } = 4;
}

public class PlanSummaryDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string StageName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public string AmountLabel { get; set; } = "";
    public double AmountLines { get; set; }
    public int DaysPerWeek { get; set; }
    public int SurahCount { get; set; }
    
    
    
    public string? RulesLesson { get; set; }
    public string TajweedLevel { get; set; } = "ØµØºØ§Ø±";
    public DateTime CreatedAt { get; set; }
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public double ProgressPercent { get; set; }
    public string? StartSurah { get; set; }
    public int StartAyah { get; set; }
}

public class SessionDto
{
    public int Id { get; set; }
    public int SessionNumber { get; set; }
    public int WeekNumber { get; set; }
    public string Day { get; set; } = "";
    public DateTime Date { get; set; }
    public string FromRef { get; set; } = "";
    public string ToRef { get; set; } = "";
    public int FromPage { get; set; }
    public int ToPage { get; set; }
    public int FromLine { get; set; }
    public int ToLine { get; set; }
    public string PageRef { get; set; } = "";
    public string LineRef { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Amount { get; set; }
    public string Unit { get; set; } = "";
    public string ReviewRef { get; set; } = "";
    public string TajweedLesson { get; set; } = "";
    public bool IsSurahPromotion { get; set; }
    public string PromotionSurahName { get; set; } = "";
    public int Stars { get; set; }
    public string Emoji { get; set; } = "*";
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class PlanDetailDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string StageName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public string AmountLabel { get; set; } = "";
    public double AmountLines { get; set; }
    public int DaysPerWeek { get; set; }
    public int SurahCount { get; set; }
    
    
    
    public string? RulesLesson { get; set; }
    public string TajweedLevel { get; set; } = "ØµØºØ§Ø±";
    public string Summary { get; set; } = "";
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public double ProgressPercent { get; set; }
    public List<SessionDto> Sessions { get; set; } = new();
}