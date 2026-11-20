using AniTrack.Server.Data;
using AniTrack.Server.Dtos;
using AniTrack.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AniTrack.Server.Services;

public enum SaveListEntryStatus { Created, Updated, AnimeNotFound, InvalidProgress }

public record SaveListEntryResult(SaveListEntryStatus Status, ListEntryDto? Entry = null, string? Error = null);

public class ListService
{
    private readonly AppDbContext _db;

    public ListService(AppDbContext db)
    {
        _db = db;
    }

    // Reusable projection: ListEntry → ListEntryDto, translated to SQL by EF Core
    private static readonly Expression<Func<ListEntry, ListEntryDto>> ToDto = e => new ListEntryDto(
        e.AnimeId,
        e.Status,
        e.Score,
        e.Progress,
        e.StartedOn,
        e.CompletedOn,
        e.Notes,
        e.UpdatedAt,
        new AnimeSummaryDto(
            e.Anime.Id,
            e.Anime.TitleEnglish ?? e.Anime.TitleRomaji,
            e.Anime.CoverImageUrl,
            e.Anime.Format,
            e.Anime.Season,
            e.Anime.SeasonYear,
            e.Anime.Episodes,
            e.Anime.AverageScore,
            e.Anime.Genres.OrderBy(g => g.Name).Select(g => g.Name).ToList()));

    public async Task<List<ListEntryDto>> GetListAsync(string userId, ListStatus? status, CancellationToken ct = default)
    {
        var query = _db.ListEntries.AsNoTracking().Where(e => e.UserId == userId);

        if (status is not null)
            query = query.Where(e => e.Status == status);

        return await query
            .OrderByDescending(e => e.UpdatedAt)
            .Select(ToDto)
            .ToListAsync(ct);
    }

    public async Task<ListEntryDto?> GetEntryAsync(string userId, int animeId, CancellationToken ct = default)
    {
        return await _db.ListEntries
            .AsNoTracking()
            .Where(e => e.UserId == userId && e.AnimeId == animeId)
            .Select(ToDto)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<SaveListEntryResult> SaveAsync(
        string userId, int animeId, SaveListEntryRequest request, CancellationToken ct = default)
    {
        // Only fetch the one column we need from the anime
        var anime = await _db.Anime
            .Where(a => a.Id == animeId)
            .Select(a => new { a.Episodes })
            .FirstOrDefaultAsync(ct);

        if (anime is null)
            return new(SaveListEntryStatus.AnimeNotFound);

        if (anime.Episodes is int total && request.Progress > total)
            return new(SaveListEntryStatus.InvalidProgress, Error: $"Progress can't be more than {total} episodes.");

        // Insert or update (the same upsert pattern as the AniList import)
        var entry = await _db.ListEntries
            .FirstOrDefaultAsync(e => e.UserId == userId && e.AnimeId == animeId, ct);

        var created = entry is null;
        if (entry is null)
        {
            entry = new ListEntry { UserId = userId, AnimeId = animeId, CreatedAt = DateTime.UtcNow };
            _db.ListEntries.Add(entry);
        }

        entry.Status = request.Status!.Value;
        entry.Score = request.Score;
        entry.Progress = request.Progress;
        entry.StartedOn = request.StartedOn;
        entry.CompletedOn = request.CompletedOn;
        entry.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        entry.UpdatedAt = DateTime.UtcNow;

        ApplyBusinessRules(entry, anime.Episodes);

        await _db.SaveChangesAsync(ct);

        var dto = await _db.ListEntries
            .AsNoTracking()
            .Where(e => e.Id == entry.Id)
            .Select(ToDto)
            .FirstAsync(ct);

        return new(created ? SaveListEntryStatus.Created : SaveListEntryStatus.Updated, dto);
    }

    public async Task<bool> DeleteAsync(string userId, int animeId, CancellationToken ct = default)
    {
        var deleted = await _db.ListEntries
            .Where(e => e.UserId == userId && e.AnimeId == animeId)
            .ExecuteDeleteAsync(ct);

        return deleted > 0;
    }

    private static void ApplyBusinessRules(ListEntry entry, int? totalEpisodes)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Watched every episode while "Watching" → it's completed
        if (entry.Status == ListStatus.Watching && totalEpisodes is int total && entry.Progress == total)
            entry.Status = ListStatus.Completed;

        // Completed → progress is the full episode count, and fill in the completion date
        if (entry.Status == ListStatus.Completed)
        {
            if (totalEpisodes is int t) entry.Progress = t;
            entry.CompletedOn ??= today;
        }

        // Started watching → fill in the start date
        if (entry.Status == ListStatus.Watching && entry.StartedOn is null)
            entry.StartedOn = today;
    }
}