using Microsoft.EntityFrameworkCore;
using QuranPlanner.API.Data;
using QuranPlanner.API.Services;

var builder = WebApplication.CreateBuilder(args);

// SQL Server LocalDB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<QuranDataService>();
builder.Services.AddSingleton<AyahLocationService>();
builder.Services.AddSingleton<PlanGeneratorService>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("[DB] Migrations applied");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DB] Migration failed: {ex.Message}");
    }
}

app.Run();