using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuranPlanner.API.Data;
using QuranPlanner.API.Models;
using QuranPlanner.API.Services;

namespace QuranPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PlanGeneratorService _gen;

    public PlansController(AppDbContext db, PlanGeneratorService gen)
    {
        _db  = db;
        _gen = gen;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlanDto dto)
    {
        Student? student = null;
        if (dto.StudentId.HasValue && dto.StudentId.Value > 0)
        {
            student = await _db.Students.FindAsync(dto.StudentId.Value);
            if (student == null) return BadRequest("Student not found");
        }
        else if (!string.IsNullOrWhiteSpace(dto.StudentName))
        {
            student = new Student { Name = dto.StudentName.Trim(), StageName = dto.StageName?.Trim() ?? "", CreatedAt = DateTime.UtcNow };
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
        }
        else return BadRequest("Student name required");

        var genReq = new PlanRequest
        {
            StartSurah = dto.StartSurah,
            StartAyah = dto.StartAyah,
            DurationWeeks = dto.DurationWeeks,
            Unit = "lines",
            AmountPerSession = dto.AmountLines,
            SessionsPerWeek = dto.DaysPerWeek,
            StudentName = student.Name,
            TajweedLevel = dto.TajweedLevel ?? "صغار"
        };

        var genRes = _gen.Generate(genReq);

        var plan = new Plan
        {
            StudentId = student.Id,
            StageName = dto.StageName ?? "",
            StartDate = dto.StartDate,
            AmountLabel = dto.AmountLabel,
            AmountLines = dto.AmountLines,
            DaysPerWeek = dto.DaysPerWeek,
            SurahCount = 0,
            RulesLesson = dto.RulesLesson,
            TajweedLevel = dto.TajweedLevel ?? "صغار",
            StartSurah = dto.StartSurah,
            StartAyah = dto.StartAyah,
            DurationWeeks = dto.DurationWeeks,
            CreatedAt = DateTime.UtcNow
        };
        _db.Plans.Add(plan);
        await _db.SaveChangesAsync();

        foreach (var s in genRes.Sessions)
        {
            _db.Sessions.Add(new Session
            {
                PlanId = plan.Id,
                SessionNumber = s.SessionNumber,
                WeekNumber = s.WeekNumber,
                Day = s.Day,
                Date = DateTime.TryParse(s.Date, out var d) ? d : dto.StartDate,
                FromRef = s.FromRef,
                ToRef = s.ToRef,
                FromPage = s.FromPage,
                ToPage = s.ToPage,
                PageRef = s.PageRef,
                LineRef = s.LineRef ?? "",
                Title = s.Title,
                Description = s.Description,
                Amount = s.Amount,
                Unit = s.Unit,
                Emoji = s.Emoji,
                ReviewRef = s.ReviewRef ?? "",
                TajweedLesson = s.TajweedLesson ?? "",
                IsSurahPromotion = s.IsSurahPromotion,
                PromotionSurahName = s.PromotionSurahName ?? "",
                Completed = false
            });
        }
        await _db.SaveChangesAsync();
        return Ok(new { planId = plan.Id, studentId = student.Id });
    }

    [HttpGet("by-student/{studentId}")]
    public async Task<IActionResult> ByStudent(int studentId)
    {
        var student = await _db.Students.FindAsync(studentId);
        if (student == null) return NotFound();

        var plans = await _db.Plans
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PlanSummaryDto
            {
                Id = p.Id,
                StudentId = p.StudentId,
                StudentName = student.Name,
                StageName = p.StageName,
                StartDate = p.StartDate,
                AmountLabel = p.AmountLabel,
                AmountLines = p.AmountLines,
                DaysPerWeek = p.DaysPerWeek,
                SurahCount = p.SurahCount,
                RulesLesson = p.RulesLesson,
                CreatedAt = p.CreatedAt,
                TotalSessions = p.Sessions.Count,
                CompletedSessions = p.Sessions.Count(s => s.Completed),
                ProgressPercent = p.Sessions.Count == 0 ? 0 : (double)p.Sessions.Count(s => s.Completed) / p.Sessions.Count * 100.0,
                StartSurah = p.StartSurah,
                StartAyah = p.StartAyah
            }).ToListAsync();

        return Ok(plans);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Plans.Include(x => x.Student).Include(x => x.Sessions).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return NotFound();

        int done = p.Sessions.Count(s => s.Completed);
        int total = p.Sessions.Count;

        var dto = new PlanDetailDto
        {
            Id = p.Id,
            StudentId = p.StudentId,
            StudentName = p.Student?.Name ?? "",
            StageName = p.StageName,
            StartDate = p.StartDate,
            AmountLabel = p.AmountLabel,
            AmountLines = p.AmountLines,
            DaysPerWeek = p.DaysPerWeek,
            SurahCount = p.SurahCount,
            RulesLesson = p.RulesLesson,
            Summary = $"{p.Student?.Name} - {p.StageName}",
            TotalSessions = total,
            CompletedSessions = done,
            ProgressPercent = total == 0 ? 0 : (double)done / total * 100.0,
            Sessions = p.Sessions.OrderBy(s => s.SessionNumber).Select(s => new SessionDto
            {
                Id = s.Id,
                SessionNumber = s.SessionNumber,
                WeekNumber = s.WeekNumber,
                Day = s.Day,
                Date = s.Date,
                FromRef = s.FromRef,
                ToRef = s.ToRef,
                FromPage = s.FromPage,
                ToPage = s.ToPage,
                PageRef = s.PageRef,
                LineRef = s.LineRef,
                Title = s.Title,
                Description = s.Description,
                Amount = s.Amount,
                Unit = s.Unit,
                Emoji = s.Emoji,
                Completed = s.Completed,
                ReviewRef = s.ReviewRef,
                TajweedLesson = s.TajweedLesson,
                IsSurahPromotion = s.IsSurahPromotion,
                PromotionSurahName = s.PromotionSurahName,
                Stars = s.Stars
            }).ToList()
        };
        return Ok(dto);
    }

    [HttpPost("session/{sessionId}/toggle")]
    public async Task<IActionResult> ToggleSession(int sessionId)
    {
        var s = await _db.Sessions.FindAsync(sessionId);
        if (s == null) return NotFound();
        s.Completed = !s.Completed;
        s.CompletedAt = s.Completed ? DateTime.UtcNow : null;
        await _db.SaveChangesAsync();
        return Ok(new { completed = s.Completed });
    }

    [HttpPost("session/{sessionId}/rate")]
    public async Task<IActionResult> RateSession(int sessionId, [FromBody] int stars)
    {
        var s = await _db.Sessions.FindAsync(sessionId);
        if (s == null) return NotFound();
        s.Stars = Math.Clamp(stars, 1, 5);
        s.Completed = true;
        s.CompletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { stars = s.Stars, completed = s.Completed });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Plans.FindAsync(id);
        if (p == null) return NotFound();
        _db.Plans.Remove(p);
        await _db.SaveChangesAsync();
        return Ok(new { deleted = true });
    }
}