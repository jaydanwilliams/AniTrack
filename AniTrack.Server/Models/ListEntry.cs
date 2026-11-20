namespace AniTrack.Server.Models;

public class ListEntry
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    public int AnimeId { get; set; }
    public Anime Anime { get; set; } = null!;

    public ListStatus Status { get; set; }
    public int? Score { get; set; }
    public int Progress { get; set; }
    public DateOnly? StartedOn { get; set; }
    public DateOnly? CompletedOn { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}