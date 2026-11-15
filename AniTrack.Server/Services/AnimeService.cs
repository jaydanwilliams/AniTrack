using AniTrack.Server.Data;
using AniTrack.Server.Dtos;
using AniTrack.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace AniTrack.Server.Services;

public class AnimeService
{
    private readonly AppDbContext _db;

    public AnimeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AnimeSummaryDto>> SearchAsync(AnimeQuery q, CancellationToken ct = default)
    {
        // 1. Start with "all anime". Nothing runs against the database yet.
        IQueryable<Anime> query = _db.Anime.AsNoTracking();

        // 2. Add a filter for each option the user provided
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var term = q.Search.Trim();
            query = query.Where(a =>
                a.TitleRomaji.Contains(term) ||
                (a.TitleEnglish != null && a.TitleEnglish.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(q.Genre))
            query = query.Where(a => a.Genres.Any(g => g.Name == q.Genre));

        if (q.Year is not null)
            query = query.Where(a => a.SeasonYear == q.Year);

        if (q.Season is not null)
            query = query.Where(a => a.Season == q.Season);

        if (q.Format is not null)
            query = query.Where(a => a.Format == q.Format);

        if (q.Status is not null)
            query = query.Where(a => a.Status == q.Status);

        // 3. Sort. Always end with a unique column so the order never changes between requests.
        query = q.Sort switch
        {
            AnimeSort.Score => query.OrderByDescending(a => a.AverageScore).ThenBy(a => a.Id),
            AnimeSort.Title => query.OrderBy(a => a.TitleEnglish ?? a.TitleRomaji).ThenBy(a => a.Id),
            AnimeSort.Newest => query.OrderByDescending(a => a.StartDate).ThenBy(a => a.Id),
            _ => query.OrderByDescending(a => a.Popularity).ThenBy(a => a.Id),
        };

        // 4. Count how many match (query #1)
        var total = await query.CountAsync(ct);

        // 5. Get just this page, selecting only the fields we need (query #2)
        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(a => new AnimeSummaryDto(
                a.Id,
                a.TitleEnglish ?? a.TitleRomaji,
                a.CoverImageUrl,
                a.Format,
                a.Season,
                a.SeasonYear,
                a.Episodes,
                a.AverageScore,
                a.Genres.OrderBy(g => g.Name).Select(g => g.Name).ToList()))
            .ToListAsync(ct);

        return new PagedResult<AnimeSummaryDto>(items, q.Page, q.PageSize, total);
    }

    public async Task<AnimeDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Anime
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AnimeDetailDto(
                a.Id,
                a.AniListId,
                a.TitleRomaji,
                a.TitleEnglish,
                a.Description,
                a.CoverImageUrl,
                a.BannerImageUrl,
                a.Format,
                a.Status,
                a.Season,
                a.SeasonYear,
                a.Episodes,
                a.DurationMinutes,
                a.AverageScore,
                a.Popularity,
                a.StartDate,
                a.EndDate,
                a.Genres.OrderBy(g => g.Name).Select(g => g.Name).ToList()))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<string>> GetGenresAsync(CancellationToken ct = default)
    {
        return await _db.Genres
            .AsNoTracking()
            .OrderBy(g => g.Name)
            .Select(g => g.Name)
            .ToListAsync(ct);
    }
}