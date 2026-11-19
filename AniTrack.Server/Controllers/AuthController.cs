using AniTrack.Server.Dtos;
using AniTrack.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AniTrack.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly SignInManager<AppUser> _signIn;

    public AuthController(UserManager<AppUser> users, SignInManager<AppUser> signIn)
    {
        _users = users;
        _signIn = signIn;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
    {
        var user = new AppUser { UserName = request.UserName, Email = request.Email };

        var result = await _users.CreateAsync(user, request.Password); // hashes the password
        if (!result.Succeeded)
        {
            // Turn Identity's errors (e.g. "username taken") into the same
            // Problem Details format as our other validation errors
            foreach (var error in result.Errors)
                ModelState.AddModelError(FieldFor(error.Code), error.Description);

            return ValidationProblem(ModelState);
        }

        await _signIn.SignInAsync(user, isPersistent: false); // log them in straight away
        return Ok(await ToDtoAsync(user));
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginRequest request)
    {
        var user = request.UserNameOrEmail.Contains('@')
            ? await _users.FindByEmailAsync(request.UserNameOrEmail)
            : await _users.FindByNameAsync(request.UserNameOrEmail);

        if (user is null)
            return InvalidCredentials();

        var result = await _signIn.PasswordSignInAsync(
            user, request.Password, request.RememberMe, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Problem(
                title: "Too many failed attempts. Try again in a few minutes.",
                statusCode: StatusCodes.Status429TooManyRequests);

        if (!result.Succeeded)
            return InvalidCredentials();

        return Ok(await ToDtoAsync(user));
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync(); // tells the browser to delete the cookie
        return NoContent();
    }

    // GET /api/auth/me  (React calls this on startup: "am I logged in?")
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return Unauthorized();
        return Ok(await ToDtoAsync(user));
    }

    private ObjectResult InvalidCredentials() =>
        Problem(title: "Invalid username or password.", statusCode: StatusCodes.Status401Unauthorized);

    private async Task<UserDto> ToDtoAsync(AppUser user) =>
        new(user.Id, user.UserName!, user.Email!, (await _users.GetRolesAsync(user)).ToList());

    private static string FieldFor(string identityErrorCode) => identityErrorCode switch
    {
        var c when c.Contains("Password") => nameof(RegisterRequest.Password),
        var c when c.Contains("UserName") => nameof(RegisterRequest.UserName),
        var c when c.Contains("Email") => nameof(RegisterRequest.Email),
        _ => string.Empty,
    };
}