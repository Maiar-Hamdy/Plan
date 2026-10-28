namespace QuranPlanner.API.Models;

public class Plan
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    // Ø¨ÙŠØ§Ù†Ø§Øª Ø§Ù„Ù…Ø±Ø­Ù„Ø©
    public string StageName { get; set; } = "";
    public DateTime StartDate { get; set; }

    // ÙƒÙ…ÙŠØ© Ø§Ù„Ø­ÙØ¸ (Ù†Øµ ÙˆØµÙÙŠ + Ø£Ø³Ø·Ø± Ù…ÙƒØ§ÙØ¦Ø©)
    public string AmountLabel { get; set; } = "";
    public double AmountLines { get; set; }

    // Ø¹Ø¯Ø¯ Ø£ÙŠØ§Ù… Ø§Ù„Ø­Ø¶ÙˆØ± Ø£Ø³Ø¨ÙˆØ¹ÙŠØ§Ù‹
    public int DaysPerWeek { get; set; } = 5;

    // Ø¹Ø¯Ø¯ Ø³ÙˆØ± Ø§Ù„Ù…Ø±Ø­Ù„Ø©
    public int SurahCount { get; set; }

    // ØªÙˆØ§Ø±ÙŠØ® Ø§Ù„Ù…Ø±Ø­Ù„Ø©
    public DateTime? PrepDate { get; set; }          // Ù…ÙˆØ¹Ø¯ ØªØ¬Ù‡ÙŠØ² Ø§Ù„Ù…Ø±Ø­Ù„Ø©
    public DateTime? PromotionDate { get; set; }     // Ù…ÙˆØ¹Ø¯ ØªØµØ¹ÙŠØ¯ Ø§Ù„Ù…Ø±Ø­Ù„Ø©
    public DateTime? LinkingDate { get; set; }       // Ù…ÙˆØ¹Ø¯ ØªØ¬Ù‡ÙŠØ² Ø±Ø¨Ø· Ø§Ù„Ù…Ø±Ø­Ù„Ø©

    // Ø¯Ø±Ø³ Ø§Ù„Ø£Ø­ÙƒØ§Ù…
    public string? RulesLesson { get; set; }
    public string TajweedLevel { get; set; } = "صغار"; // صغار | شباب | كبار

    // Ù†Ù‚Ø·Ø© Ø§Ù„Ø¨Ø¯Ø§ÙŠØ©
    public string StartSurah { get; set; } = "Al-Fatiha";
    public int StartAyah { get; set; } = 1;

    // Ø§Ù„Ù…Ø¯Ø©
    public int DurationWeeks { get; set; } = 4;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Session> Sessions { get; set; } = new();
}