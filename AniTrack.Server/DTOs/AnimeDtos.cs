using AniTrack.Server.Models;

namespace AniTrack.Server.Dtos;

// Small: used in grids and lists
public record AnimeSummaryDto(
    int Id,
    string Title,
    string? CoverImageUrl,
    AnimeFormat? Format,
    AnimeSeason? Season,
    int? SeasonYear,
    int? Episodes,
    int? AverageScore,
    List<string> Genres);

// Full: used on the detail page
public record AnimeDetailDto(
    int Id,
    int AniListId,
    string TitleRomaji,
    string? TitleEnglish,
    string? Description,
    string? CoverImageUrl,
    string? BannerImageUrl,
    AnimeFormat? Format,
    AnimeStatus? Status,
    AnimeSeason? Season,
    int? SeasonYear,
    int? Episodes,
    int? DurationMinutes,
    int? AverageScore,
    int? Popularity,
    DateOnly? StartDate,
    DateOnly? EndDate,
    List<string> Genres);