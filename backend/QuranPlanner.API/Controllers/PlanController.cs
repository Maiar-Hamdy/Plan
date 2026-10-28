using Microsoft.AspNetCore.Mvc;
using QuranPlanner.API.Models;
using QuranPlanner.API.Services;

namespace QuranPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanController : ControllerBase
{
    private readonly PlanGeneratorService _gen;
    private readonly QuranDataService _quran;

    public PlanController(PlanGeneratorService gen, QuranDataService quran)
    {
        _gen   = gen;
        _quran = quran;
    }

    [HttpGet("surahs")]
    public IActionResult GetSurahs() => Ok(_quran.Surahs);

        [HttpGet("surahs-by-juz/{juzNumber}")]
    public IActionResult GetSurahsByJuz(int juzNumber)
    {
        if (juzNumber < 1 || juzNumber > 30) return BadRequest("Juz must be between 1 and 30");

        // الحساب التقريبي: كل جزء 20 صفحة.
        // الجزء الأول يبدأ من صفحة 1، الجزء الثاني من 22، وهكذا.
        // الصيغة التقريبية: (juz - 1) * 20 + 1
        int startPage = ((juzNumber - 1) * 20) + 1;
        int endPage = startPage + 19;

        // جلب السور التي تتداخل مع هذه الصفحات
        var surahs = _quran.Surahs.Where(s => s.EndPage >= startPage && s.StartPage <= endPage).ToList();
        
        return Ok(surahs);
    }

    [HttpGet("health")]
    public IActionResult Health() =>
        Ok(new { status = "ok", time = DateTime.UtcNow });

    [HttpPost("generate")]
    public IActionResult Generate([FromBody] PlanRequest req)
    {
        if (req.DurationWeeks  < 1) req.DurationWeeks  = 1;
        if (req.SessionsPerWeek < 1) req.SessionsPerWeek = 1;
        if (req.SessionsPerWeek > 7) req.SessionsPerWeek = 7;
        if (req.AmountPerSession <= 0) req.AmountPerSession = 1;
        if (req.PreferredDays == null || req.PreferredDays.Count == 0)
            req.PreferredDays = new() { "Sun","Mon","Tue","Wed","Thu" };

        var plan = _gen.Generate(req);
        return Ok(plan);
    }
}