namespace AniTrack.Server.Models;

public class Anime
{
    public int Id { get; set; }                  // our primary key
    public int AniListId { get; set; }           // the ID on AniList, used when syncing

    public string TitleRomaji { get; set; } = string.Empty;
    public string? TitleEnglish { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? BannerImageUrl { get; set; }

    public AnimeFormat? Format { get; set; }
    public AnimeStatus? Status { get; set; }
    public AnimeSeason? Season { get; set; }
    public int? SeasonYear { get; set; }
    public int? Episodes { get; set; }
    public int? DurationMinutes { get; set; }
    public int? AverageScore { get; set; }       // 0–100, from AniList
    public int? Popularity { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public DateTime LastSyncedAt { get; set; }   // when we last refreshed this row from AniList

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}