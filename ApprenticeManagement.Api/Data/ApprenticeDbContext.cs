using Microsoft.EntityFrameworkCore;
using ApprenticeManagement.Api.Models;

namespace ApprenticeManagement.Api.Data;

public class ApprenticeDbContext(DbContextOptions<ApprenticeDbContext> options) : DbContext(options)
{
    public DbSet<Apprentice> Apprentices => Set<Apprentice>();
    public DbSet<WorkJournalEntry> WorkJournalEntries => Set<WorkJournalEntry>();
    public DbSet<Grade> Grades => Set<Grade>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Apprentice>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Email).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Program).IsRequired().HasMaxLength(200);
            entity.HasIndex(a => a.Email).IsUnique();
        });

        modelBuilder.Entity<WorkJournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.HasOne(e => e.Apprentice)
                  .WithMany(a => a.WorkJournalEntries)
                  .HasForeignKey(e => e.ApprenticeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Subject).IsRequired().HasMaxLength(200);
            entity.HasOne(g => g.Apprentice)
                  .WithMany(a => a.Grades)
                  .HasForeignKey(g => g.ApprenticeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
