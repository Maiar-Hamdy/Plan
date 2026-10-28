using Microsoft.EntityFrameworkCore;
using QuranPlanner.API.Models;

namespace QuranPlanner.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<Session> Sessions => Set<Session>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Student>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.StageName).HasMaxLength(200);
        });

        mb.Entity<Plan>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Student)
             .WithMany(s => s.Plans)
             .HasForeignKey(x => x.StudentId)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.AmountLabel).HasMaxLength(50);
            e.Property(x => x.StageName).HasMaxLength(200);
            e.Property(x => x.StartSurah).HasMaxLength(50);
            e.Property(x => x.RulesLesson).HasMaxLength(500);
        });

        mb.Entity<Session>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Plan)
             .WithMany(p => p.Sessions)
             .HasForeignKey(x => x.PlanId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}