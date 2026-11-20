using AniTrack.Server.Dtos;
using AniTrack.Server.Extensions;
using AniTrack.Server.Models;
using AniTrack.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniTrack.Server.Controllers;

[ApiController]
[Route("api/list")]
[Authorize]                       // every action requires a logged-in user
public class ListController : ControllerBase
{
    private readonly ListService _list;

    public ListController(ListService list)
    {
        _list = list;
    }

    private string UserId => User.GetUserId();

    // GET /api/list?status=Watching
    [HttpGet]
    public async Task<ActionResult<List<ListEntryDto>>> GetMyList([FromQuery] ListStatus? status, CancellationToken ct)
    {
        return Ok(await _list.GetListAsync(UserId, status, ct));
    }

    // GET /api/list/5  → my entry for anime 5 (404 if it's not on my list)
    [HttpGet("{animeId:int}")]
    public async Task<ActionResult<ListEntryDto>> GetEntry(int animeId, CancellationToken ct)
    {
        var entry = await _list.GetEntryAsync(UserId, animeId, ct);
        return entry is null ? NotFound() : Ok(entry);
    }

    // PUT /api/list/5  → add anime 5 to my list, or update my entry
    [HttpPut("{animeId:int}")]
    public async Task<ActionResult<ListEntryDto>> Save(int animeId, SaveListEntryRequest request, CancellationToken ct)
    {
        var result = await _list.SaveAsync(UserId, animeId, request, ct);

        return result.Status switch
        {
            SaveListEntryStatus.AnimeNotFound => NotFound(),
            SaveListEntryStatus.InvalidProgress => FieldError(nameof(request.Progress), result.Error!),
            SaveListEntryStatus.Created => CreatedAtAction(nameof(GetEntry), new { animeId }, result.Entry),
            _ => Ok(result.Entry),
        };
    }

    // DELETE /api/list/5
    [HttpDelete("{animeId:int}")]
    public async Task<IActionResult> Delete(int animeId, CancellationToken ct)
    {
        return await _list.DeleteAsync(UserId, animeId, ct) ? NoContent() : NotFound();
    }

    private ActionResult FieldError(string field, string message)
    {
        ModelState.AddModelError(field, message);
        return ValidationProblem(ModelState);
    }
}