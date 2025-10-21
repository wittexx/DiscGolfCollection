using Microsoft.EntityFrameworkCore;

namespace MyDiscgolfDiscs.Models;

public class DiscContext : DbContext
{
    public DbSet<Disc> Discs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Create database in the application directory
        var dbPath = Path.Combine(Environment.CurrentDirectory, "discs.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Disc entity
        modelBuilder.Entity<Disc>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Brand).HasMaxLength(50);
            entity.Property(d => d.Description).HasMaxLength(500);
            entity.Property(d => d.Plastic).HasMaxLength(20);
            entity.Property(d => d.Color).HasMaxLength(20);
            entity.Property(d => d.ImagePath).HasMaxLength(500);
            entity.Property(d => d.Category).HasConversion<int>();
        });
    }
}
