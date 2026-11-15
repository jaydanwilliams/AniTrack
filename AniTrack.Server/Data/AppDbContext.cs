using AniTrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AniTrack.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Anime> Anime => Set<Anime>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>(e =>
        {
            e.HasIndex(a => a.AniListId).IsUnique();
            e.Property(a => a.TitleRomaji).HasMaxLength(300);
            e.Property(a => a.TitleEnglish).HasMaxLength(300);
            e.Property(a => a.CoverImageUrl).HasMaxLength(500);
            e.Property(a => a.BannerImageUrl).HasMaxLength(500);

            // Store enums as text ("TV") instead of numbers (0)
            e.Property(a => a.Format).HasConversion<string>().HasMaxLength(20);
            e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(a => a.Season).HasConversion<string>().HasMaxLength(10);
        });

        modelBuilder.Entity<Genre>(e =>
        {
            e.HasIndex(g => g.Name).IsUnique();
            e.Property(g => g.Name).HasMaxLength(50);
        });
    }
}