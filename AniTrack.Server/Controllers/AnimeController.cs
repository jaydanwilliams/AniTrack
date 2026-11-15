using AniTrack.Server.Dtos;
using AniTrack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniTrack.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimeController : ControllerBase
{
    private readonly AnimeService _anime;

    public AnimeController(AnimeService anime)
    {
        _anime = anime;
    }

    // GET /api/anime?search=titan&genre=Action&sort=Score&page=1
    [HttpGet]
    public async Task<ActionResult<PagedResult<AnimeSummaryDto>>> GetAnime(
        [FromQuery] AnimeQuery query, CancellationToken ct)
    {
        return Ok(await _anime.SearchAsync(query, ct));
    }

    // GET /api/anime/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnimeDetailDto>> GetById(int id, CancellationToken ct)
    {
        var anime = await _anime.GetByIdAsync(id, ct);
        return anime is null ? NotFound() : Ok(anime);
    }
}