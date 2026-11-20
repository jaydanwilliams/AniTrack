using AniTrack.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace AniTrack.Server.Dtos;

public class SaveListEntryRequest
{
    [Required]
    public ListStatus? Status { get; set; }

    [Range(1, 10)]
    public int? Score { get; set; }

    [Range(0, 10_000)]
    public int Progress { get; set; }

    public DateOnly? StartedOn { get; set; }
    public DateOnly? CompletedOn { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

public record ListEntryDto(
    int AnimeId,
    ListStatus Status,
    int? Score,
    int Progress,
    DateOnly? StartedOn,
    DateOnly? CompletedOn,
    string? Notes,
    DateTime UpdatedAt,
    AnimeSummaryDto Anime);