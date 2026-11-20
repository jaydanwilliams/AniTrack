using AniTrack.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AniTrack.Server.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Anime> Anime => Set<Anime>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<ListEntry> ListEntries => Set<ListEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

            // Add indexes for the columns we filter/sort on
            e.HasIndex(a => a.Popularity);
            e.HasIndex(a => a.AverageScore);
            e.HasIndex(a => a.SeasonYear);
        });

        modelBuilder.Entity<Genre>(e =>
        {
            e.HasIndex(g => g.Name).IsUnique();
            e.Property(g => g.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<ListEntry>(e =>
        {
            // One entry per user per anime. Also makes "get this user's list" fast,
            // because the index is sorted by UserId first.
            e.HasIndex(x => new { x.UserId, x.AnimeId }).IsUnique();

            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Notes).HasMaxLength(1000);

            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Anime).WithMany().HasForeignKey(x => x.AnimeId).OnDelete(DeleteBehavior.Cascade);

            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ListEntries_Score", "[Score] IS NULL OR [Score] BETWEEN 1 AND 10");
                t.HasCheckConstraint("CK_ListEntries_Progress", "[Progress] >= 0");
            });
        });
    }
}