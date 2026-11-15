using AniTrack.Server.Data;
using AniTrack.Server.Models;
using AniTrack.Server.Services.AniList;
using Microsoft.EntityFrameworkCore;

namespace AniTrack.Server.Services;

public class AnimeImportService
{
    private const int PageSize = 50; // AniList's maximum per page

    private readonly AppDbContext _db;
    private readonly AniListClient _aniList;
    private readonly ILogger<AnimeImportService> _logger;

    public AnimeImportService(AppDbContext db, AniListClient aniList, ILogger<AnimeImportService> logger)
    {
        _db = db;
        _aniList = aniList;
        _logger = logger;
    }

    public async Task<int> ImportPopularAsync(int pages, CancellationToken ct = default)
    {
        var total = 0;

        for (var page = 1; page <= pages; page++)
        {
            var result = await _aniList.GetPopularAnimeAsync(page, PageSize, ct);
            await UpsertAsync(result.Media, ct);
            total += result.Media.Count;

            _logger.LogInformation("Imported page {Page} ({Count} anime)", page, result.Media.Count);

            if (!result.PageInfo.HasNextPage) break;

            // AniList limits requests per minute, so wait a little between pages
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }

        return total;
    }

    private async Task UpsertAsync(List<AniListMedia> mediaList, CancellationToken ct)
    {
        // 1. Genres: load the ones that already exist (one query), create the missing ones
        var genreNames = mediaList.SelectMany(m => m.Genres).Distinct().ToList();

        var genres = await _db.Genres
            .Where(g => genreNames.Contains(g.Name))
            .ToDictionaryAsync(g => g.Name, ct);

        foreach (var name in genreNames.Where(n => !genres.ContainsKey(n)))
        {
            var genre = new Genre { Name = name };
            _db.Genres.Add(genre);
            genres[name] = genre;
        }

        // 2. Anime: load the ones we already have (one query)
        var aniListIds = mediaList.Select(m => m.Id).ToList();

        var existing = await _db.Anime
            .Include(a => a.Genres)
            .Where(a => aniListIds.Contains(a.AniListId))
            .ToDictionaryAsync(a => a.AniListId, ct);

        // 3. Update the ones we have, insert the new ones
        foreach (var media in mediaList)
        {
            if (!existing.TryGetValue(media.Id, out var anime))
            {
                anime = new Anime { AniListId = media.Id };
                _db.Anime.Add(anime);
            }

            Map(media, anime, genres);
        }

        // 4. Save everything in one go
        await _db.SaveChangesAsync(ct);
    }

    private static void Map(AniListMedia src, Anime dest, Dictionary<string, Genre> genres)
    {
        dest.TitleRomaji = src.Title.Romaji ?? src.Title.English ?? "Untitled";
        dest.TitleEnglish = src.Title.English;
        dest.Description = src.Description;
        dest.CoverImageUrl = src.CoverImage?.Large;
        dest.BannerImageUrl = src.BannerImage;
        dest.Format = MapFormat(src.Format);
        dest.Status = MapStatus(src.Status);
        dest.Season = MapSeason(src.Season);
        dest.SeasonYear = src.SeasonYear;
        dest.Episodes = src.Episodes;
        dest.DurationMinutes = src.Duration;
        dest.AverageScore = src.AverageScore;
        dest.Popularity = src.Popularity;
        dest.StartDate = ToDateOnly(src.StartDate);
        dest.EndDate = ToDateOnly(src.EndDate);
        dest.LastSyncedAt = DateTime.UtcNow;

        // Make the anime's genres match AniList's: remove ones that are gone, add new ones
        var newGenres = src.Genres.Select(name => genres[name]).ToList();

        foreach (var g in dest.Genres.Where(g => !newGenres.Contains(g)).ToList())
            dest.Genres.Remove(g);

        foreach (var g in newGenres.Where(g => !dest.Genres.Contains(g)))
            dest.Genres.Add(g);
    }

    private static AnimeFormat? MapFormat(string? value) => value switch
    {
        "TV" => AnimeFormat.TV,
        "TV_SHORT" => AnimeFormat.TVShort,
        "MOVIE" => AnimeFormat.Movie,
        "SPECIAL" => AnimeFormat.Special,
        "OVA" => AnimeFormat.OVA,
        "ONA" => AnimeFormat.ONA,
        "MUSIC" => AnimeFormat.Music,
        _ => null
    };

    private static AnimeStatus? MapStatus(string? value) => value switch
    {
        "FINISHED" => AnimeStatus.Finished,
        "RELEASING" => AnimeStatus.Releasing,
        "NOT_YET_RELEASED" => AnimeStatus.NotYetReleased,
        "CANCELLED" => AnimeStatus.Cancelled,
        "HIATUS" => AnimeStatus.Hiatus,
        _ => null
    };

    private static AnimeSeason? MapSeason(string? value) => value switch
    {
        "WINTER" => AnimeSeason.Winter,
        "SPRING" => AnimeSeason.Spring,
        "SUMMER" => AnimeSeason.Summer,
        "FALL" => AnimeSeason.Fall,
        _ => null
    };

    private static DateOnly? ToDateOnly(AniListFuzzyDate? d) =>
        d is { Year: int y, Month: int m, Day: int day } ? new DateOnly(y, m, day) : null;
}