using AniTrack.Server.Data;
using AniTrack.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniTrack.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimeController : ControllerBase
{
    private readonly AppDbContext _db;

    public AnimeController(AppDbContext db)   // ← DI provides the DbContext
    {
        _db = db;
    }

    // GET /api/anime
    [HttpGet]
    public async Task<ActionResult<List<Anime>>> GetAll()
    {
        var anime = await _db.Anime.AsNoTracking().ToListAsync();
        return Ok(anime);
    }
}