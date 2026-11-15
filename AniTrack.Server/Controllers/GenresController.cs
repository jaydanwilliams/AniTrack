using AniTrack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniTrack.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly AnimeService _anime;

    public GenresController(AnimeService anime)
    {
        _anime = anime;
    }

    // GET /api/genres  (for the genre dropdown in the UI)
    [HttpGet]
    public async Task<ActionResult<List<string>>> GetGenres(CancellationToken ct)
    {
        return Ok(await _anime.GetGenresAsync(ct));
    }
}