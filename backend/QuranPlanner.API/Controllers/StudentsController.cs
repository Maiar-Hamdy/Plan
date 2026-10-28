using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuranPlanner.API.Data;
using QuranPlanner.API.Models;

namespace QuranPlanner.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Students
            .OrderBy(s => s.Name)
            .Select(s => new StudentDto
            {
                Id        = s.Id,
                Name      = s.Name,
                StageName = s.StageName,
                CreatedAt = s.CreatedAt,
                PlanCount = s.Plans.Count
            })
            .ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s == null) return NotFound();

        return Ok(new StudentDto
        {
            Id        = s.Id,
            Name      = s.Name,
            StageName = s.StageName,
            CreatedAt = s.CreatedAt,
            PlanCount = await _db.Plans.CountAsync(p => p.StudentId == id)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required");

        var s = new Student
        {
            Name      = dto.Name.Trim(),
            StageName = dto.StageName?.Trim() ?? "",
            CreatedAt = DateTime.UtcNow
        };
        _db.Students.Add(s);
        await _db.SaveChangesAsync();

        return Ok(new StudentDto
        {
            Id = s.Id, Name = s.Name, StageName = s.StageName,
            CreatedAt = s.CreatedAt, PlanCount = 0
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _db.Students.FindAsync(id);
        if (s == null) return NotFound();
        _db.Students.Remove(s);
        await _db.SaveChangesAsync();
        return Ok(new { deleted = true });
    }
}