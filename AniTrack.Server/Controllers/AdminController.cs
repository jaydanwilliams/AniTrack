using AniTrack.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AniTrack.Server.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AnimeImportService _import;

    public AdminController(AnimeImportService import)
    {
        _import = import;
    }

    // POST /api/admin/import?pages=4
    [HttpPost("import")]
    public async Task<IActionResult> Import([FromQuery] int pages = 2, CancellationToken ct = default)
    {
        if (pages is < 1 or > 20)
            return BadRequest("pages must be between 1 and 20.");

        var imported = await _import.ImportPopularAsync(pages, ct);
        return Ok(new { imported });
    }
}