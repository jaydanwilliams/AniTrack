using AniTrack.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace AniTrack.Server.Dtos;

public enum AnimeSort { Popularity, Score, Title, Newest }

public class AnimeQuery
{
    [MaxLength(100)]
    public string? Search { get; set; }

    public string? Genre { get; set; }

    [Range(1940, 2100)]
    public int? Year { get; set; }

    public AnimeSeason? Season { get; set; }
    public AnimeFormat? Format { get; set; }
    public AnimeStatus? Status { get; set; }

    public AnimeSort Sort { get; set; } = AnimeSort.Popularity;

    [Range(1, 10_000)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 24;
}