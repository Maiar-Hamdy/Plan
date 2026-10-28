namespace QuranPlanner.API.Models;

public class Session
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public Plan? Plan { get; set; }

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

    // تفاصيل إضافية
    public string ReviewRef { get; set; } = "";
    public string TajweedLesson { get; set; } = "";
    public bool IsSurahPromotion { get; set; } = false;
    public string PromotionSurahName { get; set; } = "";
    public int Stars { get; set; } = 0;

    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public double Amount { get; set; }
    public string Unit { get; set; } = "";
    public string Emoji { get; set; } = "*";

    public bool Completed { get; set; } = false;
    public DateTime? CompletedAt { get; set; }
}