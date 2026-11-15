namespace AniTrack.Server.Services.AniList;

public record AniListResponse(AniListData? Data);
public record AniListData(AniListPage Page);
public record AniListPage(AniListPageInfo PageInfo, List<AniListMedia> Media);
public record AniListPageInfo(bool HasNextPage, int CurrentPage);

public record AniListMedia(
    int Id,
    AniListTitle Title,
    string? Description,
    AniListCoverImage? CoverImage,
    string? BannerImage,
    string? Format,
    string? Status,
    string? Season,
    int? SeasonYear,
    int? Episodes,
    int? Duration,
    int? AverageScore,
    int? Popularity,
    AniListFuzzyDate? StartDate,
    AniListFuzzyDate? EndDate,
    List<string> Genres);

public record AniListTitle(string? Romaji, string? English);
public record AniListCoverImage(string? Large);
public record AniListFuzzyDate(int? Year, int? Month, int? Day);